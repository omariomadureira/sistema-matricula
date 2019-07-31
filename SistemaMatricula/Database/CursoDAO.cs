using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

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

        public static bool Incluir(string Nome, string Descricao, int Creditos)
        {
            try
            {
                string[] row = { Guid.NewGuid().ToString(), Nome, Descricao, Creditos.ToString(), DateTime.Now.ToString(), string.Empty };

                TextFile t = new TextFile("Curso");
                t.Add(row);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Curso Consultar(Guid IdCurso)
        {
            try
            {
                TextFile t = new TextFile("Curso");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdCurso)
                    {
                        CursoDAO curso = new CursoDAO
                        {
                            IdCurso = Guid.Parse(lines[i]),
                            Nome = lines[i + 1],
                            Descricao = lines[i + 2],
                            Creditos = int.Parse(lines[i + 3]),
                            DataCadastro = DateTime.Parse(lines[i + 4])
                        };

                        if (!string.IsNullOrEmpty(lines[i + 5]))
                            curso.DataExclusao = DateTime.Parse(lines[i + 5]);

                        return Converter(curso);
                    }
                }
            }
            catch { }

            return null;
        }

        public static List<Curso> Listar(string palavra)
        {
            try
            {
                TextFile t = new TextFile("Curso");
                string[] lines = t.Read();

                List<CursoDAO> cursos = new List<CursoDAO>();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    CursoDAO curso = new CursoDAO
                    {
                        IdCurso = Guid.Parse(lines[i]),
                        Nome = lines[i + 1],
                        Descricao = lines[i + 2],
                        Creditos = int.Parse(lines[i + 3]),
                        DataCadastro = DateTime.Parse(lines[i + 4])
                    };

                    if (!string.IsNullOrEmpty(lines[i + 5]))
                        curso.DataExclusao = DateTime.Parse(lines[i + 5]);

                    cursos.Add(curso);
                }

                IEnumerable<CursoDAO> query = cursos.Where(x => x.DataExclusao == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()) || x.Descricao.ToLower().Contains(palavra.ToLower()));

                return query.Select(x => Converter(x)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Guid IdCurso, string Nome, string Descricao, int Creditos)
        {
            try
            {
                TextFile t = new TextFile("Curso");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdCurso)
                    {
                        lines[i + 1] = Nome;
                        lines[i + 2] = Descricao;
                        lines[i + 3] = Creditos.ToString();

                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdCurso)
        {
            try
            {
                TextFile t = new TextFile("Curso");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdCurso)
                    {
                        lines[i + 5] = DateTime.Now.ToString();
                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static Curso Converter(CursoDAO a)
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