using SwissLV95Convert.Core.Services;
using System;
using System.IO;
using System.Linq;

// Console.WriteLine("test Swiss LV95 <-> WGS for this address Route d'Hermance 261. ==> long 6.220180365 lat 46.270537055");
// var (lat2, lon2) = SwisstopoConverter.FromWgsToMN95(46.270537055, 6.220180365);
// Console.WriteLine($"{lat2}, {lon2}");
// var (e2, n2) = SwisstopoConverter.FromMN95ToWgs(2506071.293, 1125076.987);
// Console.WriteLine($"{e2}, {n2}");

class Program
{
    static void Main(string[] args)
    {
       Console.OutputEncoding = System.Text.Encoding.UTF8;

        // print all available options 
        Console.WriteLine("""
            SwissLV95Convert — Swiss LV95 (MN95) ↔ WGS84 Converter
            -----------------------------------------------------
            Choose a mode:
            1) LV95 → WGS84
            2) WGS84 → LV95
            0) Exit
            """);
       
       // Give the choice ligne and read it
        Console.Write("Mode (0/1/2): ");
        var mode = Console.ReadLine()?.Trim(); // read the number
        if (mode == "0")
        {
            Console.WriteLine("Exiting the program.");
            return;
        } else if (mode != "1" && mode != "2"){
            Console.WriteLine("Invalid mode selected. Please restart the program and choose a valid option.");
            return;
        }
        // Demander le path de fichier csv
        Console.Write("Please past the path of the CSV File : ");

        var inputPath = Console.ReadLine();
        // Trim both double and single quotes, then normalise to absolute path
        var csvPath = inputPath?.Trim().Trim('"').Trim('\'');
        if (csvPath != null)
            csvPath = Path.GetFullPath(csvPath);

        Console.WriteLine($"Would you like to add data with the existing or only long/lat? (y/n): ");
        var onlyLongLat = Console.ReadLine()?.Trim().ToLower() == "y" ? "y" : "n";


        if (csvPath != null)
        {
           Console.WriteLine("Processing...");
           // Materialiser pour pouvoir le relire et éviter d'épuiser l'énumérable
           var data = CsvService.ReadCsv(csvPath, separator: ';', skipHeader: false).ToList();

           var outputDir = Path.GetDirectoryName(csvPath) ?? Environment.CurrentDirectory;
           var outputPath = Path.GetFullPath(
               Path.Combine(
                   outputDir,
                   $"output_converted_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
               )
           );
            // depending on the mode call the right conversion
            if (mode == "2"){
               Console.WriteLine("WGS84 to LV95 conversion is not yet implemented.");
               return;
            }
            else if (mode == "1"){
               CsvService.ConvertAndAddToCsv(outputPath, data, 8, 9, onlyLongLat);
           }
           // inform the user of the output path
           Console.WriteLine($"Conversion completed. Output saved to: {outputPath}");
        }

       }
}
