using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MeshcapadeDataScraper.Services;
using MeshcapadeDataScraper.Data;
using MeshcapadeDataScraper.Configuration;

namespace MeshcapadeDataScraper;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/scraper-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = Host.CreateDefaultBuilder(args);

        // Clear default logging and use only Serilog
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog();
        });

        builder.ConfigureServices((context, services) =>
        {
            // Configuration
            services.Configure<ScrapingConfig>(context.Configuration.GetSection("Scraping"));
            
            // Database
            services.AddDbContext<MeshcapadeDbContext>(options =>
                options.UseSqlite("Data Source=meshcapade_data.db"));
            
            // Services - ULTRA STEALTH MODE
            services.AddTransient<UltraStealthManualScraper>();
        });

        var host = builder.Build();

        // Database migration
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MeshcapadeDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        // Launch the ULTRA STEALTH SYSTEM
        using (var scope = host.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var ultraStealthScraper = scope.ServiceProvider.GetRequiredService<UltraStealthManualScraper>();

            logger.LogInformation("=== MESHCAPADE DATA SCRAPER ONLINE - EDITOR TARGET ===");
            logger.LogInformation("Mode: Incognito + Stealth tactics");
            logger.LogInformation("Approach: Slow movements + Casual browsing first");
            logger.LogInformation("Target: Editor page measurement controls");
            logger.LogInformation("Assistance: Human manual assistance required");
            
            try
            {
                // Initialize ultra stealth mode
                var initSuccess = await ultraStealthScraper.InitializeAsync();
                
                if (initSuccess)
                {
                    logger.LogInformation("💥💥💥 ULTRA STEALTH BROWSER READY 💥💥💥");
                    logger.LogInformation("🎯 STARTING CASUAL BROWSING THEN MANUAL LOADING");
                    
                    // Wait for ultra stealth manual loading
                    var stealthSuccess = await ultraStealthScraper.WaitForUltraStealthManualLoadingAsync();
                    
                    if (stealthSuccess)
                    {
                        logger.LogInformation("🎉🎉🎉 ULTRA STEALTH SUCCESS! BOT PROTECTION BYPASSED! 🎉🎉🎉");
                        logger.LogInformation("📸 Check ultra stealth screenshots and content files!");
                        logger.LogInformation("🎯 MEASUREMENT CONTROLS ANALYSIS COMPLETE!");
                    }
                    else
                    {
                        logger.LogError("💀💀💀 ULTRA STEALTH FAILED - SITE PROTECTION TOO STRONG 💀💀💀");
                        logger.LogInformation("💡 SUGGESTION: Site might require human verification or different approach");
                    }
                }
                else
                {
                    logger.LogError("💥 ULTRA STEALTH INITIALIZATION FAILED - ABORTING");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "🔥 ULTRA STEALTH SYSTEM FAILED WITH CRITICAL ERROR");
            }
            finally
            {
                // Clean up
                await ultraStealthScraper.DisposeAsync();
                logger.LogInformation("🧹 ULTRA STEALTH SYSTEM DISPOSED");
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
