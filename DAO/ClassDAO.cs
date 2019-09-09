﻿using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class ClassDAO
    {
        public static bool Add(Class item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                if (item.Course == null)
                    throw new Exception("Parâmetro vazio");

                ClassData row = new ClassData
                {
                    IdClass = Guid.NewGuid(),
                    Name = item.Name,
                    Description = item.Description,
                    IdCourse = item.Course.IdCourse,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.ClassData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Item: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.Add", notes);
            }

            return false;
        }


        public static Class Find(Guid id)
        {
            try
            {
                if (id == null || Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                Class item = null;

                using (Entities db = new Entities())
                {
                    ClassData row = db.ClassData.FirstOrDefault(x => x.IdClass == id);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Id: {0}. Erro: {1}", id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.Find", notes);
            }

            return null;
        }

        public static List<Class> List(Class filters = null)
        {
            try
            {
                List<Class> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<ClassData> query = db.ClassData.Where(x => x.DeleteDate == null);

                    if (filters != null)
                    {
                        if (!string.IsNullOrWhiteSpace(filters.Name))
                        {
                            filters.Name = filters.Name.Trim().ToLower();

                            query = query
                                .Where(x => 
                                    x.Name.ToLower().Contains(filters.Name) 
                                    || x.Description.ToLower().Contains(filters.Name));
                        }

                        if (filters.Course != null && !Equals(Guid.Empty, filters.Course.IdCourse))
                            query = query.Where(x => x.IdCourse == filters.Course.IdCourse);
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
                string notes = string.Format("Filtros: {0}. Erro: {1}", filters.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Class item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                if (item.Course == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    ClassData row = db.ClassData.FirstOrDefault(x => x.IdClass == item.IdClass);

                    if (row == null)
                        throw new Exception("Registro não encontrado");

                    row.Name = item.Name;
                    row.Description = item.Description;
                    row.IdCourse = item.Course.IdCourse;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.Update", notes);
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
                    ClassData row = db.ClassData.FirstOrDefault(x => x.IdClass == id);

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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.Delete", notes);
            }

            return false;
        }

        public static Class Convert(ClassData item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                var course = CourseDAO.Find(item.IdCourse);

                if (course == null)
                    throw new Exception("Registro não encontrado");

                return new Class
                {
                    IdClass = item.IdClass,
                    Name = item.Name,
                    Course = course,
                    Description = item.Description,
                    RegisterDate = item.RegisterDate,
                    RegisterBy = item.RegisterBy,
                    DeleteDate = item.DeleteDate,
                    DeleteBy = item.DeleteBy
                };
            }
            catch (Exception e)
            {
                string notes = string.Format("Objeto: {0}. Erro: {1}", item.ToString(), e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.ClassDAO.Convert", notes);
            }

            return null;
        }
    }
}