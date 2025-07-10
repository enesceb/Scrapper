# Meshcapade Data Scraper

## Overview
This is a professional data scraping solution for extracting measurement data from the Meshcapade platform (me.meshcapade.com). The scraper is designed to collect anthropometric measurements from the editor page and systematically store them in a SQLite database.

## Features

### Core Functionality
- **Automated Data Collection**: Systematically increments height values from 150cm to 200cm
- **Real-time Measurement Capture**: Captures all body measurements as the system calculates them
- **Database Storage**: Stores all collected data in a SQLite database for analysis
- **Stealth Technology**: Uses advanced browser automation techniques to bypass bot protection

### Technical Specifications
- **Platform**: .NET 8.0 / C#
- **Browser Automation**: Microsoft Playwright
- **Database**: SQLite with Entity Framework Core
- **Data Points**: 51 measurement sets (150cm-200cm height range)
- **Measurements Captured**: Height, Weight, Chest, Waist, Hip, Inseam, and additional body measurements

## Architecture

### Components
1. **UltraStealthManualScraper**: Main scraping engine with anti-bot protection
2. **MeasurementData**: Data model for storing anthropometric measurements
3. **MeshcapadeDbContext**: Database context for data persistence
4. **ScrapingConfig**: Configuration management

### Data Model
```csharp
public class MeasurementData
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal Chest { get; set; }
    public decimal Waist { get; set; }
    public decimal Hip { get; set; }
    public decimal Inseam { get; set; }
    public string Notes { get; set; }
}
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Modern web browser (Chrome recommended)
- Internet connection

### Installation
1. Clone or download the project
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

### Usage
1. Run the application:
   ```bash
   dotnet run
   ```
2. Follow the on-screen instructions for manual assistance
3. Open an incognito browser tab when prompted
4. Navigate to https://me.meshcapade.com/editor
5. Wait for the page to load completely
6. Press ENTER in the console to start automated data collection

## Manual Assistance Process

Due to the website's bot protection mechanisms, the scraper requires human assistance to establish the initial connection:

1. **Wait Phase**: Allow 10 seconds for bot detection to settle
2. **Browser Setup**: Open a new incognito tab
3. **Navigation**: Navigate to the Meshcapade editor page
4. **Verification**: Ensure measurement controls are visible
5. **Trigger**: Press ENTER to start automated collection

## Data Collection Process

Once initiated, the scraper will:
1. Focus on the height input field
2. Set height value incrementally (150cm → 200cm)
3. Trigger system calculations
4. Capture all measurement values
5. Store data in SQLite database
6. Log progress and results

## Database Access

The collected data is stored in `meshcapade_data.db`. You can access it using:

### SQLite Command Line
```bash
sqlite3 meshcapade_data.db
.tables
SELECT * FROM MeasurementData LIMIT 10;
```

### View Data Summary
```sql
SELECT Height, Weight, Chest, Waist, Hip 
FROM MeasurementData 
ORDER BY Height;
```

## Output Files

The scraper generates several output files:
- `meshcapade_data.db`: SQLite database with all measurement data
- `editor_results.html`: Captured HTML content from the editor page
- `editor_results.png`: Screenshot of the editor page
- `logs/`: Application logs (if enabled)

## Configuration

Configuration is managed through `appsettings.json`:
```json
{
  "ScrapingConfig": {
    "UserAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
    "WaitTimeMs": 2000,
    "MaxRetries": 3
  }
}
```

## Troubleshooting

### Common Issues
- **Bot Protection**: If the site detects automation, try waiting longer between actions
- **Page Loading**: Ensure the page is fully loaded before starting collection
- **Database Access**: Check file permissions if database operations fail

### Support
For technical support or questions, refer to the application logs or contact the development team.

## Legal Notice

This software is for authorized use only. Ensure compliance with the target website's terms of service and applicable laws regarding data collection and web scraping.

## License

This project is proprietary software developed for specific client requirements. 