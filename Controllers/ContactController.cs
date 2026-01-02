using EmailSenderApp.Models;
using EmailSenderApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderApp.Controllers
{
    [ApiController]
    [Route("contact")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] EmailRequest request)
        {
            if (!request.IsValid(out var validationResults))
                return BadRequest(validationResults);

            try
            {
                await _emailSender.SendEmailAsync(request);
                return Ok(new { success = true, message = "Mensagem enviada com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail");
                return Problem(
                    detail: "Ocorreu um erro ao processar sua solicitação.",
                    statusCode: 500);
            }
        }
    }

}
