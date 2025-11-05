using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue{NumeroUe = numeroUe, Intitule = intitule};
        return await ExecuteAsync(ue);
    }
    
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue u = await repositoryFactory.UeRepository().CreateAsync(ue);
        repositoryFactory.UeRepository().SaveChangesAsync().Wait();
        return u;
    }
    
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        
        // On recherche une UE avec le même numéro
        List<Ue> existe = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));

        // Si une UE avec le même numéro existe déjà, on lève une exception personnalisée
        if (existe is {Count: > 0}) throw new DuplicateNumeroUeException(ue.NumeroUe + " - ce numéro d'UE est déjà affecté à une UE");
        
        // Le métier définit que l'intitulé doit contenir plus de 3 caractères
        if (ue.Intitule.Length < 3) throw new InvalidIntituleException(ue.Intitule + " incorrect - L'intitulé d'une UE doit contenir plus de 3 caractères");
    }
}