using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Professor
    {
        public Guid IdProfessor { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataNascimento { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [EmailAddress(ErrorMessage = "Preencha um e-mail válido")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Curriculo { get; set; }
        public bool TemUsuario
        {
            get
            {
                return Usuario.Existe(Email);
            }
        }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(Professor item)
        {
            return ProfessorDAO.Incluir(item);
        }

        public static Professor Consultar(Guid? IdProfessor = null, string email = null)
        {
            return ProfessorDAO.Consultar(IdProfessor, email);
        }

        public static Professor ConsultarLogado()
        {
            string email = Usuario.Logado.Email;
            return Professor.Consultar(email: email);
        }

        public static List<Professor> Listar(string palavra = null)
        {
            return ProfessorDAO.Listar(palavra);
        }

        public static bool Alterar(Professor item)
        {
            return ProfessorDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdProfessor)
        {
            return ProfessorDAO.Desativar(IdProfessor);
        }
    }
}