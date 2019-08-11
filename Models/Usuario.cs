using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class Usuario
    {
        public Guid IdUsuario { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static List<Usuario> Listar(string palavra = null)
        {
            return UsuarioDAO.Listar(palavra);
        }

        public static bool Desativar(Guid IdUsuario)
        {
            return UsuarioDAO.Desativar(IdUsuario);
        }
    }
}