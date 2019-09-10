using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Grid
    {
        public Guid IdGrid { get; set; }
        public Class Class { get; set; }
        public Semester Semester { get; set; }
        public Teacher Teacher { get; set; }
        public int Weekday { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public string WeekdayName
        {
            get
            {
                return ((DayOfWeek)Weekday).ToString().ToUpper();
            }
        }

        public const string REGISTERED = "CADASTRADA";
        public const string RELEASED = "LIBERADA";
        public const string CANCELED = "CANCELADA";
        public const string FINISHED = "ENCERRADA";

        public static List<string> WeekdayList()
        {
            return new List<string>()
            {
                DayOfWeek.Monday.ToString(),
                DayOfWeek.Tuesday.ToString(),
                DayOfWeek.Wednesday.ToString(),
                DayOfWeek.Thursday.ToString(),
                DayOfWeek.Friday.ToString(),
                DayOfWeek.Saturday.ToString()
            };
        }

        public static List<string> TimeList(string period)
        {
            switch (period)
            {
                case Semester.PERIOD_MATUTINAL:
                    return new List<string>()
                    {
                        "7:00",
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                    };
                case Semester.PERIOD_VESPERTINE:
                    return new List<string>()
                    {
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                        "17:00",
                    };
                case Semester.PERIOD_NIGHTLY:
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

        public Pagination Pagination { get; set; }

        public static bool Add(Grid item)
        {
            return GridDAO.Add(item);
        }

        public static Grid Find(Guid id)
        {
            return GridDAO.Find(id);
        }

        public static List<Grid> List(Grid filters = null, bool? active = null)
        {
            return GridDAO.List(filters, active);
        }

        public static bool Update(Grid item)
        {
            return GridDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return GridDAO.Delete(id);
        }

        //TODO: Verificar a necessidade de toda essa lógica abaixo

        /*
        public static bool UpdateStatus(string status)
        {
            return GridDAO.UpdateStatus(status);
        }

        public static Grid[] Listar(Grid filtros = null, bool grade = false)
        {
            //TODO: Adicionar regra na procedure de disciplinas que não devem ser inseridas na grade, pois não atingiram a quantidade de alunos

            List<Grid> lista = new List<Grid>();

            try
            {
                if (filtros.Class != null && filtros.Semester != null)
                {
                    if (!grade)
                    {
                        return GridDAO.Listar(filtros).ToArray();
                    }

                    Class novoFiltro = new Class()
                    {
                        Course = new Course { IdCourse = filtros.Class.Course.IdCourse }
                    };

                    List<Class> disciplinas = Class.List(novoFiltro);

                    foreach (Class disciplina in disciplinas)
                    {
                        filtros.Class.IdClass = disciplina.IdClass;
                        List<Grid> definidas = GridDAO.Listar(filtros);

                        if (definidas.Count == 0)
                        {
                            definidas.Add(new Grid()
                            {
                                Class = disciplina,
                                Semester = filtros.Semester
                            });
                        }

                        lista.AddRange(definidas);
                    }

                    lista.Sort((a, b) => -1 * a.Semester.InitialDate.CompareTo(b.Semester.InitialDate));
                }
                else
                {
                    return GridDAO.Listar(filtros).ToArray();
                }
            }
            catch { }

            return lista.ToArray();
        }

        public static object ListarCourses(Guid? IdSemester = null, Guid? IdCourse = null, string StatusGrade = null, string PalavraChave = null)
        {
            //TODO: Adicionar regra na procedure de disciplinas que não devem ser inseridas na grade, pois não atingiram a quantidade de alunos
            return GridDAO.ListarCourses(IdSemester, IdCourse, StatusGrade, PalavraChave);
        }

        public static object ListarGrade(string StatusClass = null, Guid? IdCourse = null)
        {
            return GridDAO.ListarGrade(StatusClass, IdCourse);
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
        */
    }
}