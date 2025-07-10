namespace MeshcapadeDataScraper.Configuration;

public class ScrapingConfig
{
    public string BaseUrl { get; set; } = "https://me.meshcapade.com";
    public string VaultUrl { get; set; } = "https://me.meshcapade.com/vault";
    public string EditorUrl { get; set; } = "https://me.meshcapade.com/editor";
    
    public int DelayBetweenRequests { get; set; } = 2000; // milliseconds
    public int PageLoadTimeout { get; set; } = 30000; // milliseconds
    public int ElementTimeout { get; set; } = 10000; // milliseconds
    
    public decimal HeightStart { get; set; } = 150.0m; // cm
    public decimal HeightEnd { get; set; } = 200.0m; // cm  
    public decimal HeightIncrement { get; set; } = 1.0m; // cm
    
    public bool HeadlessMode { get; set; } = true;
    public bool SaveScreenshots { get; set; } = false;
    public string ScreenshotPath { get; set; } = "screenshots";
    
    public int MaxRetries { get; set; } = 3;
    public bool ContinueOnError { get; set; } = true;
    
    public string DatabasePath { get; set; } = "meshcapade_data.db";
} 