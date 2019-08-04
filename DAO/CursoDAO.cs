using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class CursoDAO
    {
        public static bool Incluir(Curso item)
        {
            try
            {
                CursoData curso = new CursoData
                {
                    IdCurso = Guid.NewGuid(),
                    Nome = item.Nome,
                    Descricao = item.Descricao,
                    Creditos = item.Creditos,
                    CadastroData = DateTime.Now,
                    CadastroPor = Guid.Empty //TODO: Alterar para ID do usuário logado
                };

                SistemaMatriculaEntities db = new SistemaMatriculaEntities();
                db.CursoData.Add(curso);
                db.SaveChanges();
                db.Dispose();

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
                CursoData curso = new SistemaMatriculaEntities().CursoData.FirstOrDefault(x => x.IdCurso == IdCurso);
                return Converter(curso);
            }
            catch { }

            return null;
        }

        public static List<Curso> Listar(string palavra)
        {
            try
            {
                SistemaMatriculaEntities db = new SistemaMatriculaEntities();

                IEnumerable<CursoData> query = db.CursoData.Where(x => x.ExclusaoData == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Nome.ToLower().Contains(palavra.ToLower()) || x.Descricao.ToLower().Contains(palavra.ToLower()));

                List<Curso> cursos = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return cursos;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Curso item)
        {
            try
            {
                SistemaMatriculaEntities db = new SistemaMatriculaEntities();
                CursoData curso = db.CursoData.FirstOrDefault(x => x.IdCurso == item.IdCurso);

                if (curso != null)
                {
                    curso.Nome = item.Nome;
                    curso.Descricao = item.Descricao;
                    curso.Creditos = item.Creditos;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdCurso)
        {
            try
            {
                SistemaMatriculaEntities db = new SistemaMatriculaEntities();
                CursoData curso = db.CursoData.FirstOrDefault(x => x.IdCurso == IdCurso);

                if (curso != null)
                {
                    curso.ExclusaoData = DateTime.Now;
                    curso.ExclusaoPor = Guid.Empty; //TODO: Alterar para ID do usuário logado

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Curso Converter(CursoData a)
        {
            try
            {
                return new Curso
                {
                    IdCurso = a.IdCurso,
                    Nome = a.Nome,
                    Descricao = a.Descricao,
                    Creditos = a.Creditos,
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