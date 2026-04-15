using ATMWeb.Dtos;
using ATMWeb.Exceptions;
using ATMWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ATMWeb.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController(IAtmService atmService) : ControllerBase
{
    [HttpPost("reset")]
    public ActionResult<MessageDto> Reset()
    {
        try
        {
            atmService.Reset();

            return Ok(
                new MessageDto
                {
                    Message = "ATM réinitialisé"
                }
            );
        }
        catch (NotFoundException ex)
        {
            return NotFound(
                new MessageDto
                {
                    Message = ex.Message
                }
            );
        }
    }
}