using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

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

        public const string ROLE_ADMINISTRADOR = "ADMINISTRADOR";
        public const string ROLE_ALUNO = "ALUNO";
        public const string ROLE_PROFESSOR = "PROFESSOR";

        public static Usuario Logado
        {
            get
            {
                if (System.Web.HttpContext.Current.User.Identity.Name != null)
                {
                    string login = System.Web.HttpContext.Current.User.Identity.Name;
                    return UsuarioDAO.Consultar(login);
                }
                return null;
            }
        }

        public static List<Usuario> Listar(string palavra = null)
        {
            return UsuarioDAO.Listar(palavra);
        }

        public static bool Desativar(Guid IdUsuario)
        {
            return UsuarioDAO.Desativar(IdUsuario);
        }

        public static bool EhAtivo(string Login)
        {
            return UsuarioDAO.EhAtivo(Login);
        }

        public static bool Existe(string Login)
        {
            return UsuarioDAO.Existe(Login);
        }

        public static List<string> Roles()
        {
            return new List<string>()
            {
                ROLE_ADMINISTRADOR,
                ROLE_ALUNO,
                ROLE_PROFESSOR
            };
        }
    }
}