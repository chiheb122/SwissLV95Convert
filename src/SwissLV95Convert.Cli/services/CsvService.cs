using System.Data;
using System.Linq;
using SwissLV95Convert.Core;

namespace SwissLV95Convert.Cli.Services;


class CsvService
{
    public IEnumerable<string[]> ReadCsv(string path, char separator = ',',bool skipHeader = true)
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

// Column indexes (0-based) for LV95 coordinates in your dataset
    public void ConvertAndAddToCsv(string outputPath, IEnumerable<string[]> data, int eastingIndex, int northingIndex)
    {
        // Ensure output directory exists
        var outDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outDir))
            Directory.CreateDirectory(outDir);



        bool isFirstRow = true;

        // Write to CSV
        using var writer = new StreamWriter(outputPath);
        foreach (var row in data)
        {
           // add to the header the lon and lat atttribute
           if (isFirstRow)
            {
                // Append header columns by creating a NEW sequence, then write it
                var header = row.Concat(new[] { "Latitude", "Longitude" });
                writer.WriteLine(string.Join(";", header));
                isFirstRow = false;
                continue;
            }
            // if the row is not the header convert coord and add it
           var (latitude, longitude) = SwisstopoConverter.FromMN95ToWgs(
                double.Parse(row[eastingIndex]), 
                double.Parse(row[northingIndex])
           );
            // Write the original row with new lat/lon columns
            var newRow = row.Concat(new[] { latitude.ToString("F6"), longitude.ToString("F6") });
            writer.WriteLine(string.Join(";", newRow));
        }
        // Close the writer 
        writer.Flush();
        writer.Close();

 
    }


    


}