using SistemaMatricula.Database;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class Professor
    {
        public Guid IdProfessor { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(string Nome, string Descricao)
        {
            return ProfessorDAO.Incluir(Nome, Descricao);
        }

        public static Professor Consultar(Guid IdProfessor)
        {
            return ProfessorDAO.Consultar(IdProfessor);
        }

        public static List<Professor> Listar(string palavra = null)
        {
            return ProfessorDAO.Listar(palavra);
        }

        public static bool Alterar(Guid IdProfessor, string Nome, string Descricao)
        {
            return ProfessorDAO.Alterar(IdProfessor, Nome, Descricao);
        }

        public static bool Desativar(Guid IdProfessor)
        {
            return ProfessorDAO.Desativar(IdProfessor);
        }
    }
}