using System;
using System.Collections.Generic;
using SistemaMatricula.Database;

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
        public CursoDAO Cursos { get; set; }

        public Curso()
        {
            Cursos = new CursoDAO();
        }

        public bool Incluir(string Nome, string Descricao, int Creditos)
        {
            return Cursos.Incluir(Nome, Descricao, Creditos);
        }

        public Curso Consultar(Guid Id)
        {
            return Cursos.Consultar(Id);
        }

        public List<Curso> Listar()
        {
            return Cursos.Listar();
        }

        public bool Desativar(Guid Id)
        {
            return Cursos.Desativar(Id);
        }

        public bool Alterar(Guid Id, string Nome, string Descricao, int Creditos)
        {
            return Cursos.Alterar(Id, Nome, Descricao, Creditos);
        }
    }
}