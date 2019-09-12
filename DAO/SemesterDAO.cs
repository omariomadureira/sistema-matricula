using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class SemesterDAO
    {
        public static bool Add(Semester item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                SemesterData row = new SemesterData
                {
                    IdSemester = Guid.NewGuid(),
                    Period = item.Period,
                    InitialDate = item.InitialDate,
                    RegisterDate = DateTime.Now,
                    RegisterBy = User.Logged.IdUser
                };

                using (Entities db = new Entities())
                {
                    db.SemesterData.Add(row);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.Add", notes);
            }

            return false;
        }

        public static Semester Find(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                Semester item = null;

                using (Entities db = new Entities())
                {
                    SemesterData row = db.SemesterData.FirstOrDefault(x => x.IdSemester == id);

                    if (row == null)
                        throw new Exception("Semestre não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.Find", notes);
            }

            return null;
        }

        public static Semester Last()
        {
            try
            {
                Semester item = null;

                using (Entities db = new Entities())
                {
                    SemesterData row = db.SemesterData
                        .Where(x => x.DeleteDate == null)
                        .OrderByDescending(x => x.InitialDate)
                        .FirstOrDefault();

                    if (row == null)
                        throw new Exception("Semestre não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = string.Format("Erro: {0}", e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.Last", notes);
            }

            return null;
        }

        public static List<Semester> List(Semester filters = null)
        {
            try
            {
                List<Semester> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<SemesterData> query = db.SemesterData.Where(x => x.DeleteDate == null);

                    if (filters != null && !string.IsNullOrWhiteSpace(filters.Period))
                        query = query.Where(x => x.Period.ToLower().Contains(filters.Period.ToLower()));

                    query = query.OrderByDescending(x => x.RegisterDate);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<Semester>();

                        query = query.Skip(filters.Pagination.Skip).Take(filters.Pagination.ItensPerPage);
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
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.List", notes);
            }

            return null;
        }

        public static bool Update(Semester item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Parâmetro vazio");

                using (Entities db = new Entities())
                {
                    SemesterData row = db.SemesterData.FirstOrDefault(x => x.IdSemester == item.IdSemester);

                    if (row == null)
                        throw new Exception("Semestre não encontrado");

                    row.InitialDate = item.InitialDate;
                    row.Period = item.Period;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(item, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.Update", notes);
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
                    SemesterData row = db.SemesterData.FirstOrDefault(x => x.IdSemester == id);

                    if (row == null)
                        throw new Exception("Semestre não encontrado");

                    row.DeleteDate = DateTime.Now;
                    row.DeleteBy = User.Logged.IdUser;

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.SemesterDAO.Delete", notes);
            }

            return false;
        }

        public static Semester Convert(SemesterData item)
        {
            if (item == null)
                return null;

            return new Semester
            {
                IdSemester = item.IdSemester,
                Period = item.Period,
                InitialDate = item.InitialDate,
                RegisterDate = item.RegisterDate,
                RegisterBy = item.RegisterBy,
                DeleteDate = item.DeleteDate,
                DeleteBy = item.DeleteBy
            };
        }
    }
}