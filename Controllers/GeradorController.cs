using APITesteDev.Controllers.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APITesteDev.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeradorController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(Nomes.GerarNomes(10));
        }
        [Authorize]
        [HttpGet("{valor:int}")]
        public IActionResult GerarDados(int valor)
        {
            return Ok(Nomes.GerarNomes(valor));
        }
        [Authorize]
        [HttpGet("excel/{valor:int}")]
        public IActionResult GerarExcel(int valor)
        {
            if (valor < 1 || valor > 1000)
                return BadRequest("O valor deve estar entre 1 e 1000.");

            var dados = Nomes.GerarNomes(valor);
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Clientes");

            worksheet.Cell(1, 1).Value = "Nome";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Endereço";
            worksheet.Cell(1, 4).Value = "Data de Nascimento";

            var cabecalho = worksheet.Range(1, 1, 1, 4);
            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            var linha = 2;
            foreach (var cliente in dados)
            {
                worksheet.Cell(linha, 1).Value = cliente.Nome;
                worksheet.Cell(linha, 2).Value = cliente.Email;
                worksheet.Cell(linha, 3).Value = cliente.Endereco;
                worksheet.Cell(linha, 4).Value = cliente.DataNascimento.ToString("dd/MM/yyyy");
                linha++;
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"clientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(folderPath, fileName);
            workbook.SaveAs(filePath);

            return (PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName));
        }

    }
}
