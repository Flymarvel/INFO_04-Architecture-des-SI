using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.Csv;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.CsvExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Csv;

public class ImportCsvNotesUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<int> ExecuteAsync(Stream csvContent)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.CsvNoteService());

        // Parser le CSV
        var notesCsv = repositoryFactory.CsvNoteService().ParseCsv(csvContent);

        if (notesCsv.Count == 0)
            throw new InvalidCsvDataException("Le fichier CSV est vide ou ne contient aucune donnée");

        // Valider toutes les données AVANT d'enregistrer quoi que ce soit
        var (notesValides, erreurs) = await ValidateAndPrepareNotes(notesCsv);

        // Si des erreurs existent, lever une exception avec toutes les erreurs
        if (erreurs.Count > 0)
            throw new CsvValidationException(erreurs);

        // Toutes les validations ont passé, enregistrer les notes
        await repositoryFactory.NoteRepository().CreateOrUpdateManyAsync(notesValides);

        return notesValides.Count;
    }

    private async Task<(List<Note> notesValides, List<string> erreurs)> ValidateAndPrepareNotes(
        List<NoteCsvDto> notesCsv)
    {
        var notesValides = new List<Note>();
        var erreurs = new List<string>();
        var etudiantsTraites = new HashSet<string>(); // Pour détecter les doublons

        // Vérifier que toutes les lignes concernent la même UE
        var numeroUe = notesCsv.First().NumeroUe;
        if (notesCsv.Any(n => n.NumeroUe != numeroUe))
        {
            erreurs.Add("Toutes les lignes doivent concerner la même UE");
        }

        // Récupérer l'UE
        Ue? ue = await repositoryFactory.UeRepository().FindByNumeroUeAsync(numeroUe);
        if (ue == null)
        {
            erreurs.Add($"L'UE avec le numéro '{numeroUe}' n'existe pas");
            return (notesValides, erreurs);
        }

        int ligne = 2; // Ligne 1 = en-tête
        foreach (var noteCsv in notesCsv)
        {
            // Vérifier les doublons d'étudiants dans le fichier
            if (etudiantsTraites.Contains(noteCsv.NumEtud))
            {
                erreurs.Add($"Ligne {ligne}: L'étudiant {noteCsv.NumEtud} apparaît plusieurs fois dans le fichier");
                ligne++;
                continue;
            }
            etudiantsTraites.Add(noteCsv.NumEtud);

            // Si pas de note, passer à la ligne suivante (case vide = pas de note à enregistrer)
            if (!noteCsv.Note.HasValue)
            {
                ligne++;
                continue;
            }

            // Valider la note (entre 0 et 20)
            if (noteCsv.Note.Value < 0 || noteCsv.Note.Value > 20)
            {
                erreurs.Add($"Ligne {ligne}: La note {noteCsv.Note.Value} pour l'étudiant {noteCsv.NumEtud} doit être comprise entre 0 et 20");
                ligne++;
                continue;
            }

            // Trouver l'étudiant
            var etudiant = await repositoryFactory.EtudiantRepository().FindByNumEtudAsync(noteCsv.NumEtud);
            if (etudiant == null)
            {
                erreurs.Add($"Ligne {ligne}: L'étudiant avec le numéro '{noteCsv.NumEtud}' n'existe pas");
                ligne++;
                continue;
            }

            // Vérifier que l'étudiant est bien inscrit dans un parcours qui enseigne cette UE
            bool estInscritDansUe = await VerifierEtudiantInscritUe(etudiant.Id, ue.Id);
            if (!estInscritDansUe)
            {
                erreurs.Add($"Ligne {ligne}: L'étudiant {noteCsv.NumEtud} n'est pas inscrit dans un parcours qui enseigne l'UE {numeroUe}");
                ligne++;
                continue;
            }

            // Créer la note
            notesValides.Add(new Note
            {
                EtudiantId = etudiant.Id,
                UeId = ue.Id,
                Valeur = noteCsv.Note.Value
            });

            ligne++;
        }

        return (notesValides, erreurs);
    }

    private async Task<bool> VerifierEtudiantInscritUe(long etudiantId, long ueId)
    {
        // Récupérer l'UE avec ses parcours
        var ue = await repositoryFactory.UeRepository().FindUeWithEtudiantsAndNotesAsync(ueId);
        if (ue?.EnseigneeDans == null) return false;

        // Vérifier si l'étudiant est inscrit dans un des parcours
        return ue.EnseigneeDans.Any(p =>
            p.Inscrits?.Any(e => e.Id == etudiantId) == true);
    }

    public bool IsAuthorized(string role)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses notes
        return false;
    }
}
