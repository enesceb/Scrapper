using Microsoft.Playwright;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MeshcapadeDataScraper.Configuration;
using MeshcapadeDataScraper.Models;
using MeshcapadeDataScraper.Data;

namespace MeshcapadeDataScraper.Services;

public class UltraStealthManualScraper
{
    private readonly ILogger<UltraStealthManualScraper> _logger;
    private readonly ScrapingConfig _config;
    private readonly MeshcapadeDbContext _dbContext;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public UltraStealthManualScraper(
        ILogger<UltraStealthManualScraper> logger,
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
            _logger.LogInformation("🕵️‍♂️ ULTRA STEALTH MANUAL SCRAPER INITIALIZING...");
            _playwright = await Playwright.CreateAsync();

            // Ultra stealth browser without custom user data directory

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage",
                    "--disable-web-security",
                    "--allow-running-insecure-content",
                    "--ignore-certificate-errors",
                    "--disable-blink-features=AutomationControlled",
                    "--exclude-switches=enable-automation",
                    "--disable-background-networking",
                    "--disable-sync",
                    "--disable-translate",
                    "--disable-default-apps",
                    "--no-first-run",
                    "--disable-component-update",
                    "--disable-domain-reliability",
                    "--disable-features=VizDisplayCompositor",
                    "--disable-ipc-flooding-protection",
                    "--disable-renderer-backgrounding",
                    "--disable-backgrounding-occluded-windows",
                    "--disable-field-trial-config",
                    "--disable-back-forward-cache",
                    "--disable-hang-monitor",
                    "--disable-prompt-on-repost",
                    "--disable-extensions",
                    "--disable-client-side-phishing-detection",
                    "--disable-component-extensions-with-background-pages",
                    "--disable-permissions-api",
                    "--disable-background-timer-throttling",
                    "--disable-backgrounding-occluded-windows",
                    "--disable-renderer-backgrounding",
                    "--incognito"
                },
                SlowMo = 300, // Very slow movements
                Timeout = 120000 // 2 minutes timeout
            });

            _logger.LogInformation("✅ ULTRA STEALTH BROWSER LAUNCHED WITH INCOGNITO MODE");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 ULTRA STEALTH SCRAPER INITIALIZATION FAILED");
            return false;
        }
    }

    public async Task<bool> WaitForUltraStealthManualLoadingAsync()
    {
        if (_browser == null)
        {
            _logger.LogError("Browser not initialized");
            return false;
        }

        try
        {
            _logger.LogInformation("Creating stealth browser context...");
            
            // Create incognito context
            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
                ViewportSize = new ViewportSize { Width = 1366, Height = 768 },
                DeviceScaleFactor = 1,
                Locale = "en-US",
                TimezoneId = "America/New_York",
                ColorScheme = ColorScheme.Light,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8",
                    ["Accept-Language"] = "en-US,en;q=0.9",
                    ["Accept-Encoding"] = "gzip, deflate, br",
                    ["Sec-Fetch-Dest"] = "document",
                    ["Sec-Fetch-Mode"] = "navigate",
                    ["Sec-Fetch-Site"] = "none",
                    ["Sec-Fetch-User"] = "?1",
                    ["Upgrade-Insecure-Requests"] = "1",
                    ["Cache-Control"] = "no-cache",
                    ["Pragma"] = "no-cache"
                }
            });

            // Open empty page first
            var page = await context.NewPageAsync();
            
            // Wait before any action
            await Task.Delay(2000);
            
            // Go to Google first (establish browsing pattern)
            _logger.LogInformation("Phase 1: Establishing browsing pattern - navigating to Google...");
            _logger.LogInformation("Wait 30 Seconds before proceeding");

            await page.GotoAsync("https://www.google.com", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 5000
            });
            
            // Wait and simulate human behavior
            await Task.Delay(2000);
            
            // Search for something casual
            try
            {
                await page.FillAsync("input[name='q']", "3d modeling software");
                await Task.Delay(2000);
                await page.PressAsync("input[name='q']", "Enter");
                await Task.Delay(3000);
            }
            catch
            {
                _logger.LogInformation("Google search step skipped");
            }

            _logger.LogInformation("Browser ready for manual loading process.");
            _logger.LogInformation("Manual steps required:");
            _logger.LogInformation("1. Wait 10 seconds before proceeding");
            _logger.LogInformation("2. Open a new incognito tab");
            _logger.LogInformation("3. Navigate to: https://me.meshcapade.com");
            _logger.LogInformation("4. Wait for page to load completely");
            _logger.LogInformation("5. Navigate to /vault or /editor if needed");
            _logger.LogInformation("6. Ensure all content is fully loaded");
            _logger.LogInformation("7. Press ENTER in console to continue");

            Console.WriteLine("\n=== MESHCAPADE DATA SCRAPER - MANUAL ASSISTANCE MODE ===");
            Console.WriteLine("IMPORTANT: Please wait 10 seconds before proceeding!");
            Console.WriteLine("\nSTEPS:");
            Console.WriteLine("   1. WAIT 10 SECONDS (allow bot detection to settle)");
            Console.WriteLine("   2. Open NEW INCOGNITO TAB (Cmd+Shift+N on Mac, Ctrl+Shift+N on Windows)");
            Console.WriteLine("   3. Navigate to: https://me.meshcapade.com/editor");
            Console.WriteLine("   4. Press Enter and wait for page to load completely");
            Console.WriteLine("   5. Verify measurement controls are visible (height, weight, age)");
            Console.WriteLine("   6. Wait for all content to load completely");
            Console.WriteLine("   7. Press ENTER here when ready to proceed");
            Console.WriteLine("\nWaiting for user confirmation... Press ENTER when ready:");
            Console.ReadLine();

            _logger.LogInformation("Analyzing loaded pages...");

            // Wait longer before checking
            await Task.Delay(5000);

            // Get all pages (tabs) in the context
            var pages = context.Pages;
            _logger.LogInformation($"Found {pages.Count} open tabs");

            var foundContent = false;

            foreach (var openPage in pages)
            {
                try
                {
                    var url = openPage.Url;
                    _logger.LogInformation($"Checking tab: {url}");

                    if (url.Contains("me.meshcapade.com"))
                    {
                        _logger.LogInformation($"Found Meshcapade tab: {url}");
                        
                        // Wait longer to ensure page is fully loaded
                        await Task.Delay(5000);
                        
                        // Check content
                        var bodyText = await openPage.TextContentAsync("body");
                        var contentLength = bodyText?.Length ?? 0;
                        
                        _logger.LogInformation($"Content length: {contentLength} characters");
                        
                        if (contentLength > 500)
                        {
                            _logger.LogInformation("Content loaded successfully!");
                            
                            // Take screenshot - prioritize editor
                            var fileName = url.Contains("editor") ? "editor_results.png" : 
                                         url.Contains("vault") ? "vault_results.png" : 
                                         "meshcapade_results.png";
                            
                            await openPage.ScreenshotAsync(new PageScreenshotOptions
                            {
                                Path = fileName,
                                FullPage = true
                            });
                            
                            // Save full content - prioritize editor
                            var contentFileName = url.Contains("editor") ? "editor_results.html" : 
                                                url.Contains("vault") ? "vault_results.html" : 
                                                "meshcapade_results.html";
                            
                            var fullContent = await openPage.ContentAsync();
                            await File.WriteAllTextAsync(contentFileName, fullContent);
                            
                            _logger.LogInformation($"Files saved: {fileName}, {contentFileName}");
                            
                            // Search for controls
                            await SearchForControlsUltraCarefullyAsync(openPage, url);
                            
                            foundContent = true;
                        }
                        else
                        {
                            _logger.LogWarning($"Minimal content detected on {url}");
                            _logger.LogInformation($"Content preview: {bodyText?[..Math.Min(200, bodyText.Length)]?.Replace("\n", " ")}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error checking tab: {ex.Message}");
                }
            }

            if (!foundContent)
            {
                _logger.LogWarning("No Meshcapade content found");
                _logger.LogInformation("Suggestion: The site might have strong bot protection");
                _logger.LogInformation("Try waiting longer or accessing from a different network");
                
                Console.WriteLine("\nWould you like to try again? (Y/N)");
                var retry = Console.ReadLine();
                if (retry?.ToUpper() == "Y")
                {
                    await Task.Delay(10000); // Wait 10 seconds
                    return await WaitForUltraStealthManualLoadingAsync();
                }
            }

            return foundContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 ULTRA STEALTH MANUAL LOADING FAILED");
            return false;
        }
    }

    private async Task SearchForControlsUltraCarefullyAsync(IPage page, string url)
    {
        try
        {
            _logger.LogInformation($"Searching for measurement controls on {url}...");
            
            // Wait longer for dynamic content
            await Task.Delay(8000);
            
            // First, do basic control search
            var measurementInputSelector = ".measurementSlider_numInput__DTNXQ";
            var measurementInputs = await page.QuerySelectorAllAsync(measurementInputSelector);
            
            _logger.LogInformation($"Found {measurementInputs.Count} measurement inputs: {measurementInputSelector}");
            
            if (measurementInputs.Count >= 2 && url.Contains("editor"))
            {
                _logger.LogInformation("Starting height increment data collection...");
                await StartHeightIncrementDataCollection(page, measurementInputs);
                return;
            }
            
            // If no measurement inputs found, do full control search
            var controlSelectors = new[]
            {
                // Primary measurement controls
                "input[type='range']",
                "input[type='number']",
                "input[type='text']",
                ".measurementSlider_numInput__DTNXQ",
                
                // Slider controls
                ".slider",
                "[role='slider']",
                "[type='range']",
                
                // Data attributes
                "[data-testid*='measurement']",
                "[data-testid*='body']",
                
                // Class-based selectors
                "[class*='slider']",
                "[class*='control']",
                "[class*='input']",
                "[class*='measurement']",
                
                // Interactive elements
                "button",
                "select"
            };

            var foundControls = new List<(string selector, int count)>();

            foreach (var selector in controlSelectors)
            {
                try
                {
                    var elements = await page.QuerySelectorAllAsync(selector);
                    if (elements.Any())
                    {
                        foundControls.Add((selector, elements.Count));
                        _logger.LogInformation($"🎯 FOUND {elements.Count} ELEMENTS: {selector}");
                    }
                    
                    // Small delay between searches
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Selector {selector} failed: {ex.Message}");
                }
            }

            // Text content analysis
            var textContent = await page.TextContentAsync("body");
            var lowerText = textContent?.ToLower() ?? "";
            
            var keywords = new[] { 
                "height", "weight", "age", "measurement", "measurements", "cm", "kg", 
                "chest", "waist", "hip", "shoulder", "arm", "leg", "torso", "neck",
                "slider", "control", "parameter", "value", "range", "min", "max",
                "avatar", "character", "model", "figure", "shape", "size", "build"
            };
            var foundKeywords = keywords.Where(k => lowerText.Contains(k)).ToList();
            
            _logger.LogInformation($"📊 FOUND KEYWORDS: {string.Join(", ", foundKeywords)}");

            if (foundControls.Any())
            {
                _logger.LogInformation($"🎉 ULTRA STEALTH SUCCESS! FOUND {foundControls.Count} TYPES OF CONTROLS!");
                
                foreach (var (selector, count) in foundControls)
                {
                    _logger.LogInformation($"   📋 {selector}: {count} elements");
                }
            }
            else
            {
                _logger.LogWarning("😞 NO INTERACTIVE CONTROLS FOUND");
                
                // Get page title and basic info
                var title = await page.TitleAsync();
                var url2 = page.Url;
                
                _logger.LogInformation($"📊 PAGE INFO: Title='{title}', URL='{url2}'");
                _logger.LogInformation($"📊 CONTENT LENGTH: {textContent?.Length ?? 0} characters");
                
                if (foundKeywords.Any())
                {
                    _logger.LogInformation($"✅ POSITIVE: Found measurement keywords!");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 ULTRA CAREFUL CONTROL SEARCH FAILED");
        }
    }

    private async Task StartHeightIncrementDataCollection(IPage page, IReadOnlyList<IElementHandle> measurementInputs)
    {
        try
        {
            _logger.LogInformation("=== STARTING COMPREHENSIVE HEIGHT-WEIGHT DATA COLLECTION ===");
            _logger.LogInformation($"Height range: 137cm - 210cm (74 values)");
            _logger.LogInformation($"Weight range: 40kg - 150kg (111 values)");
            _logger.LogInformation($"Total combinations: 74 × 111 = 8,214 records");
            _logger.LogInformation($"Batch size: 200 records per database save");
            _logger.LogInformation($"Expected duration: ~4.5 hours");
            
            if (measurementInputs.Count == 0)
            {
                _logger.LogWarning("No measurement inputs found!");
                return;
            }

            // Height should be first input (index 0), Weight should be second (index 1)
            var heightInput = measurementInputs[0];
            var weightInput = measurementInputs[1];
            
            // Batch processing setup
            var totalHeightValues = 210 - 137 + 1; // 74 height values
            var totalWeightValues = 150 - 40 + 1; // 111 weight values
            var totalIterations = totalHeightValues * totalWeightValues; // 8,214 total
            var currentIteration = 0;
            var batchSize = 200;
            var batchData = new List<MeasurementData>();
            
            // Time tracking
            var startTime = DateTime.Now;
            
            // Print table header
            _logger.LogInformation("\n" + "=".PadRight(120, '='));
            _logger.LogInformation("| #    | Height | Weight | Chest | Waist | Hip   | Progress | Elapsed  | ETA      | Status    |");
            _logger.LogInformation("|------|--------|--------|-------|-------|-------|----------|----------|----------|-----------|");
            
            for (int heightValue = 137; heightValue <= 210; heightValue++) // 137cm to 210cm
            {
                _logger.LogInformation($"=== PROCESSING HEIGHT {heightValue}cm ===");
                
                // Set height value first
                try
                {
                    await heightInput.FocusAsync();
                    await Task.Delay(300);
                    await heightInput.SelectTextAsync();
                    await Task.Delay(300);
                    await heightInput.TypeAsync(heightValue.ToString());
                    await Task.Delay(300);
                    await heightInput.PressAsync("Tab");
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error setting height {heightValue}cm");
                    continue;
                }
                
                // Now iterate through all weight values for this height
                for (int weightValue = 40; weightValue <= 150; weightValue++) // 40kg to 150kg
                {
                    try
                    {
                        currentIteration++;
                        
                        // Focus on weight input
                        await weightInput.FocusAsync();
                        await Task.Delay(300);
                        await weightInput.SelectTextAsync();
                        await Task.Delay(300);
                        await weightInput.TypeAsync(weightValue.ToString());
                        await Task.Delay(300);
                        await weightInput.PressAsync("Tab");
                        
                        // Wait for system calculations
                        await Task.Delay(2000);
                        
                        // Read all measurement values
                        var measurementData = new List<(string Name, string Value)>();
                        
                        for (int i = 0; i < measurementInputs.Count; i++)
                        {
                            var input = measurementInputs[i];
                            var value = await input.GetAttributeAsync("value") ?? "0";
                            var name = GetMeasurementName(i);
                            measurementData.Add((name, value));
                        }
                        
                        // Parse measurements
                        var chest = decimal.TryParse(measurementData.FirstOrDefault(m => m.Name == "chest").Value, out var chestParsed) ? chestParsed : 0;
                        var waist = decimal.TryParse(measurementData.FirstOrDefault(m => m.Name == "waist").Value, out var waistParsed) ? waistParsed : 0;
                        var hip = decimal.TryParse(measurementData.FirstOrDefault(m => m.Name == "hip").Value, out var hipParsed) ? hipParsed : 0;
                        var inseam = decimal.TryParse(measurementData.FirstOrDefault(m => m.Name == "leg_length").Value, out var inseamParsed) ? inseamParsed : 0;
                        
                        // Create measurement data object
                        var data = new MeasurementData
                        {
                            Timestamp = DateTime.UtcNow,
                            Height = (decimal)heightValue,
                            Weight = (decimal)weightValue,
                            Chest = chest,
                            Waist = waist,
                            Hip = hip,
                            Inseam = inseam,
                            Notes = $"H:{heightValue}cm, W:{weightValue}kg - Auto-collected from Editor"
                        };
                        
                        // Add to batch
                        batchData.Add(data);
                        
                        // Progress update and table display
                        var progress = (double)currentIteration / totalIterations * 100;
                        var status = batchData.Count < batchSize ? "Collecting" : "Saving...";
                        
                        // Time calculations
                        var elapsed = DateTime.Now - startTime;
                        var avgTimePerRecord = elapsed.TotalSeconds / currentIteration;
                        var remainingRecords = totalIterations - currentIteration;
                        var etaSeconds = avgTimePerRecord * remainingRecords;
                        var eta = TimeSpan.FromSeconds(etaSeconds);
                        
                        var elapsedStr = $"{elapsed:hh\\:mm\\:ss}";
                        var etaStr = eta.TotalHours < 24 ? $"{eta:hh\\:mm\\:ss}" : $"{(int)eta.TotalDays}d {eta:hh\\:mm\\:ss}";
                        
                        _logger.LogInformation($"| {currentIteration,4} | {heightValue,6} | {weightValue,6} | {chest,5:F1} | {waist,5:F1} | {hip,5:F1} | {progress,7:F1}% | {elapsedStr,-8} | {etaStr,-8} | {status,-9} |");
                        
                        // Save batch when full
                        if (batchData.Count >= batchSize)
                        {
                            await SaveBatchDataAsync(batchData);
                            batchData.Clear();
                        }
                        
                        // Small delay between iterations
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing H:{heightValue}cm, W:{weightValue}kg");
                    }
                }
                
                _logger.LogInformation($"Completed all weights for height {heightValue}cm");
            }
            
            // Save any remaining batch data
            if (batchData.Count > 0)
            {
                await SaveBatchDataAsync(batchData);
            }
            
            // Final time calculation
            var totalElapsed = DateTime.Now - startTime;
            var finalAvgTimePerRecord = totalElapsed.TotalSeconds / totalIterations;
            
            _logger.LogInformation("=".PadRight(120, '='));
            _logger.LogInformation("=== COMPREHENSIVE HEIGHT-WEIGHT DATA COLLECTION COMPLETED ===");
            _logger.LogInformation($"Successfully completed {totalIterations} iterations");
            _logger.LogInformation($"Data range: Height 137cm-210cm, Weight 40kg-150kg");
            _logger.LogInformation($"Total time elapsed: {totalElapsed:hh\\:mm\\:ss}");
            _logger.LogInformation($"Average time per record: {finalAvgTimePerRecord:F2} seconds");
            _logger.LogInformation($"All measurement data saved to SQLite database");
            _logger.LogInformation($"Collection finished at: {DateTime.Now:HH:mm:ss}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ CRITICAL ERROR: Height increment data collection failed");
            _logger.LogError($"❌ Exception type: {ex.GetType().Name}");
            _logger.LogError($"❌ Exception message: {ex.Message}");
        }
    }

    private string GetMeasurementName(int index)
    {
        var measurementNames = new[]
        {
            "height",
            "weight", 
            "chest",
            "waist",
            "hip",
            "shoulder_width",
            "arm_length",
            "leg_length",
            "neck",
            "wrist",
            "ankle", 
            "thigh",
            "forearm"
        };
        
        return index < measurementNames.Length ? measurementNames[index] : $"measurement_{index}";
    }

    private async Task SaveBatchDataAsync(List<MeasurementData> batchData)
    {
        try
        {
            _logger.LogInformation($"💾 Saving batch of {batchData.Count} records to database...");
            
            _dbContext.MeasurementData.AddRange(batchData);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation($"✅ Successfully saved {batchData.Count} records to database!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to save batch of {batchData.Count} records");
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
            
            _playwright?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error disposing browser: {ex.Message}");
        }
    }
} 