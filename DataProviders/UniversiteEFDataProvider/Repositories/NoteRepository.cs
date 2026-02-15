using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note?> FindByEtudiantAndUeAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.FirstOrDefaultAsync(n =>
            n.EtudiantId == etudiantId && n.UeId == ueId);
    }

    public async Task CreateOrUpdateManyAsync(List<Note> notes)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);

        foreach (var note in notes)
        {
            var existingNote = await FindByEtudiantAndUeAsync(note.EtudiantId, note.UeId);
            if (existingNote != null)
            {
                // Mettre à jour la note existante
                existingNote.Valeur = note.Valeur;
                Context.Notes.Update(existingNote);
            }
            else
            {
                // Créer une nouvelle note
                await Context.Notes.AddAsync(note);
            }
        }

        await Context.SaveChangesAsync();
    }

    public async Task<List<Note>> FindByUeIdAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .Include(n => n.Etudiant)
            .Where(n => n.UeId == ueId)
            .ToListAsync();
    }

    public async Task DeleteByEtudiantIdAsync(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        var notes = await Context.Notes.Where(n => n.EtudiantId == etudiantId).ToListAsync();
        Context.Notes.RemoveRange(notes);
        await Context.SaveChangesAsync();
    }
}