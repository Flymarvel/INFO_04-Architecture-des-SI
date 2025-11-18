using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomainUnitTests;

public class NoteUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateNoteUseCase()
    {
        // Arrange
        long id = 1;
        float valeur = 15.5f;

        Etudiant etudiant = new Etudiant { Id = 1, NumEtud = "et1", Nom = "Durant", Prenom = "Jean", Email = "jean.durant@etud.u-picardie.fr" };
        Ue ue = new Ue { Id = 1, NumeroUe = "UE001", Intitule = "Programmation" };

        Note noteSansId = new Note { Valeur = valeur, EtudiantNotee = etudiant, UeNotee = ue };
        Note noteCreee = new Note { Id = id, Valeur = valeur, EtudiantNotee = etudiant, UeNotee = ue };

        // Simulation du repository
        var mockNote = new Mock<INoteRepository>();
        mockNote.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>()); // Aucune note existante
        mockNote.Setup(repo => repo.CreateAsync(noteSansId)).ReturnsAsync(noteCreee);

        var mockRepositoryFactory = new Mock<IRepositoryFactory>();
        mockRepositoryFactory.Setup(factory => factory.NoteRepository()).Returns(mockNote.Object);

        // Act
        CreateNoteUseCase useCase = new CreateNoteUseCase(mockRepositoryFactory.Object);
        var noteTeste = await useCase.ExecuteAsync(noteSansId);

        // Assert
        Assert.That(noteTeste.Id, Is.EqualTo(noteCreee.Id));
        Assert.That(noteTeste.Valeur, Is.EqualTo(noteCreee.Valeur));
        Assert.That(noteTeste.EtudiantNotee?.Id, Is.EqualTo(etudiant.Id));
        Assert.That(noteTeste.UeNotee?.Id, Is.EqualTo(ue.Id));
    }
}
