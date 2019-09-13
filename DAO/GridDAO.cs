using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class GridDAO
    {
        public static bool Add(Grid item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                if (item.Class == null)
                    throw new Exception("Parâmetro 'Class' vazio");

                if (item.Semester == null)
                    throw new Exception("Parâmetro 'Semester' vazio");

                if (item.Teacher == null)
                    throw new Exception("Parâmetro 'Teacher' vazio");

                GridData row = new GridData
                {
                    IdGrid = Guid.NewGuid(),
                    IdClass = item.Class.IdClass,
                    IdSemester = item.Semester.IdSemester,
                    IdTeacher = item.Teacher.IdTeacher,
                    Weekday = item.Weekday,
                    Time = item.Time,
                    Status = item.Status,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.GridData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.Add", notes);
            }

            return false;
        }

        public static Grid Find(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                Grid item = null;

                using (Entities db = new Entities())
                {
                    GridData row = db.GridData.FirstOrDefault(x => x.IdGrid == id);

                    if (row == null)
                        throw new Exception("Grade não encontrada");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.Find", notes);
            }

            return null;
        }

        public static List<Grid> List(Grid filters = null, bool active = false)
        {
            try
            {
                List<Grid> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<GridData> query = db.GridData.Where(x => x.DeleteDate == null);

                    if (filters != null)
                    {
                        if (filters.Weekday != 0)
                            query = query.Where(x => x.Weekday == filters.Weekday);

                        if (filters.Class != null && !Equals(Guid.Empty, filters.Class.IdClass))
                            query = query.Where(x => x.IdClass == filters.Class.IdClass);

                        if (filters.Class != null
                            && filters.Class.Course != null
                            && !Equals(Guid.Empty, filters.Class.Course.IdCourse))
                        {
                            query = query.Where(x =>
                                db.ClassData
                                    .Where(y => y.IdCourse == filters.Class.Course.IdCourse)
                                    .Select(y => y.IdClass)
                                    .Contains(x.IdClass));
                        }

                        if (filters.Semester != null && !Equals(Guid.Empty, filters.Semester.IdSemester))
                            query = query.Where(x => x.IdSemester == filters.Semester.IdSemester);

                        if (!string.IsNullOrEmpty(filters.Status))
                            query = query.Where(x => x.Status == filters.Status);

                        if (filters.Teacher != null && !Equals(Guid.Empty, filters.Teacher.IdTeacher))
                            query = query.Where(x => x.IdTeacher == filters.Teacher.IdTeacher);
                    }

                    query = query.OrderByDescending(x => x.RegisterDate);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<Grid>();

                        query = query.Skip(filters.Pagination.Skip).Take(filters.Pagination.ItensPerPage);
                    }

                    IEnumerable<Grid> rows = query.Select(x => Convert(x));

                    if (active)
                        rows = rows.Where(x => x.Semester.IsActive);

                    list = rows
                       .OrderBy(x => x.Semester.Name)
                       .ThenBy(x => x.Class.Course.Name)
                       .ThenBy(x => x.Weekday)
                       .ThenBy(x => x.Time)
                       .ToList();
                }

                return list;
            }
            catch (Exception e)
            {
                object[] parameters = { filters, active };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Grid item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    GridData row = db.GridData.FirstOrDefault(x => x.IdGrid == item.IdGrid);

                    if (row == null)
                        throw new Exception("Grade não encontrada");

                    if (item.Class != null && !Equals(Guid.Empty, item.Class.IdClass))
                        row.IdClass = item.Class.IdClass;

                    if (item.Semester != null && !Equals(Guid.Empty, item.Semester.IdSemester))
                        row.IdSemester = item.Semester.IdSemester;

                    if (item.Teacher != null && !Equals(Guid.Empty, item.Teacher.IdTeacher))
                        row.IdTeacher = item.Teacher.IdTeacher;

                    if (item.Weekday != 0)
                        row.Weekday = item.Weekday;

                    if (!string.IsNullOrEmpty(item.Time))
                        row.Time = item.Time;

                    bool different = false;

                    if (!string.IsNullOrEmpty(item.Status))
                    {
                        if (row.Status.Trim() != item.Status)
                            different = true;

                        row.Status = item.Status;
                    }

                    db.SaveChanges();

                    //TODO: Testar exclusão de matrículas anteriores ao mudar de status
                    if ((item.Status == Grid.REGISTERED || item.Status == Grid.CANCELED) && different)
                    {
                        var delete = Registry.DeleteByGrid(item.IdGrid);

                        if (delete == false)
                            throw new Exception("Matrículas não deletadas");
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.Update", notes);
            }

            return false;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                using (Entities db = new Entities())
                {
                    GridData row = db.GridData.FirstOrDefault(x => x.IdGrid == id);

                    if (row == null)
                        throw new Exception("Grade não encontrada");

                    if (row.Status.Trim() != Grid.REGISTERED && row.Status.Trim() != Grid.CANCELED)
                        throw new Exception("Grade não pode ser excluída");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.Delete", notes);
            }

            return false;
        }

        public static Grid Convert(GridData item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                var classObject = Class.Find(item.IdClass);

                if (classObject == null)
                    throw new Exception("Disciplina não encontrada");

                var semester = Semester.Find(item.IdSemester);

                if (semester == null)
                    throw new Exception("Semestre não encontrado");

                var teacher = Teacher.Find(item.IdTeacher);

                if (teacher == null)
                    throw new Exception("Professor não encontrado");

                return new Grid
                {
                    IdGrid = item.IdGrid,
                    Class = classObject,
                    Semester = semester,
                    Teacher = teacher,
                    Weekday = item.Weekday,
                    Time = item.Time,
                    Status = item.Status,
                    RegisterDate = item.RegisterDate,
                    RegisterBy = item.RegisterBy,
                    DeleteDate = item.DeleteDate,
                    DeleteBy = item.DeleteBy
                };
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.GridDAO.Convert", notes);
            }

            return null;
        }
    }
}