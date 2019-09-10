using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class UserDAO
    {
        public static List<User> List(User filters = null)
        {
            try
            {
                List<User> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<AspNetUsers> query = db.AspNetUsers.Where(x => x.DeleteDate == null);

                    if (filters != null && string.IsNullOrWhiteSpace(filters.Email) == false)
                    {
                        filters.Email = filters.Email.Trim().ToLower();

                        query = query
                            .Where(x =>
                                x.Email.ToLower().Contains(filters.Email)
                                || x.UserName.ToLower().Contains(filters.Email));
                    }

                    query = query.OrderByDescending(x => x.RegisterDate);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<User>();

                        int skip = (filters.Pagination.Actual - 1) * filters.Pagination.ItensPerPage;

                        query = query.Skip(skip).Take(filters.Pagination.ItensPerPage);
                    }

                    list = query
                        .Select(x => Convert(x))
                        .ToList();
                }

                return list;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(filters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.UserDAO.List", notes);
            }

            return null;
        }

        public static User Find(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                    throw new Exception("Parâmetro vazio");

                User item = null;

                using (Entities db = new Entities())
                {
                    AspNetUsers row = db.AspNetUsers.FirstOrDefault(x => x.UserName.Trim() == login.Trim());

                    if (row == null)
                        throw new Exception("Usuário não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(login, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.UserDAO.Find", notes);
            }

            return null;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                using (Entities db = new Entities())
                {
                    AspNetUsers row = db.AspNetUsers.FirstOrDefault(x => x.Id == id.ToString());

                    if (row == null)
                        throw new Exception("Usuário não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.UserDAO.Delete", notes);
            }

            return false;
        }

        public static User Convert(AspNetUsers item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                return new User
                {
                    IdUser = Guid.Parse(item.Id),
                    Login = item.UserName,
                    Email = item.Email,
                    RegisterDate = item.RegisterDate,
                    DeleteDate = item.DeleteDate,
                    DeleteBy = item.DeleteBy
                };
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.UserDAO.Convert", notes);
            }

            return null;
        }
    }
}