using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class Student
    {
        public Guid IdStudent { get; set; }
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

        public static bool Add(Student item)
        {
            return StudentDAO.Add(item);
        }

        public static Student Find(Guid? id = null, string email = null)
        {
            return StudentDAO.Find(id, email);
        }

        public static Student FindLoggedUser()
        {
            return StudentDAO.Find(email: User.Logged.Email);
        }

        public static List<Student> List(Student filters = null)
        {
            return StudentDAO.List(filters);
        }

        public static bool Update(Student item)
        {
            return StudentDAO.Update(item);
        }

        public static bool Delete(Guid id)
        {
            return StudentDAO.Delete(id);
        }
    }
}