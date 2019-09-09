using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class RegistryDAO
    {
        public static bool Add(Registry item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                RegistryData row = new RegistryData
                {
                    IdRegistry = Guid.NewGuid(),
                    IdGrid = item.Grid.IdGrid,
                    IdStudent = item.Student.IdStudent,
                    Alternative = item.Alternative,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.RegistryData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Item: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Add", notes);
            }

            return false;
        }

        public static Registry Find(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                Registry item = null;

                using (Entities db = new Entities())
                {
                    RegistryData row = db.RegistryData.FirstOrDefault(x => x.IdRegistry == id);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Erro: {1}", id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Find", notes);
            }

            return null;
        }

        public static List<Registry> List(Registry filter = null)
        {
            try
            {
                List<Registry> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<RegistryData> query = db.RegistryData.Where(x => x.DeleteDate == null);

                    if (filter != null)
                    {
                        if (filter.Alternative != null)
                            query = query.Where(x => x.Alternative == filter.Alternative);

                        if (filter.Student != null && !Guid.Equals(filter.Student.IdStudent, Guid.Empty))
                            query = query.Where(x => x.IdStudent == filter.Student.IdStudent);
                    }

                    list = query
                        .Select(x => Convert(x))
                        .OrderBy(x => x.Alternative)
                        .ThenBy(x => x.Grid.Weekday)
                        .ThenBy(x => x.Grid.Time)
                        .ToList();
                }

                return list;
            }
            catch (Exception e)
            {
                string notes = string.Format("Filtro: {0}. Erro: {1}", filter, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.List", notes);
            }

            return null;
        }

        public static List<Registry> GridList(Guid idStudent, Guid idCourse)
        {
            try
            {
                if (idStudent == null || Guid.Equals(idStudent, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                if (idCourse == null || Guid.Equals(idCourse, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                Semester semester = SemesterDAO.Last();

                if (semester == null)
                    throw new Exception("Registro não encontrado");

                List<GridData> grid = null;

                using (Entities db = new Entities())
                {
                    grid = db.GridData
                        .Where(g => g.DeleteDate == null
                            && db.SemesterData
                                .Where(s => s.InitialDate == semester.InitialDate)
                                .Select(s => s.IdSemester)
                                .Contains(g.IdSemester)
                            && !db.RegistryData
                                .Where(r => r.DeleteDate == null && r.IdStudent == idStudent)
                                .Select(r => r.IdGrid)
                                .Contains(g.IdGrid)
                            && db.ClassData
                                .Where(c => c.IdCourse == idCourse)
                                .Select(c => c.IdCourse)
                                .Contains(g.IdClass)
                            && g.Status == Grid.RELEASED)
                        .ToList();
                }

                if (grid.Count < 1)
                    return new List<Registry>();

                List<Registry> list = grid
                    .Select(x => new Registry() { Grid = GridDAO.Convert(x) })
                    .OrderBy(x => x.Grid.Semester.Name)
                    .ThenBy(x => x.Grid.Weekday)
                    .ThenBy(x => x.Grid.Time)
                    .ToList();

                return list;
            }
            catch (Exception e)
            {
                string notes = string.Format("idStudent: {0}. idCourse: {1}. Erro: {2}", idStudent, idCourse, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.GridList", notes);
            }

            return null;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    RegistryData row = db.RegistryData.FirstOrDefault(x => x.IdRegistry == id);

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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Delete", notes);
            }

            return false;
        }

        public static Registry Convert(RegistryData item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                var grid = GridDAO.Find(item.IdGrid);

                if (grid == null)
                    throw new Exception("Registro não encontrado");

                var student = StudentDAO.Find(item.IdStudent);

                if (student == null)
                    throw new Exception("Registro não encontrado");

                return new Registry
                {
                    Grid = grid,
                    Student = student,
                    Alternative = item.Alternative,
                    RegisterDate = item.RegisterDate,
                    RegisterBy = item.RegisterBy,
                    DeleteDate = item.DeleteDate,
                    DeleteBy = item.DeleteBy
                };
            }
            catch (Exception e)
            {
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Convert", notes);
            }

            return null;
        }
    }
}