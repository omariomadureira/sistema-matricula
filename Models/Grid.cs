using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using SistemaMatricula.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Grid
    {
        public Guid IdGrid { get; set; }
        public Class Class { get; set; }
        public Semester Semester { get; set; }
        public Teacher Teacher { get; set; }
        [Display(Name = "Dia da Semana")]
        public int Weekday { get; set; }
        public string Time { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Range(1, double.MaxValue, ErrorMessage = "Preencha um valor válido")]
        [Display(Name = "Preço da Matrícula")]
        public double Price { get; set; }
        public string Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public string WeekdayName
        {
            get
            {
                return new System.Globalization.CultureInfo("pt-BR")
                    .DateTimeFormat.GetDayName((DayOfWeek)Weekday).ToUpper();
            }
        }

        public static List<Grid> WeekdayList()
        {
            List<Grid> list = new List<Grid>();

            for (int day = 1; day < 7; day++)
            {
                list.Add(new Grid() { Weekday = day });
            }

            return list;
        }

        public const string REGISTERED = "CADASTRADA";
        public const string RELEASED = "LIBERADA";
        public const string CANCELED = "CANCELADA";
        public const string FINISHED = "ENCERRADA";

        public static List<string> StatusList()
        {
            return new List<string>()
            {
                REGISTERED,
                RELEASED,
                CANCELED,
                FINISHED
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

        public int Registries
        {
            get
            {
                if (string.IsNullOrEmpty(Status))
                    return 0;

                if (Status == REGISTERED || Status == CANCELED)
                    return 0;

                Registry filters = new Registry()
                {
                    Grid = new Grid() { IdGrid = IdGrid }
                };

                var registries = Registry.List(filters);

                if (registries == null)
                    return 0;

                return registries.Count;
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

        public static List<Grid> List(Grid filters = null, bool actual = false)
        {
            return GridDAO.List(filters, actual);
        }

        public static bool Update(Grid item)
        {
            return GridDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return GridDAO.Delete(id);
        }

        public static bool Release()
        {
            try
            {
                Grid filters = new Grid()
                {
                    Status = REGISTERED
                };

                var list = List(filters);

                if (list == null)
                    throw new Exception("As grades não foram listadas");

                foreach (Grid item in list)
                {
                    item.Status = RELEASED;

                    var update = Update(item);

                    if (update == false)
                        throw new Exception(string.Format("Grade {0} não foi liberada", item.IdGrid));

                    continue;
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.Grid.Release", notes);
            }

            return false;
        }

        public static bool Close()
        {
            try
            {
                Grid filters = new Grid()
                {
                    Status = RELEASED
                };

                var list = List(filters);

                if (list == null)
                    throw new Exception("As grades não foram listadas");

                foreach (Grid item in list)
                {
                    if (item.Registries < 3)
                        item.Status = CANCELED;
                    else
                        item.Status = FINISHED;

                    var update = Update(item);

                    if (update == false)
                        throw new Exception(string.Format("Grade {0} não foi encerrada", item.IdGrid));

                    continue;
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.Grid.Close", notes);
            }

            return false;
        }

        public static bool Copy(Guid idOrigin, Guid idDestiny)
        {
            try
            {
                if (Equals(idOrigin, Guid.Empty))
                    throw new Exception("Parâmetro 'idOrigin' inválido");

                if (Equals(idDestiny, Guid.Empty))
                    throw new Exception("Parâmetro 'idDestiny' inválido");

                Grid filters = new Grid()
                {
                    Semester = new Semester() { IdSemester = idOrigin },
                    Status = FINISHED
                };

                var list = List(filters);

                if (list == null)
                    throw new Exception("As grades não foram listadas");

                var semester = Semester.Find(idDestiny);

                if (semester == null)
                    throw new Exception(string.Format("Semestre {0} não existe", idDestiny));

                foreach (Grid item in list)
                {
                    item.Semester = semester;
                    item.Status = REGISTERED;
                }

                var insert = GridDAO.Add(list);

                if (insert == false)
                    throw new Exception("A grade não foi copiada");

                return true;
            }
            catch (Exception e)
            {
                object[] parameters = { idOrigin, idDestiny };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.Grid.Copy", notes);
            }

            return false;
        }
    }
}