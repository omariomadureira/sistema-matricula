using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class DisciplinaSemestreAlunoDAO
    {
        public static bool Incluir(DisciplinaSemestreAluno item)
        {
            try
            {
                DisciplinaSemestreAlunoData DisciplinaSemestreAluno = new DisciplinaSemestreAlunoData
                {
                    IdDisciplinaSemestre = item.DisciplinaSemestre.IdDisciplinaSemestre,
                    IdAluno = item.Aluno.IdAluno,
                    Alternativa = item.Alternativa,
                    CadastroData = DateTime.Now,
                    CadastroPor = Guid.Empty //TODO: Alterar para ID do usuário logado
                };

                Entities db = new Entities();
                db.DisciplinaSemestreAlunoData.Add(DisciplinaSemestreAluno);
                db.SaveChanges();
                db.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DisciplinaSemestreAluno Consultar(DisciplinaSemestreAluno item)
        {
            try
            {
                Entities db = new Entities();

                DisciplinaSemestreAlunoData DisciplinaSemestreAluno = db.DisciplinaSemestreAlunoData.FirstOrDefault(x => x.IdDisciplinaSemestre == item.DisciplinaSemestre.IdDisciplinaSemestre && x.IdAluno == item.Aluno.IdAluno);

                db.Dispose();

                return Converter(DisciplinaSemestreAluno);
            }
            catch { }

            return null;
        }

        public static List<DisciplinaSemestreAluno> Listar(DisciplinaSemestreAluno filtro = null)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<DisciplinaSemestreAlunoData> query = db.DisciplinaSemestreAlunoData.Where(x => x.ExclusaoData == null);

                if (filtro != null)
                {
                    if (filtro.Alternativa != null)
                        query = query.Where(x => x.Alternativa == filtro.Alternativa);
                }

                List<DisciplinaSemestreAluno> DisciplinaSemestreAlunos = 
                    query
                        .Select(x => Converter(x))
                        .OrderBy(x => x.Alternativa)
                        .ThenBy(x => x.DisciplinaSemestre.DiaOrdem)
                        .ThenBy(x => x.DisciplinaSemestre.Horario)
                        .ToList();

                db.Dispose();

                return DisciplinaSemestreAlunos;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<DisciplinaSemestreAluno> ListarGrade(Guid IdCurso)
        {
            try
            {
                Semestre ativo = SemestreDAO.Ultimo();

                if (ativo != null)
                {
                    Entities db = new Entities();

                    List<DisciplinaSemestreData> grade =
                        db.DisciplinaSemestreData
                            .Where(x => x.ExclusaoData == null
                                && db.SemestreData
                                    .Where(y => y.InicioData == ativo.InicioData)
                                    .Select(y => y.IdSemestre)
                                    .Contains(x.IdSemestre)
                                && !db.DisciplinaSemestreAlunoData
                                    .Where(y => y.ExclusaoData == null && y.IdAluno == Guid.Empty)
                                    .Select(y => y.IdDisciplinaSemestre)
                                    .Contains(x.IdDisciplinaSemestre)
                                && db.DisciplinaData
                                    .Where(y => y.IdCurso == IdCurso)
                                    .Select(y => y.IdDisciplina)
                                    .Contains(x.IdDisciplina)
                                && x.Status == DisciplinaSemestre.DISCIPLINA_LIBERADA)
                            .ToList();

                    List<DisciplinaSemestreAluno> lista =
                        grade
                            .Select(x => new DisciplinaSemestreAluno() { DisciplinaSemestre = DisciplinaSemestreDAO.Converter(x) })
                            .OrderBy(x => x.DisciplinaSemestre.Semestre.Nome)
                            .ThenBy(x => x.DisciplinaSemestre.DiaOrdem)
                            .ThenBy(x => x.DisciplinaSemestre.Horario)
                            .ToList();

                    return lista;
                }
            }
            catch (Exception e) { }

            return null;
        }

        public static bool Alterar(DisciplinaSemestreAluno item)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaSemestreAlunoData DisciplinaSemestreAluno = db.DisciplinaSemestreAlunoData.FirstOrDefault(x => x.IdDisciplinaSemestre == item.DisciplinaSemestre.IdDisciplinaSemestre && x.IdAluno == item.Aluno.IdAluno);

                if (DisciplinaSemestreAluno != null)
                {
                    DisciplinaSemestreAluno.Alternativa = item.Alternativa;
                    DisciplinaSemestreAluno.ExclusaoData = null;
                    DisciplinaSemestreAluno.ExclusaoPor = null;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(DisciplinaSemestreAluno item)
        {
            try
            {
                Entities db = new Entities();
                DisciplinaSemestreAlunoData DisciplinaSemestreAluno = db.DisciplinaSemestreAlunoData.FirstOrDefault(x => x.IdDisciplinaSemestre == item.DisciplinaSemestre.IdDisciplinaSemestre && x.IdAluno == item.Aluno.IdAluno);

                if (DisciplinaSemestreAluno != null)
                {
                    DisciplinaSemestreAluno.ExclusaoData = DateTime.Now;
                    DisciplinaSemestreAluno.ExclusaoPor = Guid.Empty; //TODO: Alterar para ID do usuário logado

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static DisciplinaSemestreAluno Converter(DisciplinaSemestreAlunoData a)
        {
            try
            {
                return new DisciplinaSemestreAluno
                {
                    DisciplinaSemestre = DisciplinaSemestre.Consultar(a.IdDisciplinaSemestre),
                    Aluno = Aluno.Consultar(a.IdAluno),
                    Alternativa = a.Alternativa,
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