using UniversiteDomain.Dtos.Csv;

namespace UniversiteDomain.DataAdapters;

/// <summary>
/// Interface pour le service de manipulation CSV des notes
/// L'implémentation doit être dans le DataProvider car utilise CsvHelper
/// </summary>
public interface ICsvNoteService
{
    /// <summary>
    /// Génère un fichier CSV à partir d'une liste de notes
    /// </summary>
    /// <param name="notes">Liste des DTOs de notes à exporter</param>
    /// <returns>Contenu du fichier CSV sous forme de bytes</returns>
    byte[] GenerateCsv(List<NoteCsvDto> notes);

    /// <summary>
    /// Parse un fichier CSV et retourne la liste des notes
    /// </summary>
    /// <param name="csvContent">Contenu du fichier CSV</param>
    /// <returns>Liste des DTOs de notes parsées</returns>
    List<NoteCsvDto> ParseCsv(Stream csvContent);
}
