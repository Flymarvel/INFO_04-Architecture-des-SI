using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);

        // Supprimer d'abord le user associé à l'étudiant
        await repositoryFactory.UniversiteUserRepository().DeleteByEtudiantIdAsync(id);

        // Supprimer les notes de l'étudiant
        await repositoryFactory.NoteRepository().DeleteByEtudiantIdAsync(id);

        // Supprimer l'étudiant
        await repositoryFactory.EtudiantRepository().DeleteAsync(id);
        await repositoryFactory.EtudiantRepository().SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long id)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());

        // On vérifie que l'étudiant existe
        Etudiant? etudiant = await repositoryFactory.EtudiantRepository().FindAsync(id);
        if (etudiant == null) throw new EtudiantNotFoundException("Etudiant avec l'id " + id + " non trouvé");
    }

    public bool IsAuthorized(string role, IUniversiteUser? user, long idEtudiant)
    {
        // Responsable et Scolarité peuvent supprimer n'importe quel étudiant
        if (role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite)) return true;
        // Un étudiant peut demander la suppression de son propre compte
        return user?.Etudiant != null && role.Equals(Roles.Etudiant) && user.Etudiant.Id == idEtudiant;
    }
}
