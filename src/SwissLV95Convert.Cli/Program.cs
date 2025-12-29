using SwissLV95Convert.Core;
using SwissLV95Convert.Cli.Services;
using System;

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

        // Demander le path de fichier csv
        Console.Write("Please past the path of the CSV File : ");

        var inputPath = Console.ReadLine();
        var csvPath = inputPath.Trim().Trim('"');

        Console.WriteLine($"Your choice : {mode} and the path : {csvPath}");

        if (csvPath != null)
        {
           Console.WriteLine("Processing...");
           var csvService = new CsvService();
           var data = csvService.ReadCsv(csvPath, separator: ';', skipHeader: true);
           foreach (var row in data)
           {
               Console.WriteLine(string.Join(", ", row));
               break; // just to test reading
           }
        }

       }
}
