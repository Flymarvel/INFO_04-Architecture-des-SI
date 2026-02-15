using UniversiteDomain.Dtos.Csv;

namespace UniversiteDomain.DataAdapters;

public interface ICsvNoteService
{
    byte[] GenerateCsv(List<NoteCsvDto> notes);
    List<NoteCsvDto> ParseCsv(Stream csvContent);
}
