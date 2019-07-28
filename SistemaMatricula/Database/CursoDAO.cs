using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Models;

namespace SistemaMatricula.Database
{
    public class CursoDAO
    {
        public Guid IdCurso { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Creditos { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }
        public List<CursoDAO> Cursos { get; set; }

        public bool Incluir(string Nome, string Descricao, int Creditos)
        {
            try
            {
                CursoDAO a = new CursoDAO();
                a.IdCurso = Guid.NewGuid();
                a.Nome = Nome;
                a.Descricao = Descricao;
                a.Creditos = Creditos;
                a.DataCadastro = DateTime.Now;
                Cursos.Add(a);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Curso Consultar(Guid Id)
        {
            try
            {
                return Cursos.Where(x => x.IdCurso == Id).Select(x => Converter(x)).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<Curso> Listar()
        {
            try
            {
                CursoDAO a = new CursoDAO();
                a.IdCurso = Guid.NewGuid();
                a.Nome = "Curso A";
                a.Descricao = "Descrição do Curso A. Lorem ipsum lorem ipsum lorem ipsum lorem ipsum.";
                a.Creditos = 10;
                a.DataCadastro = DateTime.Now;

                CursoDAO b = new CursoDAO();
                b.IdCurso = Guid.NewGuid();
                b.Nome = "Curso B";
                b.Descricao = "Descrição do Curso B. Lorem ipsum lorem ipsum lorem ipsum lorem ipsum.";
                b.Creditos = 20;
                b.DataCadastro = DateTime.Now;

                CursoDAO c = new CursoDAO();
                c.IdCurso = Guid.NewGuid();
                c.Nome = "Curso A";
                c.Descricao = "Descrição do Curso A. Lorem ipsum lorem ipsum lorem ipsum lorem ipsum.";
                c.Creditos = 10;
                c.DataCadastro = DateTime.Now;

                Cursos = new List<CursoDAO>();

                Cursos.Add(a);
                Cursos.Add(b);
                Cursos.Add(c);

                return Cursos.Where(x => x.DataExclusao == null).Select(x => Converter(x)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Desativar(Guid Id)
        {
            try
            {
                Cursos = Cursos.Where(x => x.IdCurso == Id)
                               .Select(x => { x.DataExclusao = DateTime.Now; return x; })
                               .ToList();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Alterar(Guid Id, string Nome, string Descricao, int Creditos)
        {
            try
            {
                Cursos = Cursos.Where(x => x.IdCurso == Id)
                               .Select(x => { x.Nome = Nome; x.Descricao = Descricao; x.Creditos = Creditos; return x; })
                               .ToList();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Curso Converter(CursoDAO a)
        {
            try
            {
                Curso b = new Curso();
                b.IdCurso = a.IdCurso;
                b.Nome = a.Nome;
                b.Descricao = a.Descricao;
                b.Creditos = a.Creditos;
                b.DataCadastro = a.DataCadastro;
                b.DataExclusao = a.DataExclusao;

                return b;
            }
            catch
            {
                return null;
            }
        }
    }
}