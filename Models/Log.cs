using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Log
    {
        public Guid IdLog { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public Guid IdUser { get; set; }
        public DateTime RegisterDate { get; set; }

        public const string TYPE_OPERATION = "Operação";
        public const string TYPE_ERROR = "Erro";

        public static List<string> Types()
        {
            return new List<string>() {
                TYPE_ERROR,
                TYPE_OPERATION
            };
        }

        public Pagination Pagination { get; set; }

        public static void Add(string type, string description, string notes)
        {
            LogDAO.Add(type, description, notes);
        }

        public static Log Find(Guid id)
        {
            return LogDAO.Find(id);
        }

        public static List<Log> List(Log filters = null)
        {
            return LogDAO.List(filters);
        }
    }
}