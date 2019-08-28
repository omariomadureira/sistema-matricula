using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;

namespace SistemaMatricula.Models
{
    public class DisciplinaSemestre
    {
        public Guid IdDisciplinaSemestre { get; set; }
        public Disciplina Disciplina { get; set; }
        public Semestre Semestre { get; set; }
        public Professor Professor { get; set; }
        public string DiaSemana { get; set; }
        public string Horario { get; set; }
        public string Status { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public const string DISCIPLINA_CADASTRADA = "CADASTRADA";
        public const string DISCIPLINA_LIBERADA = "LIBERADA";
        public const string DISCIPLINA_CANCELADA = "CANCELADA";
        public const string DISCIPLINA_ENCERRADA = "ENCERRADA";

        public static bool Incluir(DisciplinaSemestre item)
        {
            return DisciplinaSemestreDAO.Incluir(item);
        }

        public static DisciplinaSemestre Consultar(Guid IdDisciplinaSemestre)
        {
            return DisciplinaSemestreDAO.Consultar(IdDisciplinaSemestre);
        }

        public static DisciplinaSemestre[] Listar(DisciplinaSemestre filtros = null, bool grade = false)
        {
            //TODO: Adicionar regra na procedure de disciplinas que não devem ser inseridas na grade, pois não atingiram a quantidade de alunos

            List<DisciplinaSemestre> lista = new List<DisciplinaSemestre>();

            try
            {
                if (filtros.Disciplina != null && filtros.Semestre != null)
                {
                    if (!grade)
                    {
                        return DisciplinaSemestreDAO.Listar(filtros).ToArray();
                    }

                    Disciplina novoFiltro = new Disciplina()
                    {
                        Curso = new Curso { IdCurso = filtros.Disciplina.Curso.IdCurso }
                    };

                    List<Disciplina> disciplinas = Disciplina.Listar(novoFiltro);

                    foreach (Disciplina disciplina in disciplinas)
                    {
                        filtros.Disciplina.IdDisciplina = disciplina.IdDisciplina;
                        List<DisciplinaSemestre> definidas = DisciplinaSemestreDAO.Listar(filtros);

                        if (definidas.Count == 0)
                        {
                            definidas.Add(new DisciplinaSemestre()
                            {
                                Disciplina = disciplina,
                                Semestre = filtros.Semestre
                            });
                        }

                        lista.AddRange(definidas);
                    }

                    lista.Sort((a, b) => -1 * a.Semestre.InicioData.CompareTo(b.Semestre.InicioData));
                }
                else
                {
                    return DisciplinaSemestreDAO.Listar(filtros).ToArray();
                }
            }
            catch { }

            return lista.ToArray();
        }

        public static object ListarCursos(Guid? IdSemestre = null, Guid? IdCurso = null, string StatusGrade = null, string PalavraChave = null)
        {
            //TODO: Adicionar regra na procedure de disciplinas que não devem ser inseridas na grade, pois não atingiram a quantidade de alunos
            return DisciplinaSemestreDAO.ListarCursos(IdSemestre, IdCurso, StatusGrade, PalavraChave);
        }

        public static object ListarGrade(string StatusDisciplina = null, Guid? IdCurso = null)
        {
            return DisciplinaSemestreDAO.ListarGrade(StatusDisciplina, IdCurso);
        }

        public static bool Alterar(DisciplinaSemestre item)
        {
            return DisciplinaSemestreDAO.Alterar(item);
        }

        public static bool AlterarStatus(string status)
        {
            return DisciplinaSemestreDAO.AlterarStatus(status);
        }

        public static bool Desativar(Guid IdDisciplinaSemestre)
        {
            return DisciplinaSemestreDAO.Desativar(IdDisciplinaSemestre);
        }

        public static List<string> Dias()
        {
            return new List<string>()
            {
                "SEGUNDA-FEIRA",
                "TERÇA-FEIRA",
                "QUARTA-FEIRA",
                "QUINTA-FEIRA",
                "SEXTA-FEIRA",
                "SÁBADO",
                "DOMINGO"
            };
        }

        public static List<string> Horarios(string periodo)
        {
            switch (periodo)
            {
                case Semestre.PERIODO_MATUTINO:
                    return new List<string>()
                    {
                        "7:00",
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                    };
                case Semestre.PERIODO_VESPERTINO:
                    return new List<string>()
                    {
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                        "17:00",
                    };
                case Semestre.PERIODO_NOTURNO:
                    return new List<string>()
                    {
                        "19:00",
                        "20:00",
                        "21:00",
                        "22:00",
                        "23:00"
                    };
                default:
                    return new List<string>()
                    {
                        "7:00",
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                        "17:00",
                        "19:00",
                        "20:00",
                        "21:00",
                        "22:00",
                        "23:00"
                    };
            }
        }

        public static List<string> StatusGrade()
        {
            return new List<string>()
            {
                "PENDENTE",
                "COMPLETO",
                "INCOMPLETO",
                "ENCERRADO",
                "ERRO"
            };
        }
    }
}