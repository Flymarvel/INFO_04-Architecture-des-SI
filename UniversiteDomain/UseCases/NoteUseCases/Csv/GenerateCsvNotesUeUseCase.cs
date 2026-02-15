using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.Csv;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Csv;

public class GenerateCsvNotesUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<byte[]> ExecuteAsync(long ueId)
    {
        await CheckBusinessRules(ueId);

        // Récupérer l'UE avec les étudiants et notes
        Ue? ue = await repositoryFactory.UeRepository().FindUeWithEtudiantsAndNotesAsync(ueId);
        if (ue == null)
            throw new UeNotFoundException($"L'UE avec l'ID {ueId} n'existe pas");

        // Construire la liste des DTOs pour le CSV
        var notesCsvDtos = new List<NoteCsvDto>();

        // Récupérer tous les parcours qui enseignent cette UE
        // Pour chaque parcours, récupérer les étudiants inscrits
        if (ue.EnseigneeDans != null)
        {
            foreach (var parcours in ue.EnseigneeDans)
            {
                if (parcours.Inscrits != null)
                {
                    foreach (var etudiant in parcours.Inscrits)
                    {
                        // Vérifier si l'étudiant a déjà une note pour cette UE
                        var noteExistante = ue.Notes?.FirstOrDefault(n => n.EtudiantId == etudiant.Id);

                        notesCsvDtos.Add(new NoteCsvDto
                        {
                            NumEtud = etudiant.NumEtud,
                            Nom = etudiant.Nom,
                            Prenom = etudiant.Prenom,
                            NumeroUe = ue.NumeroUe,
                            IntituleUe = ue.Intitule,
                            Note = noteExistante?.Valeur
                        });
                    }
                }
            }
        }

        // Générer le CSV via le service
        return repositoryFactory.CsvNoteService().GenerateCsv(notesCsvDtos);
    }

    private async Task CheckBusinessRules(long ueId)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.CsvNoteService());

        // Vérifier que l'UE existe
        var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
        if (ue == null)
            throw new UeNotFoundException($"L'UE avec l'ID {ueId} n'existe pas");
    }

    public bool IsAuthorized(string role)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses notes
        return false;
    }
}
