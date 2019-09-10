using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Class
    {
        public Guid IdClass { get; set; }
        public Course Course { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Description { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public Pagination Pagination { get; set; }

        public static bool Add(Class item)
        {
            return ClassDAO.Add(item);
        }

        public static Class Find(Guid id)
        {
            return ClassDAO.Find(id);
        }

        public static List<Class> List(Class filters = null)
        {
            return ClassDAO.List(filters);
        }

        public static bool Update(Class item)
        {
            return ClassDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return ClassDAO.Delete(id);
        }
    }
}