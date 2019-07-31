using SistemaMatricula.Database;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class Aluno
    {
        public Guid IdAluno { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(string Nome, DateTime DataNascimento)
        {
            return AlunoDAO.Incluir(Nome, DataNascimento);
        }

        public static Aluno Consultar(Guid IdAluno)
        {
            return AlunoDAO.Consultar(IdAluno);
        }

        public static List<Aluno> Listar(string palavra = null)
        {
            return AlunoDAO.Listar(palavra);
        }

        public static bool Alterar(Guid IdAluno, string Nome, DateTime DataNascimento)
        {
            return AlunoDAO.Alterar(IdAluno, Nome, DataNascimento);
        }

        public static bool Desativar(Guid IdAluno)
        {
            return AlunoDAO.Desativar(IdAluno);
        }
    }
}