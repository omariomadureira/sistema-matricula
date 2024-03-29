﻿using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

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
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Add", notes);
            }

            return false;
        }

        public static Registry Find(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                Registry item = null;

                using (Entities db = new Entities())
                {
                    RegistryData row = db.RegistryData.FirstOrDefault(x => x.IdRegistry == id);

                    if (row == null)
                        throw new Exception("Matrícula não encontrada");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Find", notes);
            }

            return null;
        }

        public static List<Registry> List(Registry filters = null, bool actual = false)
        {
            try
            {
                List<Registry> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<RegistryData> query = db.RegistryData.Where(x => x.DeleteDate == null);

                    if (filters != null)
                    {
                        if (filters.Alternative != null)
                            query = query.Where(x => x.Alternative == filters.Alternative);

                        if (filters.Student != null && !Equals(filters.Student.IdStudent, Guid.Empty))
                            query = query.Where(x => x.IdStudent == filters.Student.IdStudent);

                        if (filters.Grid != null && !Equals(filters.Grid.IdGrid, Guid.Empty))
                            query = query.Where(x => x.IdGrid == filters.Grid.IdGrid);

                        if (filters.Grid != null && filters.Grid.Semester != null
                            && !Equals(filters.Grid.Semester.IdSemester, Guid.Empty))
                        {
                            query = query.Where(x =>
                                db.GridData
                                    .Where(g => g.IdSemester == filters.Grid.Semester.IdSemester)
                                    .Select(g => g.IdGrid)
                                    .Contains(x.IdGrid));
                        }

                        if (filters.Grid != null && filters.Grid.Teacher != null
                            && !Equals(filters.Grid.Teacher.IdTeacher, Guid.Empty))
                        {
                            query = query.Where(x =>
                                db.GridData
                                    .Where(g => g.IdTeacher == filters.Grid.Teacher.IdTeacher)
                                    .Select(g => g.IdGrid)
                                    .Contains(x.IdGrid));
                        }

                        if (filters.Grid != null && filters.Grid.Class != null 
                            && !Equals(filters.Grid.Class.IdClass, Guid.Empty))
                        {
                            query = query.Where(x =>
                                db.GridData
                                    .Where(g => g.IdClass == filters.Grid.Class.IdClass)
                                    .Select(g => g.IdGrid)
                                    .Contains(x.IdGrid));
                        }
                    }

                    query = query.OrderByDescending(x => x.RegisterDate);

                    IEnumerable<Registry> rows = query.Select(x => Convert(x));

                    if (actual)
                        rows = rows.Where(x => x.Grid.Semester.IsActual);

                    list = rows
                       .OrderBy(x => x.Grid.Semester.Name)
                       .ThenBy(x => x.Grid.Class.Course.Name)
                       .ThenBy(x => x.Grid.Weekday)
                       .ThenBy(x => x.Grid.Time)
                       .ThenBy(x => x.Student.Name)
                       .ToList();
                }

                return list;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(filters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.List", notes);
            }

            return null;
        }

        public static List<Registry> GridList(Guid idStudent, Guid idCourse)
        {
            try
            {
                if (idStudent == null || Equals(idStudent, Guid.Empty))
                    throw new Exception("Parâmetro 'idStudent' vazio");

                if (idCourse == null || Equals(idCourse, Guid.Empty))
                    throw new Exception("Parâmetro 'idCourse' vazio");

                Semester semester = SemesterDAO.Last();

                if (semester == null)
                    throw new Exception("Semestre não encontrado");

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
                                .Select(c => c.IdClass)
                                .Contains(g.IdClass)
                            && g.Status.Trim() == Grid.RELEASED)
                        .ToList();
                }

                if (grid.Count < 1)
                    return new List<Registry>();

                Student student = StudentDAO.Find(idStudent);

                List<Registry> list = grid
                    .Select(x => new Registry() { Grid = GridDAO.Convert(x), Student = student })
                    .OrderBy(x => x.Grid.Semester.Name)
                    .ThenBy(x => x.Grid.Weekday)
                    .ThenBy(x => x.Grid.Time)
                    .ToList();

                return list;
            }
            catch (Exception e)
            {
                object[] parameters = { idStudent, idCourse };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.GridList", notes);
            }

            return null;
        }

        public static bool Delete(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                using (Entities db = new Entities())
                {
                    RegistryData row = db.RegistryData.FirstOrDefault(x => x.IdRegistry == id);

                    if (row == null)
                        throw new Exception("Matrícula não encontrada");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Delete", notes);
            }

            return false;
        }

        public static bool DeleteByGrid(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                using (Entities db = new Entities())
                {
                    List<RegistryData> rows = db.RegistryData.Where(x => x.IdGrid == id).ToList();

                    if (rows.Count < 1)
                        return true;

                    foreach (RegistryData row in rows)
                    {
                        row.DeleteDate = DateTime.Now;
                        row.DeleteBy = User.Logged.IdUser;
                    }

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
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
                    throw new Exception("Grade não encontrado");

                var student = StudentDAO.Find(item.IdStudent);

                if (student == null)
                    throw new Exception("Aluno não encontrado");

                return new Registry
                {
                    IdRegistry = item.IdRegistry,
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
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.RegistryDAO.Convert", notes);
            }

            return null;
        }
    }
}