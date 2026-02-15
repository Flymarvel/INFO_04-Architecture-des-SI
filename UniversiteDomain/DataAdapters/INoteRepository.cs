using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task<Note?> FindByEtudiantAndUeAsync(long etudiantId, long ueId);
    Task CreateOrUpdateManyAsync(List<Note> notes);
    Task<List<Note>> FindByUeIdAsync(long ueId);
    Task DeleteByEtudiantIdAsync(long etudiantId);
}