namespace UniversiteDomain.Exceptions.CsvExceptions;

/// <summary>
/// Exception levée quand le fichier CSV contient des erreurs de validation
/// Contient la liste de toutes les erreurs trouvées
/// </summary>
[Serializable]
public class CsvValidationException : Exception
{
    public List<string> Erreurs { get; }

    public CsvValidationException(List<string> erreurs)
        : base($"Le fichier CSV contient {erreurs.Count} erreur(s): {string.Join("; ", erreurs)}")
    {
        Erreurs = erreurs;
    }

    public CsvValidationException(string message) : base(message)
    {
        Erreurs = new List<string> { message };
    }

    public CsvValidationException(string message, Exception inner) : base(message, inner)
    {
        Erreurs = new List<string> { message };
    }
}
