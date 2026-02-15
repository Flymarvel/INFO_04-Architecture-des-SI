using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository : IRepository<Ue>
{
    /// <summary>
    /// Récupère une UE avec tous les étudiants inscrits (via les parcours qui enseignent cette UE)
    /// et les notes existantes
    /// </summary>
    Task<Ue?> FindUeWithEtudiantsAndNotesAsync(long ueId);

    /// <summary>
    /// Trouve une UE par son numéro
    /// </summary>
    Task<Ue?> FindByNumeroUeAsync(string numeroUe);
}