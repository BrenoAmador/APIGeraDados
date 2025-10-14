using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace APITesteDev.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private string Host { get; set; }
        private int Port { get; set; }
        private string User { get; set; }
        private bool useSSL { get; set; }
        private string Password { get; set; }

        public EmailController(IConfiguration configuration)
        {
            Host = configuration["EmailSettings:SmtpServer"];
            Port = int.Parse(configuration["EmailSettings:SmtpPort"]);
            useSSL = bool.Parse(configuration["EmailSettings:useSSL"]);
            User = configuration["EmailSettings:SmtpUser"];
            Password = configuration["EmailSettings:SmtpPass"];

        }
        [Authorize]
        [HttpPost("enviar")]
        public IActionResult EnviarEmail()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos");

            if (!Directory.Exists(folderPath))
                return NotFound("Diretório não encontrado!");

            var arquivos = Directory.GetFiles(folderPath, "*.xlsx");

            if (arquivos.Length == 0)
                return NotFound("Nenhum arquivo encontrado.");

            var ultimoArquivo = arquivos
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .First();

            var filePath = ultimoArquivo.FullName;

            if (!System.IO.File.Exists(filePath))
                return NotFound("Ultimo arquivo da pasta não encontrado.");

            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Breno Amador", User));

                message.To.Add(MailboxAddress.Parse("brenoamadors@gmail.com"));

                message.Subject = "[Gerador de Clientes - Excel] - Dados Gerados";

                var builder = new BodyBuilder
                {
                    TextBody = "Segue o arquivo gerado do projeto."
                };

                builder.Attachments.Add(filePath);

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(Host, Port, useSSL ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(User, Password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return Ok($"Arquivo {ultimoArquivo.Name} enviado com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar o e-mail: {ex.Message}");
            }
        }
    }
}
