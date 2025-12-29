using System;

namespace SwissLV95Convert.Core;

/// <summary>
/// Convert GPS (WGS84) to Swiss (LV95 / MN95) coordinates - and vice versa.
/// Ported from Antistatique\Swisstopo\SwisstopoConverter (PHP).
/// </summary>
public static class SwisstopoConverter
{
    // -------------------------
    // Public API (MN95 <-> WGS)
    // -------------------------

    /// <summary>
    /// Convert Swiss (MN95 / LV95) East+North (meters) to WGS84 (lat, lon) in degrees.
    /// </summary>
    public static (double Lat, double Lon) FromMN95ToWgs(double east, double north)
        => (FromMN95ToWgsLatitude(east, north), FromMN95ToWgsLongitude(east, north));

    /// <summary>
    /// Convert WGS84 (lat, lon) in degrees to Swiss (MN95 / LV95) East+North (meters).
    /// </summary>
    public static (double East, double North) FromWgsToMN95(double lat, double lon)
        => (FromWgsToMN95East(lat, lon), FromWgsToMN95North(lat, lon));

    // -------------------------
    // MN95 -> WGS
    // -------------------------

    public static double FromMN95ToWgsLatitude(double east, double north)
    {
        // Convert MN95 to auxiliary values (Bern = 0/0), unit: 1000 km
        double yAux = (east - 2_600_000.0) / 1_000_000.0;
        double xAux = (north - 1_200_000.0) / 1_000_000.0;

        // Latitude in unit 10000"
        double lat =
            16.9023892 +
            3.238272 * xAux -
            0.270978 * Math.Pow(yAux, 2) -
            0.002528 * Math.Pow(xAux, 2) -
            0.0447 * Math.Pow(yAux, 2) * xAux -
            0.0140 * Math.Pow(xAux, 3);

        // Convert 10000" -> degrees (same as *100/3600)
        return lat * 100.0 / 36.0;
    }

    private static double FromMN95ToWgsLongitude(double east, double north)
    {
        double yAux = (east - 2_600_000.0) / 1_000_000.0;
        double xAux = (north - 1_200_000.0) / 1_000_000.0;

        double lon =
            2.6779094 +
            4.728982 * yAux +
            0.791484 * yAux * xAux +
            0.1306 * yAux * Math.Pow(xAux, 2) -
            0.0436 * Math.Pow(yAux, 3);

        return lon * 100.0 / 36.0;
    }

    // -------------------------
    // WGS -> MN95
    // -------------------------

    private static double FromWgsToMN95North(double lat, double lon)
    {
        // Decimal degrees -> sexagesimal -> seconds of arc
        double latSex = DegToSex(lat);
        double lonSex = DegToSex(lon);

        double phi = DegToSec(latSex);
        double lambda = DegToSec(lonSex);

        // Auxiliary values (relative to Bern), unit [10000"]
        double phiAux = (phi - 169028.66) / 10000.0;
        double lambdaAux = (lambda - 26782.5) / 10000.0;

        // North (MN95)
        return 1_200_147.07
               + 308_807.95 * phiAux
               + 3_745.25 * Math.Pow(lambdaAux, 2)
               + 76.63 * Math.Pow(phiAux, 2)
               - 194.56 * Math.Pow(lambdaAux, 2) * phiAux
               + 119.79 * Math.Pow(phiAux, 3);
    }

    private static double FromWgsToMN95East(double lat, double lon)
    {
        double latSex = DegToSex(lat);
        double lonSex = DegToSex(lon);

        double phi = DegToSec(latSex);
        double lambda = DegToSec(lonSex);

        double phiAux = (phi - 169028.66) / 10000.0;
        double lambdaAux = (lambda - 26782.5) / 10000.0;

        // East (MN95)
        return 2_600_072.37
               + 211_455.93 * lambdaAux
               - 10_938.51 * lambdaAux * phiAux
               - 0.36 * lambdaAux * Math.Pow(phiAux, 2)
               - 44.54 * Math.Pow(lambdaAux, 3);
    }

    // -------------------------
    // Helpers (same behavior as PHP)
    // -------------------------

    /// <summary>
    /// Convert Decimal Degrees to Sexagesimal Degrees (dd.mmss).
    /// Example: 46.95108 -> 46.5703...
    /// </summary>
    private static double DegToSex(double angle)
    {
        int deg = (int)angle;
        int min = (int)((angle - deg) * 60.0);
        double sec = (((angle - deg) * 60.0) - min) * 60.0;

        return deg + min / 100.0 + sec / 10000.0;
    }

    /// <summary>
    /// Convert Sexagesimal Degrees (dd.mmss) to Seconds of Arc.
    /// </summary>
    private static double DegToSec(double angle)
    {
        int deg = (int)angle;
        int min = (int)((angle - deg) * 100.0);
        double sec = (((angle - deg) * 100.0) - min) * 100.0;

        return sec + min * 60.0 + deg * 3600.0;
    }
}
