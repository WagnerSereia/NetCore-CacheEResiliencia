using NetCore_Redis_Polly.Domain.Entidade;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore_Redis_Polly.Domain.Interface
{
    public interface IRepositorioUsuario
    {
        IEnumerable<Usuario> ListarUsuario();
        void AdicionarUsuario(Usuario usuario);
        void RemoverUsuario(Guid id);
    }
}
