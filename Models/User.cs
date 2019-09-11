using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Models
{
    public class User
    {
        public Guid IdUser { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public static User Logged
        {
            get
            {
                var id = (Guid)System.Web.HttpContext.Current.Cache["IdUser"];

                if (id == null)
                    return null;

                if (Guid.Equals(id, Guid.Empty))
                    return null;

                return UserDAO.Find(id);
            }
        }

        public const string ROLE_ADMINISTRATOR = "ADMINISTRADOR";
        public const string ROLE_STUDENT = "ALUNO";
        public const string ROLE_TEACHER = "PROFESSOR";

        public static List<string> Roles()
        {
            return new List<string>()
            {
                ROLE_ADMINISTRATOR,
                ROLE_STUDENT,
                ROLE_TEACHER
            };
        }

        public Pagination Pagination { get; set; }

        public static List<User> List(User filters = null)
        {
            return UserDAO.List(filters);
        }

        public static User Find(Guid? id = null, string login = null)
        {
            return UserDAO.Find(id, login);
        }

        public static bool Delete(Guid id)
        {
            return UserDAO.Delete(id);
        }

        /*
        public static bool IsActive(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                    return false;

                User item = UserDAO.Find(login: login);

                if (item == null)
                    throw new Exception("Usuário não encontrado");

                if (item.DeleteDate == null)
                    return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(login, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.User.IsActive", notes);
            }

            return false;
        }

        public static bool Exists(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                    return false;

                User item = UserDAO.Find(login: login);

                if (item != null)
                    return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(login, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.User.Exists", notes);
            }

            return false;
        }
        */
    }
}