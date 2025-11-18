namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public float Valeur { get; set; }

    // ManyToOne : une note est attribuée à un seul étudiant
    public Etudiant? EtudiantNotee { get; set; } = null;

    // ManyToOne : une note est pour une seule UE
    public Ue? UeNotee { get; set; } = null;

    public override string ToString()
    {
        return $"ID {Id} : Valeur {Valeur} - Étudiant {EtudiantNotee?.NumEtud} pour l'UE {UeNotee?.NumeroUe}";
    }
}