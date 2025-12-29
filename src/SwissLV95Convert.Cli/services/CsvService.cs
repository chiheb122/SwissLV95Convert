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
    }

    public void WriteCsv(string path, IEnumerable<string> data)
    {
        // Implementation for writing CSV
    }
}