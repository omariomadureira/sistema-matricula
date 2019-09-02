using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class ProfessorDAO
    {
        public static bool Incluir(Professor item)
        {
            try
            {
                ProfessorData Professor = new ProfessorData
                {
                    IdProfessor = Guid.NewGuid(),
                    Nome = item.Nome,
                    DataNascimento = item.DataNascimento,
                    Email = item.Email,
                    CPF = item.CPF,
                    Curriculo = item.Curriculo,
                    CadastroData = DateTime.Now,
                    CadastroPor = Usuario.Logado.IdUsuario
                };

                Entities db = new Entities();
                db.ProfessorData.Add(Professor);
                db.SaveChanges();
                db.Dispose();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static Professor Consultar(Guid? IdProfessor = null, string email = null)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<ProfessorData> query = db.ProfessorData;

                if (IdProfessor != null)
                    query = query.Where(x => x.IdProfessor == IdProfessor);

                if (!string.IsNullOrEmpty(email))
                    query = query.Where(x => x.Email.Trim() == email.Trim());

                ProfessorData professor = query.FirstOrDefault();

                db.Dispose();

                return Converter(professor);
            }
            catch { }

            return null;
        }

        public static List<Professor> Listar(string palavra)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<ProfessorData> query = db.ProfessorData.Where(x => x.ExclusaoData == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()) || x.Email.ToLower().Contains(palavra.ToLower()));

                List<Professor> Professors = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return Professors;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Professor item)
        {
            try
            {
                Entities db = new Entities();
                ProfessorData Professor = db.ProfessorData.FirstOrDefault(x => x.IdProfessor == item.IdProfessor);

                if (Professor != null)
                {
                    Professor.Nome = item.Nome;
                    Professor.DataNascimento = item.DataNascimento;
                    Professor.Email = item.Email;
                    Professor.CPF = item.CPF;
                    Professor.Curriculo = item.Curriculo;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdProfessor)
        {
            try
            {
                Entities db = new Entities();
                ProfessorData Professor = db.ProfessorData.FirstOrDefault(x => x.IdProfessor == IdProfessor);

                if (Professor != null)
                {
                    Professor.ExclusaoData = DateTime.Now;
                    Professor.ExclusaoPor = Usuario.Logado.IdUsuario;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Professor Converter(ProfessorData a)
        {
            try
            {
                return new Professor
                {
                    IdProfessor = a.IdProfessor,
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    Email = a.Email,
                    CPF = a.CPF,
                    Curriculo = a.Curriculo,
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