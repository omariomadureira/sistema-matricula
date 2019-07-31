using SistemaMatricula.Database;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class Curso
    {
        public Guid IdCurso { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Creditos { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(string Nome, string Descricao, int Creditos)
        {
            return CursoDAO.Incluir(Nome, Descricao, Creditos);
        }

        public static Curso Consultar(Guid IdCurso)
        {
            return CursoDAO.Consultar(IdCurso);
        }

        public static List<Curso> Listar(string palavra = null)
        {
            return CursoDAO.Listar(palavra);
        }

        public static bool Alterar(Guid IdCurso, string Nome, string Descricao, int Creditos)
        {
            return CursoDAO.Alterar(IdCurso, Nome, Descricao, Creditos);
        }

        public static bool Desativar(Guid IdCurso)
        {
            return CursoDAO.Desativar(IdCurso);
        }
    }
}