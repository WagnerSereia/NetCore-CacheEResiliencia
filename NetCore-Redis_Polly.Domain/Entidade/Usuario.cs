using System;

namespace NetCore_Redis_Polly.Domain.Entidade
{
    public class Usuario
    {
        public Usuario(Guid idUsuario, string login, string nome)
        {
            IdUsuario = Guid.NewGuid();
            Login = login;
            Nome = nome;
        }

        public Guid IdUsuario { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
    }
}
