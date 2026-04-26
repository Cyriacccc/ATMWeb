// Import des DTO utilisés pour recevoir ou renvoyer des données JSON
using ATMWeb.Dtos;

// Import des exceptions métier levées par le service
using ATMWeb.Exceptions;

// Import de l’interface du service ATM
using ATMWeb.Services;

// Import des outils ASP.NET Core pour créer un contrôleur API
using Microsoft.AspNetCore.Mvc;

namespace ATMWeb.Controllers;

// Indique que cette classe est un contrôleur d’API REST
[ApiController]

// Définit la route de base du contrôleur : /api/compte
[Route("api/compte")]
public class CompteController(IAtmService atmService) : ControllerBase
{
    // Endpoint GET /api/compte/solde
    // Permet de consulter le solde du compte
    [HttpGet("solde")]
    public ActionResult<SoldeDto> ConsulterSolde()
    {
        try
        {
            // Récupération du numéro de carte dans les headers HTTP
            var numeroCarte = Request.Headers["X-Card"].ToString();

            // Récupération du PIN dans les headers HTTP
            var pin = Request.Headers["X-Pin"].ToString();

            // Vérification minimale de la requête HTTP
            // Si les headers sont absents, on renvoie une erreur 400
            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto { Message = "Les en-têtes X-Card et X-Pin sont requis" }
                );
            }

            // Appel du service métier pour consulter le solde
            var solde = atmService.ConsulterSolde(numeroCarte, pin);

            // Retour d’une réponse 200 OK avec un DTO contenant le solde
            return Ok(new SoldeDto { Solde = solde });
        }
        catch (NotFoundException ex)
        {
            // Si la carte n’existe pas → 404 Not Found
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            // Si le PIN est incorrect → 401 Unauthorized
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            // Si la carte est bloquée → 403 Forbidden
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
    }

    // Endpoint POST /api/compte/versement
    // Permet d’effectuer un versement sur le compte
    [HttpPost("versement")]
    public ActionResult<OperationResultDto> EffectuerVersement([FromBody] MontantDto dto)
    {
        try
        {
            // Récupération des informations d’authentification dans les headers
            var numeroCarte = Request.Headers["X-Card"].ToString();
            var pin = Request.Headers["X-Pin"].ToString();

            // Vérification que les headers nécessaires sont présents
            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto { Message = "Les en-têtes X-Card et X-Pin sont requis" }
                );
            }

            // Appel du service métier pour effectuer le versement
            // Le montant vient du body JSON via MontantDto
            var nouveauSolde = atmService.EffectuerVersement(numeroCarte, pin, dto.Montant);

            // Retour 200 OK avec un message et le nouveau solde
            return Ok(
                new OperationResultDto { Message = "Versement effectué", Solde = nouveauSolde }
            );
        }
        catch (NotFoundException ex)
        {
            // Carte introuvable → 404
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            // PIN incorrect → 401
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            // Carte bloquée → 403
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
        catch (DomainValidationException ex)
        {
            // Montant invalide ou règle métier non respectée → 400
            return BadRequest(new MessageDto { Message = ex.Message });
        }
    }

    // Endpoint POST /api/compte/retrait
    // Permet d’effectuer un retrait sur le compte
    [HttpPost("retrait")]
    public ActionResult<OperationResultDto> EffectuerRetrait([FromBody] MontantDto dto)
    {
        try
        {
            // Récupération du numéro de carte et du PIN depuis les headers
            var numeroCarte = Request.Headers["X-Card"].ToString();
            var pin = Request.Headers["X-Pin"].ToString();

            // Vérification de la présence des headers
            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto { Message = "Les en-têtes X-Card et X-Pin sont requis" }
                );
            }

            // Appel du service métier pour effectuer le retrait
            var nouveauSolde = atmService.EffectuerRetrait(numeroCarte, pin, dto.Montant);

            // Retour 200 OK avec le message et le nouveau solde
            return Ok(
                new OperationResultDto { Message = "Retrait effectué", Solde = nouveauSolde }
            );
        }
        catch (NotFoundException ex)
        {
            // Carte inexistante → 404
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            // Mauvais PIN → 401
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            // Carte bloquée → 403
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
        catch (DomainValidationException ex)
        {
            // Montant négatif ou solde insuffisant → 400
            return BadRequest(new MessageDto { Message = ex.Message });
        }
    }
}