using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Database
{
    public class AlunoDAO
    {
        public Guid IdAluno { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataExclusao { get; set; }

        public static bool Incluir(string Nome, DateTime DataNascimento)
        {
            try
            {
                string[] row = { Guid.NewGuid().ToString(), Nome, DataNascimento.ToString(), DateTime.Now.ToString(), string.Empty };

                TextFile t = new TextFile("Aluno");
                t.Add(row);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Aluno Consultar(Guid IdAluno)
        {
            try
            {
                TextFile t = new TextFile("Aluno");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdAluno)
                    {
                        AlunoDAO Aluno = new AlunoDAO
                        {
                            IdAluno = Guid.Parse(lines[i]),
                            Nome = lines[i + 1],
                            DataNascimento = DateTime.Parse(lines[i + 2]),
                            DataCadastro = DateTime.Parse(lines[i + 3])
                        };

                        if (!string.IsNullOrEmpty(lines[i + 4]))
                            Aluno.DataExclusao = DateTime.Parse(lines[i + 4]);

                        return Converter(Aluno);
                    }
                }
            }
            catch { }

            return null;
        }

        public static List<Aluno> Listar(string palavra)
        {
            try
            {
                TextFile t = new TextFile("Aluno");
                string[] lines = t.Read();

                List<AlunoDAO> Alunos = new List<AlunoDAO>();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    AlunoDAO Aluno = new AlunoDAO
                    {
                        IdAluno = Guid.Parse(lines[i]),
                        Nome = lines[i + 1],
                        DataNascimento = DateTime.Parse(lines[i + 2]),
                        DataCadastro = DateTime.Parse(lines[i + 3])
                    };

                    if (!string.IsNullOrEmpty(lines[i + 4]))
                        Aluno.DataExclusao = DateTime.Parse(lines[i + 4]);

                    Alunos.Add(Aluno);
                }

                IEnumerable<AlunoDAO> query = Alunos.Where(x => x.DataExclusao == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()));

                return query.Select(x => Converter(x)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Guid IdAluno, string Nome, DateTime DataNascimento)
        {
            try
            {
                TextFile t = new TextFile("Aluno");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdAluno)
                    {
                        lines[i + 1] = Nome;
                        lines[i + 2] = DataNascimento.ToString();

                        t.Update(lines);

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdAluno)
        {
            try
            {
                TextFile t = new TextFile("Aluno");
                string[] lines = t.Read();

                for (int i = 0; i < lines.Length; i += 5)
                {
                    if (Guid.TryParse(lines[i], out _) && Guid.Parse(lines[i]) == IdAluno)
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

        public static Aluno Converter(AlunoDAO a)
        {
            try
            {
                Aluno b = new Aluno();
                b.IdAluno = a.IdAluno;
                b.Nome = a.Nome;
                b.DataNascimento = a.DataNascimento;
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