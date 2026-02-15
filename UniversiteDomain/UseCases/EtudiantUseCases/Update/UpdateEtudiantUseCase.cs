using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        await repositoryFactory.EtudiantRepository().UpdateAsync(etudiant);
        await repositoryFactory.EtudiantRepository().SaveChangesAsync();
        return etudiant;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(etudiant);

        // On vérifie que l'étudiant existe
        Etudiant? existing = await repositoryFactory.EtudiantRepository().FindAsync(etudiant.Id);
        if (existing == null) throw new EtudiantNotFoundException("Etudiant avec l'id " + etudiant.Id + " non trouvé");
    }

    public bool IsAuthorized(string role, IUniversiteUser? user, long idEtudiant)
    {
        // Responsable et Scolarité peuvent modifier n'importe quel étudiant
        if (role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite)) return true;
        // Un étudiant peut modifier ses propres informations
        return user?.Etudiant != null && role.Equals(Roles.Etudiant) && user.Etudiant.Id == idEtudiant;
    }
}
