using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Semester
    {
        public Guid IdSemester { get; set; }
        public string Period { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Display(Name = "Data de Início")]
        public DateTime InitialDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public string Name
        {
            get
            {
                return string.Format("{0} - {1}", InitialDate.ToString("Y").ToUpper(), Period);
            }
        }

        public bool IsActual
        {
            get
            {
                Semester item = SemesterDAO.Last();

                if (item == null)
                    return false;

                if (!DateTime.Equals(item.InitialDate, InitialDate))
                    return false;

                return true;
            }
        }

        public const string PERIOD_MATUTINAL = "MATUTINO";
        public const string PERIOD_VESPERTINE = "VESPERTINO";
        public const string PERIOD_NIGHTLY = "NOTURNO";

        public static List<string> Periods()
        {
            return new List<string>()
            {
                PERIOD_MATUTINAL,
                PERIOD_VESPERTINE,
                PERIOD_NIGHTLY
            };
        }

        public Pagination Pagination { get; set; }

        public static bool Add(Semester item)
        {
            return SemesterDAO.Add(item);
        }

        public static Semester Find(Guid id)
        {
            return SemesterDAO.Find(id);
        }

        public static List<Semester> List(Semester filters = null)
        {
            return SemesterDAO.List(filters);
        }

        public static bool Update(Semester item)
        {
            return SemesterDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return SemesterDAO.Delete(id);
        }
    }
}