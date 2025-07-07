# FurnitureImporter

A tool for automating the import of furniture data into your Habbo emulator database, using furnidata to fill some gaps. Designed for **developers and retro hotel owners** who want to quickly add new furniture to their Kepler server.

![](https://i.imgur.com/EW6Kmn2.png)

---

## Features
- Fetches live furnidata XML from Habbo’s official endpoint  
- Processes furniture files you place in the `input` folder and uses the file name to get the furniture name.
- Generates and inserts item definitions and catalogue entries into your database  
- Handles variants, aliases, and different furniture types (e.g., different coloured chairs, beds, wall items, or branded advertisements vs non-branded of the same variant)  
- Built-in fancy console boot screen for some extra flair

---

## Requirements
- [.NET 8 or later](https://dotnet.microsoft.com/download)
- A MySQL database (configured via `appsettings.json`)
- The following folders:
  - `input/` → drop your furniture ``.swf`` or ``.cct`` files here
  - `output/` → automatically generated furniture data will appear here

---

## Configuration

Before running, set up your `appsettings.json` file with your database connection. Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=kepler;user=root;password=yourpassword;"
  }
}
```

Place appsettings.json in the same directory as your compiled executable.
