using BCrypt.Net;

namespace APITesteDev.Controllers.Classes
{
    public class Usuario
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        private string Senha { get; private set; }

        public Usuario(string nome, string email, string senha)
        {
            Nome = nome;
            Email = email;
            Senha = BCrypt.Net.BCrypt.EnhancedHashPassword(senha, 8);
        }

        public bool ValidarSenha(string senhaDigitada)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(senhaDigitada, Senha);
        }

        public static List<Usuario> Usuarios = new()
        {
            new Usuario("Admin", "admin@email.com", "admin123"),
            new Usuario("Usuario", "usuario@email.com", "user123")
        };
    }
}
