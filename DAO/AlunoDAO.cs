using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class AlunoDAO
    {
        public static bool Incluir(Aluno item)
        {
            try
            {
                AlunoData Aluno = new AlunoData
                {
                    IdAluno = Guid.NewGuid(),
                    Nome = item.Nome,
                    DataNascimento = item.DataNascimento,
                    Email = item.Email,
                    CPF = item.CPF,
                    CadastroData = DateTime.Now,
                    CadastroPor = Usuario.Logado.IdUsuario
                };

                Entities db = new Entities();
                db.AlunoData.Add(Aluno);
                db.SaveChanges();
                db.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Aluno Consultar(Guid? IdAluno = null, string email = null)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<AlunoData> query = db.AlunoData;

                if (IdAluno != null)
                    query = query.Where(x => x.IdAluno == IdAluno);

                if (!string.IsNullOrEmpty(email))
                    query = query.Where(x => x.Email.Trim() == email.Trim());

                AlunoData aluno = query.FirstOrDefault();

                db.Dispose();

                return Converter(aluno);
            }
            catch { }

            return null;
        }

        public static List<Aluno> Listar(string palavra)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<AlunoData> query = db.AlunoData.Where(x => x.ExclusaoData == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()) || x.Email.ToLower().Contains(palavra.ToLower()));

                List<Aluno> Alunos = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return Alunos;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Aluno item)
        {
            try
            {
                Entities db = new Entities();
                AlunoData Aluno = db.AlunoData.FirstOrDefault(x => x.IdAluno == item.IdAluno);

                if (Aluno != null)
                {
                    Aluno.Nome = item.Nome;
                    Aluno.DataNascimento = item.DataNascimento;
                    Aluno.Email = item.Email;
                    Aluno.CPF = item.CPF;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdAluno)
        {
            try
            {
                Entities db = new Entities();
                AlunoData Aluno = db.AlunoData.FirstOrDefault(x => x.IdAluno == IdAluno);

                if (Aluno != null)
                {
                    Aluno.ExclusaoData = DateTime.Now;
                    Aluno.ExclusaoPor = Usuario.Logado.IdUsuario;
                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Aluno Converter(AlunoData a)
        {
            try
            {
                return new Aluno
                {
                    IdAluno = a.IdAluno,
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    Email = a.Email,
                    CPF = a.CPF,
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };
            }
            catch
            {
                return null;
            }
        }
    }
}