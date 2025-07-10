using Microsoft.Playwright;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MeshcapadeDataScraper.Configuration;
using MeshcapadeDataScraper.Models;
using MeshcapadeDataScraper.Data;
using HtmlAgilityPack;
using Polly;
using System.Text.Json;

namespace MeshcapadeDataScraper.Services;

public class MeshcapadeScraper
{
    private readonly ILogger<MeshcapadeScraper> _logger;
    private readonly ScrapingConfig _config;
    private readonly MeshcapadeDbContext _dbContext;
    private IBrowser? _browser;
    private IPlaywright? _playwright;

    public MeshcapadeScraper(
        ILogger<MeshcapadeScraper> logger,
        IOptions<ScrapingConfig> config,
        MeshcapadeDbContext dbContext)
    {
        _logger = logger;
        _config = config.Value;
        _dbContext = dbContext;
    }

    public async Task<bool> InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing Playwright...");
            _playwright = await Playwright.CreateAsync();
            
            var browserOptions = new BrowserTypeLaunchOptions
            {
                Headless = _config.HeadlessMode,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox", 
                    "--disable-dev-shm-usage",
                    "--disable-accelerated-2d-canvas",
                    "--no-first-run",
                    "--no-zygote",
                    "--disable-gpu",
                    "--disable-background-timer-throttling",
                    "--disable-backgrounding-occluded-windows",
                    "--disable-renderer-backgrounding",
                    "--disable-features=TranslateUI",
                    "--disable-ipc-flooding-protection",
                    "--disable-search-engine-choice-screen",
                    "--disable-web-security",
                    "--disable-extensions"
                }
            };

            _browser = await _playwright.Chromium.LaunchAsync(browserOptions);
            _logger.LogInformation("Browser initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize browser");
            return false;
        }
    }

    public async Task<bool> StartScrapingAsync()
    {
        if (_browser == null)
        {
            _logger.LogError("Browser not initialized");
            return false;
        }

        try
        {
            var context = await CreateStealthContextAsync();
            var page = await context.NewPageAsync();

            // Apply stealth techniques
            await ApplyStealthTechniquesAsync(page);

            // Navigate to vault page first
            _logger.LogInformation("Navigating to vault page...");
            var response = await page.GotoAsync(_config.VaultUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = _config.PageLoadTimeout
            });

            if (response?.Status != 200)
            {
                _logger.LogWarning($"Vault page returned status: {response?.Status}");
            }

            // Wait for page to load completely
            await Task.Delay(5000);

            // Take screenshot for debugging
            if (_config.SaveScreenshots)
            {
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = Path.Combine(_config.ScreenshotPath, "vault_initial.png")
                });
            }

            // Look for editor button/link
            _logger.LogInformation("Looking for editor access...");
            var editorFound = await FindAndClickEditorAsync(page);

            if (!editorFound)
            {
                _logger.LogWarning("Editor not found, trying direct navigation");
                await page.GotoAsync(_config.EditorUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = _config.PageLoadTimeout
                });
            }

            // Wait for editor to load
            await Task.Delay(10000);

            // Check if we have measurement controls
            var measurementControls = await FindMeasurementControlsAsync(page);
            
            if (measurementControls)
            {
                _logger.LogInformation("Measurement controls found! Starting data collection...");
                await CollectMeasurementDataAsync(page);
            }
            else
            {
                _logger.LogError("Measurement controls not found");
                
                // Save page content for analysis
                var content = await page.ContentAsync();
                await File.WriteAllTextAsync("editor_debug.html", content);
                
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during scraping");
            return false;
        }
    }

    private async Task<IBrowserContext> CreateStealthContextAsync()
    {
        var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Locale = "en-US",
            TimezoneId = "America/New_York",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                ["Accept-Language"] = "en-US,en;q=0.5",
                ["Accept-Encoding"] = "gzip, deflate, br",
                ["DNT"] = "1",
                ["Connection"] = "keep-alive",
                ["Upgrade-Insecure-Requests"] = "1"
            }
        });

        return context;
    }

    private async Task ApplyStealthTechniquesAsync(IPage page)
    {
        // Mask webdriver properties
        await page.AddInitScriptAsync(@"
            Object.defineProperty(navigator, 'webdriver', {
                get: () => undefined,
            });
            
            Object.defineProperty(navigator, 'plugins', {
                get: () => [1, 2, 3, 4, 5],
            });
            
            Object.defineProperty(navigator, 'languages', {
                get: () => ['en-US', 'en'],
            });
            
            window.chrome = {
                runtime: {}
            };
            
            Object.defineProperty(navigator, 'permissions', {
                get: () => ({
                    query: () => Promise.resolve({ state: 'granted' })
                })
            });
        ");

        // Human-like behavior
        await page.Mouse.MoveAsync(100, 100);
        await Task.Delay(500);
        await page.Mouse.MoveAsync(200, 200);
    }

    private async Task<bool> FindAndClickEditorAsync(IPage page)
    {
        var editorSelectors = new[]
        {
            "text=Editor",
            "text=editor", 
            "a[href*='editor']",
            "button:has-text('Editor')",
            "[data-testid*='editor']",
            ".editor-button",
            "#editor-link"
        };

        foreach (var selector in editorSelectors)
        {
            try
            {
                var element = await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                {
                    Timeout = 3000
                });

                if (element != null)
                {
                    _logger.LogInformation($"Found editor element with selector: {selector}");
                    await element.ClickAsync();
                    await Task.Delay(2000);
                    return true;
                }
            }
            catch (TimeoutException)
            {
                // Continue to next selector
            }
        }

        return false;
    }

    private async Task<bool> FindMeasurementControlsAsync(IPage page)
    {
        var measurementSelectors = new[]
        {
            "input[type='number']",
            "input[type='range']", 
            "slider",
            "[data-testid*='height']",
            "[data-testid*='weight']",
            "text=Height",
            "text=Weight",
            "text=Chest",
            "text=Waist"
        };

        foreach (var selector in measurementSelectors)
        {
            try
            {
                var element = await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                {
                    Timeout = 5000
                });

                if (element != null)
                {
                    _logger.LogInformation($"Found measurement control: {selector}");
                    return true;
                }
            }
            catch (TimeoutException)
            {
                // Continue
            }
        }

        return false;
    }

    private async Task CollectMeasurementDataAsync(IPage page)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_config.MaxRetries, 
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        for (decimal height = _config.HeightStart; height <= _config.HeightEnd; height += _config.HeightIncrement)
        {
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation($"Setting height to {height} cm");
                    
                    // Set height value
                    await SetHeightValueAsync(page, height);
                    
                    // Wait for UI to update
                    await Task.Delay(_config.DelayBetweenRequests);
                    
                    // Extract measurement data
                    var measurements = await ExtractMeasurementDataAsync(page);
                    measurements.Height = height;
                    measurements.Timestamp = DateTime.UtcNow;

                    // Save to database
                    _dbContext.MeasurementData.Add(measurements);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Saved measurement data for height {height} cm");
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to collect data for height {height} cm");
                
                if (!_config.ContinueOnError)
                    throw;
            }
        }
    }

    private async Task SetHeightValueAsync(IPage page, decimal height)
    {
        var heightSelectors = new[]
        {
            "input[placeholder*='height' i]",
            "input[name*='height' i]",
            "input[id*='height' i]",
            "[data-testid*='height' i] input",
            "input[type='number']",
            "input[type='range']"
        };

        foreach (var selector in heightSelectors)
        {
            try
            {
                var element = await page.QuerySelectorAsync(selector);
                if (element != null)
                {
                    await element.FillAsync(height.ToString());
                    await element.PressAsync("Enter");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Failed to set height with selector {selector}: {ex.Message}");
            }
        }

        throw new InvalidOperationException("Could not find height input element");
    }

    private async Task<MeasurementData> ExtractMeasurementDataAsync(IPage page)
    {
        var data = new MeasurementData();

        // Try to extract measurements from various possible selectors
        data.Weight = await ExtractValueAsync(page, "weight", 0);
        data.Chest = await ExtractValueAsync(page, "chest", 0);
        data.Waist = await ExtractValueAsync(page, "waist", 0);
        data.Hip = await ExtractValueAsync(page, "hip", 0);
        data.Inseam = await ExtractValueAsync(page, "inseam", 0);

        return data;
    }

    private async Task<decimal> ExtractValueAsync(IPage page, string measurementType, decimal defaultValue)
    {
        var selectors = new[]
        {
            $"[data-testid*='{measurementType}' i] input[type='number']",
            $"input[name*='{measurementType}' i]",
            $"input[placeholder*='{measurementType}' i]",
            $"*:has-text('{measurementType}') ~ input",
            $"td:has-text('{measurementType}') ~ td",
            $".{measurementType}-value"
        };

        foreach (var selector in selectors)
        {
            try
            {
                var element = await page.QuerySelectorAsync(selector);
                if (element != null)
                {
                    var value = await element.GetAttributeAsync("value") ?? 
                               await element.TextContentAsync() ?? "";
                    
                    if (decimal.TryParse(value, out var result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Failed to extract {measurementType} with selector {selector}: {ex.Message}");
            }
        }

        _logger.LogWarning($"Could not extract {measurementType}, using default value {defaultValue}");
        return defaultValue;
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }
} 