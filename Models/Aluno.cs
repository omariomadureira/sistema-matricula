using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Aluno
    {
        public Guid IdAluno { get; set; }
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
        public long CPFSemFormatacao
        {
            get
            {
                if (!string.IsNullOrEmpty(CPF))
                {
                    string c = CPF.Replace(".", string.Empty).Replace("-", string.Empty).Trim();
                    return long.Parse(c);
                }
                return 0;
            }
        }
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

        public static bool Incluir(Aluno item)
        {
            return AlunoDAO.Incluir(item);
        }

        public static Aluno Consultar(Guid? IdAluno = null, string email = null)
        {
            return AlunoDAO.Consultar(IdAluno, email);
        }

        public static Aluno ConsultarLogado()
        {
            string email = Usuario.Logado.Email;
            return Aluno.Consultar(email: email);
        }

        public static List<Aluno> Listar(string palavra = null)
        {
            return AlunoDAO.Listar(palavra);
        }

        public static bool Alterar(Aluno item)
        {
            return AlunoDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdAluno)
        {
            return AlunoDAO.Desativar(IdAluno);
        }
    }
}