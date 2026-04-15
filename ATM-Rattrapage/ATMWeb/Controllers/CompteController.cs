using ATMWeb.Dtos;
using ATMWeb.Exceptions;
using ATMWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ATMWeb.Controllers;

[ApiController]
[Route("api/compte")]
public class CompteController(IAtmService atmService) : ControllerBase
{
    [HttpGet("solde")]
    public ActionResult<SoldeDto> ConsulterSolde()
    {
        try
        {
            var numeroCarte = Request.Headers["X-Card"].ToString();
            var pin = Request.Headers["X-Pin"].ToString();

            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto
                    {
                        Message = "Les en-têtes X-Card et X-Pin sont requis"
                    }
                );
            }

            var solde = atmService.ConsulterSolde(numeroCarte, pin);

            return Ok(
                new SoldeDto
                {
                    Solde = solde
                }
            );
        }
        catch (NotFoundException ex)
        {
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
    }

    [HttpPost("versement")]
    public ActionResult<OperationResultDto> EffectuerVersement([FromBody] MontantDto dto)
    {
        try
        {
            var numeroCarte = Request.Headers["X-Card"].ToString();
            var pin = Request.Headers["X-Pin"].ToString();

            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto
                    {
                        Message = "Les en-têtes X-Card et X-Pin sont requis"
                    }
                );
            }

            var nouveauSolde = atmService.EffectuerVersement(numeroCarte, pin, dto.Montant);

            return Ok(
                new OperationResultDto
                {
                    Message = "Versement effectué",
                    Solde = nouveauSolde
                }
            );
        }
        catch (NotFoundException ex)
        {
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new MessageDto { Message = ex.Message });
        }
    }

    [HttpPost("retrait")]
    public ActionResult<OperationResultDto> EffectuerRetrait([FromBody] MontantDto dto)
    {
        try
        {
            var numeroCarte = Request.Headers["X-Card"].ToString();
            var pin = Request.Headers["X-Pin"].ToString();

            if (string.IsNullOrWhiteSpace(numeroCarte) || string.IsNullOrWhiteSpace(pin))
            {
                return BadRequest(
                    new MessageDto
                    {
                        Message = "Les en-têtes X-Card et X-Pin sont requis"
                    }
                );
            }

            var nouveauSolde = atmService.EffectuerRetrait(numeroCarte, pin, dto.Montant);

            return Ok(
                new OperationResultDto
                {
                    Message = "Retrait effectué",
                    Solde = nouveauSolde
                }
            );
        }
        catch (NotFoundException ex)
        {
            return NotFound(new MessageDto { Message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new MessageDto { Message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new MessageDto { Message = ex.Message });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new MessageDto { Message = ex.Message });
        }
    }
}