﻿using System;
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
                    Status = DisciplinaSemestre.DISCIPLINA_CADASTRADA,
                    CadastroData = DateTime.Now,
                    CadastroPor = Usuario.Logado.IdUsuario
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

                    if (filtros.Disciplina != null && filtros.Disciplina.Curso != null &&
                        !Equals(Guid.Empty, filtros.Disciplina.Curso.IdCurso))
                        query = query.Where(x => db.DisciplinaData.Where(y => y.IdCurso == filtros.Disciplina.Curso.IdCurso).Select(y => y.IdDisciplina).Contains(x.IdDisciplina));

                    if (filtros.Semestre != null && !Equals(Guid.Empty, filtros.Semestre.IdSemestre))
                        query = query.Where(x => x.IdSemestre == filtros.Semestre.IdSemestre);

                    if (filtros.Semestre != null && !Equals(Guid.Empty, filtros.Semestre.InicioData))
                    {
                        query = query.Where(x =>
                            db.SemestreData
                                .Where(y => y.InicioData == filtros.Semestre.InicioData && y.ExclusaoData == null)
                                .Select(y => y.IdSemestre)
                                .Contains(x.IdSemestre));
                    }

                    if(!string.IsNullOrEmpty(filtros.Status))
                        query = query.Where(x => x.Status == filtros.Status);

                    if (filtros.Professor != null && !Equals(Guid.Empty, filtros.Professor.IdProfessor))
                        query = query.Where(x => x.IdProfessor == filtros.Professor.IdProfessor);
                }

                List<DisciplinaSemestre> disciplinas =
                    query
                        .Select(x => Converter(x))
                        .OrderBy(x => x.Semestre.Nome)
                        .ThenBy(x => x.Disciplina.Curso.Nome)
                        .ThenBy(x => x.DiaOrdem)
                        .ThenBy(x => x.Horario)
                        .ToList();

                db.Dispose();

                return disciplinas;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Grade_ListarCursos_Result> ListarCursos(Guid? IdSemestre = null, Guid? IdCurso = null, string StatusGrade = null, string PalavraChave = null)
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

        public static List<Grade_ListarDisciplinas_Result> ListarGrade(string StatusDisciplina = null, Guid? IdCurso = null)
        {
            try
            {
                Entities db = new Entities();

                //TODO: Verificar se essa procedure é necessária
                List<Grade_ListarDisciplinas_Result> resultado = db.Grade_ListarDisciplinas(StatusDisciplina, IdCurso).ToList();

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
                    if (item.Disciplina != null)
                        disciplina.IdDisciplina = item.Disciplina.IdDisciplina;

                    if (item.Semestre != null)
                        disciplina.IdSemestre = item.Semestre.IdSemestre;

                    if (item.Professor != null)
                        disciplina.IdProfessor = item.Professor.IdProfessor;

                    if (!string.IsNullOrEmpty(disciplina.DiaSemana))
                        disciplina.DiaSemana = item.DiaSemana;

                    if (!string.IsNullOrEmpty(disciplina.Horario))
                        disciplina.Horario = item.Horario;

                    if (!string.IsNullOrEmpty(disciplina.Status))
                        disciplina.Status = item.Status;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch (Exception e) { }

            return false;
        }

        public static bool AlterarStatus(string status)
        {
            try
            {
                Entities db = new Entities();
                List<Grade_ListarDisciplinasParaMatricula_Result> resultado = db.Grade_ListarDisciplinasParaMatricula().ToList();

                if (resultado.Count > 0)
                {
                    foreach (Grade_ListarDisciplinasParaMatricula_Result item in resultado)
                    {
                        DisciplinaSemestreData disciplina = db.DisciplinaSemestreData.FirstOrDefault(x => x.IdDisciplinaSemestre == item.IdDisciplinaSemestre);

                        if (disciplina != null)
                        {
                            disciplina.Status = status;
                            db.SaveChanges();
                        }
                    }

                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch (Exception e) { }

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
                    disciplina.ExclusaoPor = Usuario.Logado.IdUsuario;

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