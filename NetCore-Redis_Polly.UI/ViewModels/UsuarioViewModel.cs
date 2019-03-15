using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore_Redis_Polly.UI.ViewModels
{
    public class UsuarioViewModel
    {
        public UsuarioViewModel(Guid idUsuario, string login, string nome)
        {
            IdUsuario = idUsuario;
            Login = login;
            Nome = nome;
        }

        public Guid IdUsuario { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
    }
}
