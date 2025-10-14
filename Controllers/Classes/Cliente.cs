using Bogus;
namespace APITesteDev.Controllers.Classes
{
    public class Cliente
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}