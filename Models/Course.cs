using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Course
    {
        public Guid IdCourse { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Preencha um número válido")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public int Credits { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public static bool Add(Course item)
        {
            return CourseDAO.Add(item);
        }

        public static Course Find(Guid id)
        {
            return CourseDAO.Find(id);
        }

        public static List<Course> List(string filter = null)
        {
            return CourseDAO.List(filter);
        }

        public static bool Update(Course item)
        {
            return CourseDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return CourseDAO.Delete(id);
        }
    }
}