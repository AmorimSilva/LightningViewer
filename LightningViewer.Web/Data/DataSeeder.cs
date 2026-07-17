using LightningViewer.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LightningViewer.Web.Data;

public static class DataSeeder
{
    public static async Task SeedUnidadesAsync(
        AppDbContext context,
        string? configuredCsvPath = null,
        ILogger? logger = null)
    {
        if (await context.UnidadesTomadoras.AnyAsync())
            return;

        var csvPath = string.IsNullOrWhiteSpace(configuredCsvPath)
            ? Path.Combine(AppContext.BaseDirectory, "data", "unidades.csv")
            : configuredCsvPath;

        if (!File.Exists(csvPath))
        {
            logger?.LogWarning(
                "CSV das unidades não encontrado em {CsvPath}. O seed foi ignorado.",
                csvPath);
            return;
        }

        var lines = await File.ReadAllLinesAsync(csvPath, System.Text.Encoding.UTF8);
        var unidades = new List<UnidadeTomadora>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(';');
            if (parts.Length < 5)
                continue;

            if (!int.TryParse(parts[0].Trim(), out var numero))
                continue;

            var latStr = parts[3].Trim().Replace(',', '.');
            var lonStr = parts[4].Trim().Replace(',', '.');

            if (!double.TryParse(
                    latStr,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var latitude))
                continue;

            if (!double.TryParse(
                    lonStr,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var longitude))
                continue;

            unidades.Add(new UnidadeTomadora
            {
                Numero = numero,
                Municipio = parts[1].Trim(),
                Nome = parts[2].Trim(),
                Latitude = latitude,
                Longitude = longitude,
                Cnpj = parts.Length > 5 ? parts[5].Trim() : null,
                Endereco = parts.Length > 6 ? parts[6].Trim() : null
            });
        }

        context.UnidadesTomadoras.AddRange(unidades);
        await context.SaveChangesAsync();

        logger?.LogInformation(
            "Seed concluído: {Count} unidades importadas de {CsvPath}.",
            unidades.Count,
            csvPath);
    }
}
