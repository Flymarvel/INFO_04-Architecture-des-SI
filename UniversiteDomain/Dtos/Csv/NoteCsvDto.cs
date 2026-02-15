namespace UniversiteDomain.Dtos.Csv;

public class NoteCsvDto
{
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NumeroUe { get; set; } = string.Empty;
    public string IntituleUe { get; set; } = string.Empty;
    public float? Note { get; set; }
}
