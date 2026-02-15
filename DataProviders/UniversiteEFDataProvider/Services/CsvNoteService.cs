using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos.Csv;
using UniversiteDomain.Exceptions.CsvExceptions;

namespace UniversiteEFDataProvider.Services;

public class CsvNoteService : ICsvNoteService
{
    public byte[] GenerateCsv(List<NoteCsvDto> notes)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true
        });

        // Écrire les en-têtes personnalisées en français
        csv.WriteField("NumEtud");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("NumeroUe");
        csv.WriteField("IntituleUe");
        csv.WriteField("Note");
        csv.NextRecord();

        // Écrire les données
        foreach (var note in notes)
        {
            csv.WriteField(note.NumEtud);
            csv.WriteField(note.Nom);
            csv.WriteField(note.Prenom);
            csv.WriteField(note.NumeroUe);
            csv.WriteField(note.IntituleUe);
            csv.WriteField(note.Note.HasValue ? note.Note.Value.ToString(CultureInfo.InvariantCulture) : "");
            csv.NextRecord();
        }

        writer.Flush();
        return memoryStream.ToArray();
    }

    public List<NoteCsvDto> ParseCsv(Stream csvContent)
    {
        try
        {
            using var reader = new StreamReader(csvContent, Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            });

            // Configuration du mapping
            csv.Context.RegisterClassMap<NoteCsvDtoMap>();

            var records = csv.GetRecords<NoteCsvDto>().ToList();
            return records;
        }
        catch (CsvHelperException ex)
        {
            throw new InvalidCsvFormatException($"Erreur de format CSV: {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is not InvalidCsvFormatException)
        {
            throw new InvalidCsvFormatException($"Erreur lors de la lecture du fichier CSV: {ex.Message}", ex);
        }
    }
}

public sealed class NoteCsvDtoMap : ClassMap<NoteCsvDto>
{
    public NoteCsvDtoMap()
    {
        Map(m => m.NumEtud).Name("NumEtud");
        Map(m => m.Nom).Name("Nom");
        Map(m => m.Prenom).Name("Prenom");
        Map(m => m.NumeroUe).Name("NumeroUe");
        Map(m => m.IntituleUe).Name("IntituleUe");
        Map(m => m.Note).Name("Note")
            .TypeConverter<NullableFloatConverter>();
    }
}

public class NullableFloatConverter : CsvHelper.TypeConversion.DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        // Essayer aussi avec la culture française (virgule comme séparateur décimal)
        if (float.TryParse(text, NumberStyles.Float, new CultureInfo("fr-FR"), out result))
            return result;

        throw new InvalidCsvDataException($"La valeur '{text}' n'est pas un nombre valide");
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null)
            return "";

        return ((float)value).ToString(CultureInfo.InvariantCulture);
    }
}
