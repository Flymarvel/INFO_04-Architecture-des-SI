using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.CsvExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Csv;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    private void CheckSecu(out string role, out string email, out IUniversiteUser? user)
    {
        role = "";
        // Récupération des informations de connexion dans la requête http entrante
        ClaimsPrincipal claims = HttpContext.User;
        // Faisons nos tests pour savoir si la personne est bien connectée
        if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
        // Récupérons le email de la personne connectée
        var emailClaim = claims.FindFirst(ClaimTypes.Email);
        if (emailClaim == null) throw new UnauthorizedAccessException();
        email = emailClaim.Value;
        if (email == null) throw new UnauthorizedAccessException();
        // Vérifions qu'il est bien associé à un utilisateur référencé
        user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
        if (user == null) throw new UnauthorizedAccessException();
        // Vérifions qu'un rôle a bien été défini
        if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
        // Récupérons le rôle de l'utilisateur
        var ident = claims.Identities.FirstOrDefault();
        if (ident == null) throw new UnauthorizedAccessException();
        var roleClaim = ident.FindFirst(ClaimTypes.Role);
        if (roleClaim == null) throw new UnauthorizedAccessException();
        role = roleClaim.Value;
        if (role == null) throw new UnauthorizedAccessException();
        // Vérifions que le user a bien le role envoyé via http
        bool isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result;
        if (!isInRole) throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// Télécharge un fichier CSV pour saisir les notes d'une UE
    /// Le fichier contient tous les étudiants inscrits à l'UE avec leurs notes existantes
    /// </summary>
    /// <param name="ueId">ID de l'UE</param>
    /// <returns>Fichier CSV</returns>
    [HttpGet("csv/{ueId}")]
    public async Task<IActionResult> GetCsvNotesUe(long ueId)
    {
        string role;
        string email;
        IUniversiteUser? user;

        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        GenerateCsvNotesUeUseCase uc = new GenerateCsvNotesUeUseCase(repositoryFactory);

        // Vérifier l'autorisation (uniquement Scolarité)
        if (!uc.IsAuthorized(role, user)) return Unauthorized("Seule la scolarité peut télécharger le fichier CSV des notes");

        try
        {
            var csvContent = await uc.ExecuteAsync(ueId);
            return File(csvContent, "text/csv", $"notes_ue_{ueId}.csv");
        }
        catch (UeNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(ex), ex.Message);
            return ValidationProblem();
        }
    }

    /// <summary>
    /// Importe les notes d'une UE à partir d'un fichier CSV
    /// Les notes ne sont enregistrées que si aucune erreur n'est trouvée
    /// </summary>
    /// <param name="file">Fichier CSV contenant les notes</param>
    /// <returns>Nombre de notes créées/mises à jour</returns>
    [HttpPost("csv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportCsvNotesUe(IFormFile file)
    {
        string role;
        string email;
        IUniversiteUser? user;

        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        ImportCsvNotesUeUseCase uc = new ImportCsvNotesUeUseCase(repositoryFactory);

        // Vérifier l'autorisation (uniquement Scolarité)
        if (!uc.IsAuthorized(role)) return Unauthorized("Seule la scolarité peut importer les notes via CSV");

        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier fourni");

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Le fichier doit être au format CSV");

        try
        {
            using var stream = file.OpenReadStream();
            var nombreNotesImportees = await uc.ExecuteAsync(stream);
            return Ok(new { message = $"{nombreNotesImportees} note(s) importée(s) avec succès" });
        }
        catch (CsvValidationException ex)
        {
            return BadRequest(new
            {
                message = "Le fichier contient des erreurs de validation",
                erreurs = ex.Erreurs
            });
        }
        catch (InvalidCsvFormatException ex)
        {
            return BadRequest(new { message = "Format CSV invalide", erreur = ex.Message });
        }
        catch (InvalidCsvDataException ex)
        {
            return BadRequest(new { message = "Données CSV invalides", erreur = ex.Message });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(ex), ex.Message);
            return ValidationProblem();
        }
    }
}
