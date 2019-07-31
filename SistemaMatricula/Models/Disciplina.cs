using SistemaMatricula.Database;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class Disciplina
    {
        public Guid IdDisciplina { get; set; }
        public Curso Curso { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(Guid IdCurso, string Nome, string Descricao)
        {
            return DisciplinaDAO.Incluir(IdCurso, Nome, Descricao);
        }

        public static Disciplina Consultar(Guid IdDisciplina)
        {
            return DisciplinaDAO.Consultar(IdDisciplina);
        }

        public static List<Disciplina> Listar(string[] filtros = null)
        {
            return DisciplinaDAO.Listar(filtros);
        }

        public static bool Alterar(Guid IdDisciplina, Guid IdCurso, string Nome, string Descricao)
        {
            return DisciplinaDAO.Alterar(IdDisciplina, IdCurso, Nome, Descricao);
        }

        public static bool Desativar(Guid IdDisciplina)
        {
            return DisciplinaDAO.Desativar(IdDisciplina);
        }
    }
}