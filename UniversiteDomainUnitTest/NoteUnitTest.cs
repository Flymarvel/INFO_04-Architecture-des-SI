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
    [Test]
    public async Task CreateNoteUseCase()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 1;
        float valeur = 15.5f;

        Note noteSansId = new Note { Valeur = valeur, EtudiantId = etudiantId, UeId = ueId };
        Note noteCreee = new Note { Valeur = valeur, EtudiantId = etudiantId, UeId = ueId };

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
        Assert.That(noteTeste.Valeur, Is.EqualTo(noteCreee.Valeur));
        Assert.That(noteTeste.EtudiantId, Is.EqualTo(etudiantId));
        Assert.That(noteTeste.UeId, Is.EqualTo(ueId));
    }
}
