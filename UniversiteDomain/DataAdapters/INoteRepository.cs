using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    /// <summary>
    /// Récupère une note par étudiant et UE
    /// </summary>
    Task<Note?> FindByEtudiantAndUeAsync(long etudiantId, long ueId);

    /// <summary>
    /// Crée ou met à jour plusieurs notes de façon atomique
    /// </summary>
    Task CreateOrUpdateManyAsync(List<Note> notes);

    /// <summary>
    /// Récupère toutes les notes d'une UE
    /// </summary>
    Task<List<Note>> FindByUeIdAsync(long ueId);
}