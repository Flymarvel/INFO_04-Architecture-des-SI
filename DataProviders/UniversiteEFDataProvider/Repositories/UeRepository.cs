using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    public async Task<Ue?> FindUeWithEtudiantsAndNotesAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues
            .Include(u => u.Notes)
            .Include(u => u.EnseigneeDans!)
                .ThenInclude(p => p.Inscrits)
            .FirstOrDefaultAsync(u => u.Id == ueId);
    }

    public async Task<Ue?> FindByNumeroUeAsync(string numeroUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues.FirstOrDefaultAsync(u => u.NumeroUe == numeroUe);
    }
}