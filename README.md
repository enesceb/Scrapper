# Meshcapade Data Scraper (.NET)

A modern .NET scraper for extracting body measurement data from Meshcapade using Playwright browser automation.

## 🚀 Features

- **Advanced Anti-Bot Detection**: Uses Playwright with stealth techniques
- **Robust Data Collection**: Incremental height variations with measurement tracking
- **SQLite Database**: Persistent storage of measurement data
- **Comprehensive Logging**: Detailed logs with Serilog
- **Error Handling**: Retry policies and graceful error handling
- **Configurable**: Easy configuration through `ScrapingConfig`

## 📋 Requirements

- .NET 8.0 or later
- Playwright browsers (automatically installed)

## 🛠️ Installation

1. Clone the repository
2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Install Playwright browsers:
   ```bash
   dotnet run
   ```

## ⚙️ Configuration

The scraper is configured through `ScrapingConfig` in `Program.cs`:

```csharp
config.HeightStart = 150.0m;       // Starting height (cm)
config.HeightEnd = 200.0m;         // Ending height (cm)
config.HeightIncrement = 5.0m;     // Height increment (cm)
config.HeadlessMode = false;       // Set to true for headless mode
config.SaveScreenshots = true;     // Save screenshots for debugging
```

## 🎯 Usage

Run the scraper:
```bash
dotnet run
```

The scraper will:
1. Navigate to Meshcapade vault page
2. Look for editor access
3. Find measurement controls
4. Collect data by varying height values
5. Store results in SQLite database

## 📊 Database Schema

The measurements are stored in the `MeasurementData` table:

| Column    | Type     | Description        |
|-----------|----------|--------------------|
| Id        | int      | Primary key        |
| Timestamp | DateTime | Collection time    |
| Height    | decimal  | Height (cm)        |
| Weight    | decimal  | Weight (kg)        |
| Chest     | decimal  | Chest (cm)         |
| Waist     | decimal  | Waist (cm)         |
| Hip       | decimal  | Hip (cm)           |
| Inseam    | decimal  | Inseam (cm)        |
| Notes     | string   | Additional notes   |

## 🔧 Architecture

- **Services/MeshcapadeScraper.cs**: Main scraping logic
- **Models/MeasurementData.cs**: Data model
- **Data/MeshcapadeDbContext.cs**: Database context
- **Configuration/ScrapingConfig.cs**: Configuration settings

## 🎭 Anti-Bot Techniques

- Navigator property masking
- Plugin simulation
- Human-like mouse movements
- Realistic delays
- Advanced browser arguments
- Stealth context creation

## 📝 Logging

Logs are written to:
- Console (for real-time monitoring)
- Files in `logs/` directory (daily rotation)

## 🐛 Debugging

- Screenshots are saved in `screenshots/` folder
- HTML content is saved for analysis
- Detailed logs include selector attempts and errors

## 🔄 Error Handling

- Automatic retry with exponential backoff
- Graceful handling of missing elements
- Configurable continue-on-error behavior
- Comprehensive error logging

## 📁 Project Structure

```
MeshcapadeDataScraper/
├── Configuration/
│   └── ScrapingConfig.cs
├── Data/
│   └── MeshcapadeDbContext.cs
├── Models/
│   └── MeasurementData.cs
├── Services/
│   └── MeshcapadeScraper.cs
├── logs/
├── screenshots/
└── Program.cs
```

## 🎯 Next Steps

1. Test the scraper with visible mode
2. Analyze collected data
3. Optimize scraping performance
4. Add data export functionality
5. Implement data visualization 