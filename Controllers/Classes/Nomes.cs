using Bogus;

namespace APITesteDev.Controllers.Classes
{
    public class Nomes
    {
        private readonly String? valoresFalsos;
        public static Faker<Cliente>? ValoresFalsos { get; private set; }
        public static List<Cliente> GerarNomes(int quantidade)
        {
            ValoresFalsos = new Faker<Cliente>("pt_BR")
                 .RuleFor(c => c.Nome, f => f.Person.FullName)
                 .RuleFor(c => c.Email, f => f.Person.Email)
                 .RuleFor(c => c.Endereco, f => f.Person.Address.City)
                 .RuleFor(c => c.DataNascimento, f => f.Date.Past(40, DateTime.Now.AddYears(-13)));

            return ValoresFalsos.Generate(quantidade);
        }
    }
}
