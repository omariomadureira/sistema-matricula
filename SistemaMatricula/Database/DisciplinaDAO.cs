using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Database
{
    public class DisciplinaDAO
    {
        public Guid IdDisciplina { get; set; }
        public Curso Curso { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(Guid IdCurso, string Nome, string Descricao)
        {
            try
            {
                string[] row = { Guid.NewGuid().ToString(), IdCurso.ToString(), Nome, Descricao, DateTime.Now.ToString(), string.Empty };

                TextFile t = new TextFile("Disciplina");
                t.Add(row);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Disciplina Consultar(Guid IdDisciplina)
        {
            try
            {
                TextFile t = new TextFile("Disciplina");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdDisciplina)
                    {
                        DisciplinaDAO disciplina = new DisciplinaDAO
                        {
                            IdDisciplina = Guid.Parse(lines[i]),
                            Curso = CursoDAO.Consultar(Guid.Parse(lines[i + 1])),
                            Nome = lines[i + 2],
                            Descricao = lines[i + 3],
                            DataCadastro = DateTime.Parse(lines[i + 4])
                        };

                        if (disciplina.Curso == null)
                            break;

                        if (!string.IsNullOrEmpty(lines[i + 5]))
                            disciplina.DataExclusao = DateTime.Parse(lines[i + 5]);

                        return Converter(disciplina);
                    }
                }
            }
            catch { }

            return null;
        }

        public static List<Disciplina> Listar(string[] filtros = null)
        {
            try
            {
                TextFile t = new TextFile("Disciplina");
                string[] lines = t.Read();

                List<DisciplinaDAO> disciplinas = new List<DisciplinaDAO>();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    DisciplinaDAO disciplina = new DisciplinaDAO
                    {
                        IdDisciplina = Guid.Parse(lines[i]),
                        Curso = CursoDAO.Consultar(Guid.Parse(lines[i + 1])),
                        Nome = lines[i + 2],
                        Descricao = lines[i + 3],
                        DataCadastro = DateTime.Parse(lines[i + 4])
                    };

                    if (disciplina.Curso == null)
                        continue;

                    if (!string.IsNullOrEmpty(lines[i + 5]))
                        disciplina.DataExclusao = DateTime.Parse(lines[i + 5]);

                    disciplinas.Add(disciplina);
                }

                IEnumerable<DisciplinaDAO> query = disciplinas.Where(x => x.DataExclusao == null);

                if (filtros != null)
                {
                    if (!string.IsNullOrWhiteSpace(filtros[0]))
                        query = query.Where(x => x.Nome.ToLower().Contains(filtros[0].ToLower()) || x.Descricao.ToLower().Contains(filtros[0].ToLower()));

                    if (!string.IsNullOrWhiteSpace(filtros[1]) && Guid.TryParse(filtros[1], out _))
                        query = query.Where(x => x.Curso.IdCurso == Guid.Parse(filtros[1]));
                }

                return query.Select(x => Converter(x)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Guid IdDisciplina, Guid IdCurso, string Nome, string Descricao)
        {
            try
            {
                TextFile t = new TextFile("Disciplina");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdDisciplina)
                    {
                        lines[i + 1] = IdCurso.ToString();
                        lines[i + 2] = Nome;
                        lines[i + 3] = Descricao;

                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdDisciplina)
        {
            try
            {
                TextFile t = new TextFile("Disciplina");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdDisciplina)
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

        public static bool DesativarPorCurso(Guid IdCurso)
        {
            try
            {
                TextFile t = new TextFile("Disciplina");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 6)
                {
                    if (Guid.TryParse(lines[i + 1], out _) && Guid.Parse(lines[i + 1]) == IdCurso)
                        lines[i + 5] = DateTime.Now.ToString();
                }

                t.Update(lines);

                return true;
            }
            catch { }

            return false;
        }

        public static Disciplina Converter(DisciplinaDAO a)
        {
            try
            {
                Disciplina b = new Disciplina();
                b.IdDisciplina = a.IdDisciplina;
                b.Curso = a.Curso;
                b.Nome = a.Nome;
                b.Descricao = a.Descricao;
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