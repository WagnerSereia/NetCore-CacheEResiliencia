using NetCore_Redis_Polly.Domain.Entidade;
using NetCore_Redis_Polly.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore_Redis_Polly.Domain.Repositorio
{
    public class UsuarioRepositorio : IRepositorioUsuario
    {
        private List<Usuario> usuarios;
        public UsuarioRepositorio()
        {
            usuarios = new List<Usuario>();
            usuarios.Add(new Usuario(Guid.NewGuid(),"wagner","Wagner Sereia dos Santos"));
            usuarios.Add(new Usuario(Guid.NewGuid(), "maria", "Maria José"));
            usuarios.Add(new Usuario(Guid.NewGuid(), "jose", "José Maria"));            
        }
        public void AdicionarUsuario(Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        public IEnumerable<Usuario> ListarUsuario()
        {
            return usuarios;
        }

        public void RemoverUsuario(Guid id)
        {
            usuarios.Remove(usuarios.Find(x => x.IdUsuario == id));
        }
    }
}
