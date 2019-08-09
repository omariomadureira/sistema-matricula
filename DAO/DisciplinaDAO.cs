using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class DisciplinaDAO
    {
        public static bool Incluir(Disciplina item)
        {
            try
            {
                DisciplinaData disciplina = new DisciplinaData
                {
                    IdDisciplina = Guid.NewGuid(),
                    Nome = item.Nome,
                    Descricao = item.Descricao,
                    IdCurso = item.Curso.IdCurso,
                    CadastroData = DateTime.Now,
                    CadastroPor = Guid.Empty //TODO: Alterar para ID do usuário logado
                };

                Entities db = new Entities();
                db.DisciplinaData.Add(disciplina);
                db.SaveChanges();
                db.Dispose();

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
                Entities db = new Entities();

                DisciplinaData disciplina = db.DisciplinaData.FirstOrDefault(x => x.IdDisciplina == IdDisciplina);

                db.Dispose();

                return Converter(disciplina);
            }
            catch { }

            return null;
        }

        public static List<Disciplina> Listar(Disciplina filtros)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<DisciplinaData> query = db.DisciplinaData.Where(x => x.ExclusaoData == null);

                if (filtros != null)
                {
                    if (!string.IsNullOrWhiteSpace(filtros.Nome))
                        query = query.Where(x => x.Nome.ToLower().Contains(filtros.Nome.ToLower()) || x.Descricao.ToLower().Contains(filtros.Nome.ToLower()));

                    if (filtros.Curso != null && !Equals(Guid.Empty, filtros.Curso.IdCurso))
                        query = query.Where(x => x.IdCurso == filtros.Curso.IdCurso);
                }

                List<Disciplina> disciplinas = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return disciplinas;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Disciplina item)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaData disciplina = db.DisciplinaData.FirstOrDefault(x => x.IdDisciplina == item.IdDisciplina);

                if (disciplina != null)
                {
                    disciplina.Nome = item.Nome;
                    disciplina.Descricao = item.Descricao;
                    disciplina.IdCurso = item.Curso.IdCurso;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdDisciplina)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaData disciplina = db.DisciplinaData.FirstOrDefault(x => x.IdDisciplina == IdDisciplina);

                if (disciplina != null)
                {
                    disciplina.ExclusaoData = DateTime.Now;
                    disciplina.ExclusaoPor = Guid.Empty;  //TODO: Alterar para ID do usuário logado

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Disciplina Converter(DisciplinaData a)
        {
            try
            {
                Disciplina d = new Disciplina
                {
                    IdDisciplina = a.IdDisciplina,
                    Curso = CursoDAO.Consultar(a.IdCurso),
                    Nome = a.Nome,
                    Descricao = a.Descricao,
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };

                if (d.Curso != null)
                    return d;
            }
            catch { }

            return null;
        }
    }
}