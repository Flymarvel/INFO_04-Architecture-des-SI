using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public async Task CreateUeUseCase()
    {
        long id = 1;
        string numeroUe = "UE001";
        string intitule = "Architecture logicielle";
        
        // On crée l'UE qui doit être ajoutée en base
        Ue ueSansId = new Ue{NumeroUe = numeroUe, Intitule = intitule};
        
        // Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mockUe = new Mock<IUeRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'UE n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mockUe.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'une UE renvoie une UE avec l'Id 1
        Ue ueCreee = new Ue{Id = id, NumeroUe = numeroUe, Intitule = intitule};
        mockUe.Setup(repoUe=>repoUe.CreateAsync(ueSansId)).ReturnsAsync(ueCreee);
        
        // Création du mock de IRepositoryFactory
        var mockRepositoryFactory = new Mock<IRepositoryFactory>();
        mockRepositoryFactory.Setup(factory => factory.UeRepository()).Returns(mockUe.Object);
        
        // Création du use case en utilisant le mock de factory
        CreateUeUseCase useCase = new CreateUeUseCase(mockRepositoryFactory.Object);
        
        // Appel du use case
        var ueTestee = await useCase.ExecuteAsync(ueSansId);
        
        // Vérification du résultat
        Assert.That(ueTestee.Id, Is.EqualTo(ueCreee.Id));
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(ueCreee.NumeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(ueCreee.Intitule));
    }
}