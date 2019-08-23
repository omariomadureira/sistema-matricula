using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class DisciplinaSemestreDAO
    {
        public static bool Incluir(DisciplinaSemestre item)
        {
            try
            {
                DisciplinaSemestreData disciplina = new DisciplinaSemestreData
                {
                    IdDisciplinaSemestre = Guid.NewGuid(),
                    IdDisciplina = item.Disciplina.IdDisciplina,
                    IdSemestre = item.Semestre.IdSemestre,
                    IdProfessor = item.Professor.IdProfessor,
                    DiaSemana = item.DiaSemana,
                    Horario = item.Horario,
                    Status = "ATIVO",
                    CadastroData = DateTime.Now,
                    CadastroPor = Guid.Empty //TODO: Alterar para ID do usuário logado
                };

                Entities db = new Entities();
                db.DisciplinaSemestreData.Add(disciplina);
                db.SaveChanges();
                db.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DisciplinaSemestre Consultar(Guid IdDisciplinaSemestre)
        {
            try
            {
                Entities db = new Entities();

                DisciplinaSemestreData disciplina = db.DisciplinaSemestreData.FirstOrDefault(x => x.IdDisciplinaSemestre == IdDisciplinaSemestre);

                db.Dispose();

                return Converter(disciplina);
            }
            catch { }

            return null;
        }

        public static List<DisciplinaSemestre> Listar(DisciplinaSemestre filtros = null)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<DisciplinaSemestreData> query = db.DisciplinaSemestreData.Where(x => x.ExclusaoData == null);

                if (filtros != null)
                {
                    if (!string.IsNullOrWhiteSpace(filtros.DiaSemana))
                        query = query.Where(x => x.DiaSemana.ToLower().Contains(filtros.DiaSemana.ToLower()));

                    if (filtros.Disciplina != null && !Equals(Guid.Empty, filtros.Disciplina.IdDisciplina))
                        query = query.Where(x => x.IdDisciplina == filtros.Disciplina.IdDisciplina);

                    if (filtros.Semestre != null && !Equals(Guid.Empty, filtros.Semestre.IdSemestre))
                        query = query.Where(x => x.IdSemestre == filtros.Semestre.IdSemestre);

                    if (filtros.Professor != null && !Equals(Guid.Empty, filtros.Professor.IdProfessor))
                        query = query.Where(x => x.IdProfessor == filtros.Professor.IdProfessor);
                }

                List<DisciplinaSemestre> disciplinas = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return disciplinas;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Grade_ListarCursos_Result> ListarGrade(Guid? IdSemestre = null, Guid? IdCurso = null, string StatusGrade = null, string PalavraChave = null)
        {
            try
            {
                Entities db = new Entities();

                List<Grade_ListarCursos_Result> resultado = db.Grade_ListarCursos(IdSemestre, IdCurso, StatusGrade, PalavraChave).ToList();

                db.Dispose();

                return resultado;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(DisciplinaSemestre item)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaSemestreData disciplina = db.DisciplinaSemestreData.FirstOrDefault(x => x.IdDisciplinaSemestre == item.IdDisciplinaSemestre);

                if (disciplina != null)
                {
                    disciplina.IdDisciplina = item.Disciplina.IdDisciplina;
                    disciplina.IdSemestre = item.Semestre.IdSemestre;
                    disciplina.IdProfessor = item.Professor.IdProfessor;
                    disciplina.DiaSemana = item.DiaSemana;
                    disciplina.Horario = item.Horario;
                    disciplina.Status = item.Status;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdDisciplinaSemestre)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaSemestreData disciplina = db.DisciplinaSemestreData.FirstOrDefault(x => x.IdDisciplinaSemestre == IdDisciplinaSemestre);

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

        public static DisciplinaSemestre Converter(DisciplinaSemestreData a)
        {
            try
            {
                DisciplinaSemestre d = new DisciplinaSemestre
                {
                    IdDisciplinaSemestre = a.IdDisciplinaSemestre,
                    Disciplina = DisciplinaDAO.Consultar(a.IdDisciplina),
                    Semestre = SemestreDAO.Consultar(a.IdSemestre),
                    Professor = ProfessorDAO.Consultar(a.IdProfessor),
                    DiaSemana = a.DiaSemana,
                    Horario = a.Horario,
                    Status = a.Status,
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };

                if (d.Disciplina != null && d.Semestre != null && d.Professor != null)
                    return d;
            }
            catch { }

            return null;
        }
    }
}