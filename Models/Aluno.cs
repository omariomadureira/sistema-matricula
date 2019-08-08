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
        public string Nome { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataNascimento { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [EmailAddress(ErrorMessage = "Preencha um e-mail válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Range(1, 11, ErrorMessage = "Preencha um número válido")]
        public int CPF { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(Aluno item)
        {
            return AlunoDAO.Incluir(item);
        }

        public static Aluno Consultar(Guid IdAluno)
        {
            return AlunoDAO.Consultar(IdAluno);
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