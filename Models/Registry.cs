using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Registry
    {
        public Grid Grid { get; set; }
        public Student Student { get; set; }
        public bool? Alternative { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public Pagination Pagination { get; set; }

        public static bool Add(Registry item)
        {
            return RegistryDAO.Add(item);
        }

        public static Registry Find(Guid id)
        {
            return RegistryDAO.Find(id);
        }

        public static List<Registry> List(Registry filters = null)
        {
            return RegistryDAO.List(filters);
        }

        public static List<Registry> GridList(Guid idStudent, Guid idCourse)
        {
            return RegistryDAO.GridList(idStudent, idCourse);
        }

        public static bool Delete(Guid id)
        {
            return RegistryDAO.Delete(id);
        }

        public static bool DeleteByGrid(Guid id)
        {
            return RegistryDAO.DeleteByGrid(id);
        }

        public static void AcionarSistemaCobranca()
        {
            //TODO: Simular acionamento do sistema de cobrança
        }
    }
}