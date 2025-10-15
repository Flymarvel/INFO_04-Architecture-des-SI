using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int AnneeFormation)
    {
        var parcours = new Parcours{NomParcours = nomParcours, AnneeFormation = AnneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pa = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return pa;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours));

        // Si un parcours avec le même nom de parcours existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours+ " - ce nom de parcours est déjà affecté à un parcours");
        
        // On vérifie que l'année de formation est cohérente
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 5) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " - L'année de formation doit être comprise entre 1 et 5");
        
        // Le métier définit que les nom doit contenir plus de 3 lettres
        if (parcours.NomParcours.Length < 3) throw new InvalidNomParcoursException(parcours.NomParcours +" incorrect - Le nom du parcours doit contenir plus de 3 caractères");
    }
}
