using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class CourseDAO
    {
        public static bool Add(Course item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                CourseData row = new CourseData
                {
                    IdCourse = Guid.NewGuid(),
                    Name = item.Name,
                    Description = item.Description,
                    Credits = item.Credits,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.CourseData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Item: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Add", notes);
            }

            return false;
        }

        public static Course Find(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                Course item = null;

                using (Entities db = new Entities())
                {
                    CourseData row = db.CourseData.FirstOrDefault(x => x.IdCourse == id);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Erro: {1}", id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Find", notes);
            }

            return null;
        }

        public static List<Course> List(string filter = null)
        {
            try
            {
                List<Course> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<CourseData> query = db.CourseData.Where(x => x.DeleteDate == null);

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = filter.Trim().ToLower();

                        query = query
                            .Where(x =>
                            x.Name.ToLower().Contains(filter)
                            || x.Description.ToLower().Contains(filter));
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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Course item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    CourseData row = db.CourseData.FirstOrDefault(x => x.IdCourse == item.IdCourse);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    row.Name = item.Name;
                    row.Description = item.Description;
                    row.Credits = item.Credits;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Update", notes);
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
                    CourseData row = db.CourseData.FirstOrDefault(x => x.IdCourse == id);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Erro: {1}", id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Delete", notes);
            }

            return false;
        }

        public static Course Convert(CourseData item)
        {
            return new Course
            {
                IdCourse = item.IdCourse,
                Name = item.Name,
                Description = item.Description,
                Credits = item.Credits,
                RegisterDate = item.RegisterDate,
                RegisterBy = item.RegisterBy,
                DeleteDate = item.DeleteDate,
                DeleteBy = item.DeleteBy
            };
        }
    }
}