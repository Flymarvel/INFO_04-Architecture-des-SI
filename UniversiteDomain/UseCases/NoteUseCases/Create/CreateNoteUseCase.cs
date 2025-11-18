using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(float valeur, Etudiant etudiant, Ue ue)
    {
        var note = new Note { Valeur = valeur, EtudiantNotee = etudiant, UeNotee = ue };
        return await ExecuteAsync(note);
    }

    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        Note noteCreee = await repositoryFactory.NoteRepository().CreateAsync(note);
        await repositoryFactory.NoteRepository().SaveChangesAsync();
        return noteCreee;
    }

    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(note.EtudiantNotee);
        ArgumentNullException.ThrowIfNull(note.UeNotee);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());

        // Vérification que la valeur est entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20)
            throw new InvalidValeurException($"{note.Valeur} - La note doit être entre 0 et 20");

        // Vérifier qu'il n'existe pas déjà une note pour cet étudiant dans cette UE
        List<Note> existe = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n =>
                n.EtudiantNotee != null && n.EtudiantNotee.Id == note.EtudiantNotee.Id &&
                n.UeNotee != null && n.UeNotee.Id == note.UeNotee.Id);

        if (existe is { Count: > 0 })
            throw new DuplicateNoteException(
                $"Une note existe déjà pour l'étudiant {note.EtudiantNotee.NumEtud} dans l'UE {note.UeNotee.NumeroUe}");
    }
}
