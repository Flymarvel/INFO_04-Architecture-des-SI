using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;

namespace UniversiteDomainUnitTests;

public class EtudiantUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        String numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";
        
        // On crée l'étudiant qui doit être ajouté en base
        Etudiant etudiantSansId = new Etudiant{NumEtud=numEtud, Nom = nom, Prenom=prenom, Email=email};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mock = new Mock<IEtudiantRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Etudiant>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Etudiant etudiantCree =new Etudiant{Id=id,NumEtud=numEtud, Nom = nom, Prenom=prenom, Email=email};
        mock.Setup(repoEtudiant=>repoEtudiant.CreateAsync(etudiantSansId)).ReturnsAsync(etudiantCree);
        
        // On crée le bouchon (un faux etudiantRepository). Il est prêt à être utilisé
        var fauxEtudiantRepository = mock.Object;
        
        // Création du use case en injectant notre faux repository
        CreateEtudiantUseCase useCase=new CreateEtudiantUseCase(fauxEtudiantRepository);
        // Appel du use case
        var etudiantTeste=await useCase.ExecuteAsync(etudiantSansId);
        
        // Vérification du résultat
        Assert.That(etudiantTeste.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(etudiantTeste.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(etudiantTeste.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(etudiantTeste.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(etudiantTeste.Email, Is.EqualTo(etudiantCree.Email));
    }
    
       
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        // Création du use case en utilisant le mock comme datasource
        //AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockEtudiant.Object, mockParcours.Object);
        
        // Appel du use case
        //var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        //Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        //Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        //Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        //Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
}
