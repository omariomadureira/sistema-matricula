using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

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
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Add", notes);
            }

            return false;
        }

        public static Course Find(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                Course item = null;

                using (Entities db = new Entities())
                {
                    CourseData row = db.CourseData.FirstOrDefault(x => x.IdCourse == id);

                    if (row == null)
                        throw new Exception("Curso não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Find", notes);
            }

            return null;
        }

        public static List<Course> List(Course filters = null, Func<CourseData, object> sort = null)
        {
            try
            {
                List<Course> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<CourseData> query = db.CourseData.Where(x => x.DeleteDate == null);

                    if (filters != null && !string.IsNullOrWhiteSpace(filters.Name))
                    {
                        filters.Name = filters.Name.Trim().ToLower();

                        query = query
                            .Where(x =>
                            x.Name.ToLower().Contains(filters.Name)
                            || x.Description.ToLower().Contains(filters.Name));
                    }

                    if (sort == null)
                        query = query.OrderByDescending(x => x.RegisterDate);

                    if (sort != null)
                        query = query.OrderBy(sort);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<Course>();

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
                        throw new Exception("Curso não encontrado");

                    row.Name = item.Name;
                    row.Description = item.Description;
                    row.Credits = item.Credits;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.CourseDAO.Update", notes);
            }

            return false;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                using (Entities db = new Entities())
                {
                    CourseData row = db.CourseData.FirstOrDefault(x => x.IdCourse == id);

                    if (row == null)
                        throw new Exception("Curso não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
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