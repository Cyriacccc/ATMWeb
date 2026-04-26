// Import des DTO pour les réponses JSON
using ATMWeb.Dtos;

// Import des exceptions métier
using ATMWeb.Exceptions;

// Import du service ATM
using ATMWeb.Services;

// Import des outils ASP.NET Core
using Microsoft.AspNetCore.Mvc;

namespace ATMWeb.Controllers;

// Indique que c’est un contrôleur d’API REST
[ApiController]

// Route de base : /api/atm
[Route("api/atm")]
public class AtmController(IAtmService atmService) : ControllerBase
{
    // Endpoint POST /api/atm/reset
    // Permet de réinitialiser l’état de l’ATM
    [HttpPost("reset")]
    public ActionResult<MessageDto> Reset()
    {
        try
        {
            // Appel du service métier
            atmService.Reset();

            // Retour 200 OK avec un message
            return Ok(new MessageDto { Message = "ATM réinitialisé" });
        }
        catch (NotFoundException ex)
        {
            // Si la carte de test n’existe pas → 404
            return NotFound(new MessageDto { Message = ex.Message });
        }
    }
}