using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);
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

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
