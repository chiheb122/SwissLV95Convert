# SwissLV95Convert

[![Status](https://img.shields.io/badge/status-under_development-orange?style=flat-square)](https://github.com/)
[![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-net9.0-512bd4?style=flat-square)](https://dotnet.microsoft.com/)
[![Issues](https://img.shields.io/badge/issues-welcome-brightgreen?style=flat-square)](https://github.com/)

Convert Swiss LV95 (CH1903+) coordinates to WGS84 from CSV files. Includes a CLI for batch use and a work-in-progress Avalonia GUI.

## Screenshot
![GUI Preview](img/darkScreen.png)

## Project structure
```
SwissLV95Convert/
├── SwissLV95Convert.sln
├── src/
│   ├── SwissLV95Convert.Core/
│   │   └── SwissLV95Convert.Core.csproj
│   └── SwissLV95Convert.Cli/
│       └── SwissLV95Convert.Cli.csproj
│   └── SwissLV95Convert.Gui/
│       └── SwissLV95Convert.Gui.csproj
├── README.md
├── LICENSE
└── .gitignore
```

## Features
- LV95 (MN95) ↔ WGS84 conversion
- CSV input/output
- CLI for scripting and batch processing
- Cross-platform GUI (Avalonia) — under development

## Quick start

1. Restore and build the solution:
```bash
dotnet restore
dotnet build
```

2. CLI example:
```bash
cd src/SwissLV95Convert.Cli
dotnet run -- <mode> <path-to-file.csv>
# mode: 1 = LV95 → WGS84, 2 = WGS84 → LV95
```

3. GUI (development):
- Build the Gui project:
```bash
cd src/SwissLV95Convert.Gui
dotnet build
dotnet run
```
- For Avalonia Preview in VS Code: build the solution first (`dotnet build`), then open the preview.

## Contributing
Issues and PRs welcome. Please follow standard GitHub workflow.

## License
MIT — see LICENSE for details.