using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Teacher
    {
        public Guid IdTeacher { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime Birthday { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [EmailAddress(ErrorMessage = "Preencha um e-mail válido")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Resume { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public bool HasUser
        {
            get
            {
                return User.Exists(Email);
            }
        }

        public Pagination Pagination { get; set; }

        public static bool Add(Teacher item)
        {
            return TeacherDAO.Add(item);
        }

        public static Teacher Find(Guid? id = null, string email = null)
        {
            return TeacherDAO.Find(id, email);
        }

        public static Teacher FindLoggedUser()
        {
            return TeacherDAO.Find(email: User.Logged.Email);
        }

        public static List<Teacher> List(Teacher filters = null)
        {
            return TeacherDAO.List(filters);
        }

        public static bool Update(Teacher item)
        {
            return TeacherDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return TeacherDAO.Delete(id);
        }
    }
}