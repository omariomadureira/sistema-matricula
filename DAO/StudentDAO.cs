using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

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
                string notes = string.Format("Item: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Add", notes);
            }

            return false;
        }

        public static Student Find(Guid? id = null, string email = null)
        {
            try
            {
                if (id == null && string.IsNullOrEmpty(email))
                    throw new Exception("Parâmetro vazio");

                if (id != null && Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

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
                        throw new Exception("Registro não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Email: {1}. Erro: {2}", id, email, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.StudentDAO.Find", notes);
            }

            return null;
        }

        public static List<Student> List(string filter = null)
        {
            try
            {
                List<Student> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<StudentData> query = db.StudentData.Where(x => x.DeleteDate == null);

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = filter.Trim().ToLower();

                        query = query
                            .Where(x =>
                                x.Name.ToLower().Contains(filter)
                                || x.Email.ToLower().Contains(filter));
                    }

                    list = query
                        .Select(x => Convert(x))
                        .OrderByDescending(x => x.RegisterDate)
                        .ToList();
                }

                return list;
            }
            catch (Exception e)
            {
                string notes = string.Format("Filtro: {0}. Erro: {1}", filter, e.Message);
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
                        throw new Exception("Registro não encontrado");

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
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
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
                        throw new Exception("Registro não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Erro: {1}", id, e.Message);
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