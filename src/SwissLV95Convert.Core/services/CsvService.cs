using System.Data;
using System.Linq;

namespace SwissLV95Convert.Core.Services;


public static class CsvService
{
    public static IEnumerable<string[]> ReadCsv(string path, char separator = ',',bool skipHeader = true)
    {
        // Nettoyage: l'utilisateur colle parfois '...' ou "..."
        path = path.Trim().Trim('"').Trim('\'');

        // IMPORTANT: on n'assemble rien si le chemin est absolu.
        // On utilise exactement ce que l'utilisateur a donné.
        if (!Path.IsPathRooted(path))
            path = Path.GetFullPath(path);

        if (!File.Exists(path))
            throw new FileNotFoundException($"CSV not found: {path}", path);
        // Implementation for reading CSV
        using var reader = new StreamReader(path);
        string? line;

        if (skipHeader) reader.ReadLine(); // skip header


        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            yield return line.Split(separator);
        }
        // Close the reader
        reader.Close();
    }



    /// <Summary>
    /// Converts LV95 coordinates in the specified columns to WGS84 and appends them as new columns.
    /// Column indexes (0-based) for LV95 coordinates in your dataset
    /// eastingIndex: index of the easting (X) coordinate column
    /// northingIndex: index of the northing (Y) coordinate column
    /// </Summary>
    public static void ConvertAndAddToCsv(string outputPath, IEnumerable<string[]> data, int eastingIndex, int northingIndex,string onlyLongLat = "n")
    {
        // clean the output path
        outputPath = outputPath.Trim().Trim('"').Trim('\'');
        // Ensure output directory exists and has correct permissions 
        EnsureWritableDirectory(outputPath);
        
        // Flag to track the first row (header)
        bool isFirstRow = true;

        // Write to CSV
        using var writer = new StreamWriter(outputPath);
        foreach (var row in data)
        {
           // add to the header the lon and lat atttribute
           if (isFirstRow && onlyLongLat.ToLower() != "y")
            {
                // Append header columns by creating a NEW sequence, then write it
                var header = row.Concat(new[] { "Latitude", "Longitude" });
                writer.WriteLine(string.Join(";", header));
                isFirstRow = false;
                continue;
            }else if (isFirstRow && onlyLongLat.ToLower() == "y"){
                // Append header columns by creating a NEW sequence, then write it
                var header = new[] { "Latitude", "Longitude" };
                writer.WriteLine(string.Join(";", header));
                isFirstRow = false;
                continue;
            }


            // if the row is not the header convert coord and add it
           var (latitude, longitude) = SwisstopoConverter.FromMN95ToWgs(
                double.Parse(row[eastingIndex]), 
                double.Parse(row[northingIndex])
           );

            // if only long/lat is chosen, write only these two columns

            if (onlyLongLat.ToLower() == "y")
            {
                var newRowOnlyLatLon = new[] { latitude.ToString("F6"), longitude.ToString("F6") };
                writer.WriteLine(string.Join(";", newRowOnlyLatLon));
                continue;
            }else{
            // Write the original row with new lat/lon columns
            var newRow = row.Concat(new[] { latitude.ToString("F6"), longitude.ToString("F6") });
            writer.WriteLine(string.Join(";", newRow));
            }
        }
        // Close the writer 
        writer.Flush();
        writer.Close();

 
    }


    /// <Summary>
    /// Ensures that the directory for the given file path exists and is writable.
    /// Throws an exception if the directory is not writable.
    /// </Summary>
    static void EnsureWritableDirectory(string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (string.IsNullOrWhiteSpace(dir))
            return;

        Directory.CreateDirectory(dir);

        try
        {
            var testFile = Path.Combine(dir, Path.GetRandomFileName());
            using (File.Create(testFile)) { }
            File.Delete(testFile);
        }
        catch (Exception ex) when (
            ex is UnauthorizedAccessException ||
            ex is IOException)
        {
            throw new IOException($"Directory is not writable: {dir}", ex);
        }
    }

    


}