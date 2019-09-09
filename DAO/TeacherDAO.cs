using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class TeacherDAO
    {
        public static bool Add(Teacher item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                TeacherData row = new TeacherData
                {
                    IdTeacher = Guid.NewGuid(),
                    Name = item.Name,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    CPF = item.CPF,
                    Resume = item.Resume,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.TeacherData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Item: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.TeacherDAO.Add", notes);
            }

            return false;
        }

        public static Teacher Find(Guid? id = null, string email = null)
        {
            try
            {
                if (id == null && string.IsNullOrEmpty(email))
                    throw new Exception("Parâmetro vazio");

                if (id != null && Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                Teacher item = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<TeacherData> query = db.TeacherData;

                    if (id != null)
                        query = query.Where(x => x.IdTeacher == id);

                    if (!string.IsNullOrEmpty(email))
                        query = query.Where(x => x.Email.Trim() == email.Trim());

                    TeacherData row = query.FirstOrDefault();

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Email: {1}. Erro: {2}", id, email, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.TeacherDAO.Find", notes);
            }

            return null;
        }

        public static List<Teacher> List(string filter = null)
        {
            try
            {
                List<Teacher> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<TeacherData> query = db.TeacherData.Where(x => x.DeleteDate == null);

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = filter.Trim().ToLower();

                        query = query
                            .Where(x =>
                                x.Name.ToLower().Contains(filter)
                                || x.Email.ToLower().Contains(filter)
                                || x.Resume.ToLower().Contains(filter));
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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.TeacherDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Teacher item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    TeacherData row = db.TeacherData.FirstOrDefault(x => x.IdTeacher == item.IdTeacher);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    row.Name = item.Name;
                    row.Birthday = item.Birthday;
                    row.Email = item.Email;
                    row.CPF = item.CPF;
                    row.Resume = item.Resume;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.TeacherDAO.Update", notes);
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
                    TeacherData row = db.TeacherData.FirstOrDefault(x => x.IdTeacher == id);

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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.TeacherDAO.Delete", notes);
            }

            return false;
        }

        public static Teacher Convert(TeacherData item)
        {
            if (item == null)
                return null;

            return new Teacher
            {
                IdTeacher = item.IdTeacher,
                Name = item.Name,
                Birthday = item.Birthday,
                Email = item.Email,
                CPF = item.CPF,
                Resume = item.Resume,
                RegisterDate = item.RegisterDate,
                RegisterBy = item.RegisterBy,
                DeleteDate = item.DeleteDate,
                DeleteBy = item.DeleteBy
            };
        }
    }
}