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

                List<DisciplinaSemestreAluno> DisciplinaSemestreAlunos = query.Select(x => Converter(x)).ToList();

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
                List<Grade_ListarDisciplinas_Result> disciplinas = DisciplinaSemestreDAO.ListarGrade(DisciplinaSemestre.DISCIPLINA_LIBERADA, IdCurso);

                if (disciplinas.Count > 0)
                {
                    List<DisciplinaSemestreAluno> lista = new List<DisciplinaSemestreAluno>();

                    foreach (Grade_ListarDisciplinas_Result item in disciplinas)
                    {
                        DisciplinaSemestreAluno d = new DisciplinaSemestreAluno()
                        {
                            DisciplinaSemestre = DisciplinaSemestreDAO.Consultar(item.IdDisciplinaSemestre)
                        };
                        lista.Add(d);
                    }

                    return lista;
                }
            }
            catch { }

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