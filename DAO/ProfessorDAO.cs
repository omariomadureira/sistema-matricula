using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class ProfessorDAO
    {
        public Guid IdProfessor { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(string Nome, string Descricao)
        {
            try
            {
                string[] row = { Guid.NewGuid().ToString(), Nome, Descricao, DateTime.Now.ToString(), string.Empty };

                TextFile t = new TextFile("Professor");
                t.Add(row);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Professor Consultar(Guid IdProfessor)
        {
            try
            {
                TextFile t = new TextFile("Professor");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdProfessor)
                    {
                        ProfessorDAO Professor = new ProfessorDAO
                        {
                            IdProfessor = Guid.Parse(lines[i]),
                            Nome = lines[i + 1],
                            Descricao = lines[i + 2],
                            DataCadastro = DateTime.Parse(lines[i + 3])
                        };

                        if (!string.IsNullOrEmpty(lines[i + 4]))
                            Professor.DataExclusao = DateTime.Parse(lines[i + 4]);

                        return Converter(Professor);
                    }
                }
            }
            catch { }

            return null;
        }

        public static List<Professor> Listar(string palavra)
        {
            try
            {
                TextFile t = new TextFile("Professor");
                string[] lines = t.Read();

                List<ProfessorDAO> Professores = new List<ProfessorDAO>();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    ProfessorDAO Professor = new ProfessorDAO
                    {
                        IdProfessor = Guid.Parse(lines[i]),
                        Nome = lines[i + 1],
                        Descricao = lines[i + 2],
                        DataCadastro = DateTime.Parse(lines[i + 3])
                    };

                    if (!string.IsNullOrEmpty(lines[i + 4]))
                        Professor.DataExclusao = DateTime.Parse(lines[i + 4]);

                    Professores.Add(Professor);
                }

                IEnumerable<ProfessorDAO> query = Professores.Where(x => x.DataExclusao == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()) || x.Descricao.ToLower().Contains(palavra.ToLower()));

                return query.Select(x => Converter(x)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Guid IdProfessor, string Nome, string Descricao)
        {
            try
            {
                TextFile t = new TextFile("Professor");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdProfessor)
                    {
                        lines[i + 1] = Nome;
                        lines[i + 2] = Descricao;

                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdProfessor)
        {
            try
            {
                TextFile t = new TextFile("Professor");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdProfessor)
                    {
                        lines[i + 4] = DateTime.Now.ToString();
                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static Professor Converter(ProfessorDAO a)
        {
            try
            {
                Professor b = new Professor();
                b.IdProfessor = a.IdProfessor;
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