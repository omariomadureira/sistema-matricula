using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class StudentDAO
    {
        public static bool Add(Student item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                StudentData row = new StudentData
                {
                    IdStudent = Guid.NewGuid(),
                    Name = item.Name,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    CPF = item.CPF,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.StudentData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Add", notes);
            }

            return false;
        }

        public static Student Find(Guid? id = null, string email = null)
        {
            try
            {
                if (id == null && string.IsNullOrEmpty(email))
                    throw new Exception("Parâmetros vazios");

                if (id != null && Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro 'id' inválido");

                Student item = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<StudentData> query = db.StudentData;

                    if (id != null)
                        query = query.Where(x => x.IdStudent == id);

                    if (!string.IsNullOrEmpty(email))
                        query = query.Where(x => x.Email.Trim() == email.Trim());

                    StudentData row = query.FirstOrDefault();

                    if (row == null)
                        throw new Exception("Aluno não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                object[] parameters = { id, email };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Find", notes);
            }

            return null;
        }

        public static List<Student> List(Student filters = null)
        {
            try
            {
                List<Student> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<StudentData> query = db.StudentData.Where(x => x.DeleteDate == null);

                    if (filters != null && !string.IsNullOrWhiteSpace(filters.Name))
                    {
                        filters.Name = filters.Name.Trim().ToLower();

                        query = query
                            .Where(x =>
                                x.Name.ToLower().Contains(filters.Name)
                                || x.Email.ToLower().Contains(filters.Name));
                    }

                    query = query.OrderByDescending(x => x.RegisterDate);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<Student>();

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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Student item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    StudentData row = db.StudentData.FirstOrDefault(x => x.IdStudent == item.IdStudent);

                    if (row == null)
                        throw new Exception("Aluno não encontrado");

                    row.Name = item.Name;
                    row.Birthday = item.Birthday;
                    row.Email = item.Email;
                    row.CPF = item.CPF;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Update", notes);
            }

            return false;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    StudentData row = db.StudentData.FirstOrDefault(x => x.IdStudent == id);

                    if (row == null)
                        throw new Exception("Aluno não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();

                    User user = UserDAO.Find(row.Email);

                    if (user != null)
                    {
                        var deleted = UserDAO.Delete(user.IdUser);

                        if (deleted == false)
                            throw new Exception("Usuário não deletado");
                    }
                }
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Delete", notes);
            }

            return false;
        }

        public static Student Convert(StudentData item)
        {
            if (item == null)
                return null;

            return new Student
            {
                IdStudent = item.IdStudent,
                Name = item.Name,
                Birthday = item.Birthday,
                Email = item.Email,
                CPF = item.CPF,
                RegisterDate = item.RegisterDate,
                RegisterBy = item.RegisterBy,
                DeleteDate = item.DeleteDate,
                DeleteBy = item.DeleteBy
            };
        }
    }
}