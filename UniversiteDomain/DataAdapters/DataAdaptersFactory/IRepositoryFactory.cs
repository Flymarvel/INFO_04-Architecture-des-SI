namespace UniversiteDomain.DataAdapters.DataAdaptersFactory;

public interface IRepositoryFactory
{
    IParcoursRepository ParcoursRepository();
    IEtudiantRepository EtudiantRepository();
    IUeRepository UeRepository();
    INoteRepository NoteRepository();
    IUniversiteRoleRepository UniversiteRoleRepository();
    IUniversiteUserRepository UniversiteUserRepository();
    ICsvNoteService CsvNoteService();
    // Méthodes de gestion de la dadasource
    // Ce sont des méthodes qui permettent de gérer l'ensemble du data source
    // comme par exemple tout supprimer ou tout créer
    Task EnsureDeletedAsync();
    Task EnsureCreatedAsync();
    Task SaveChangesAsync();
}