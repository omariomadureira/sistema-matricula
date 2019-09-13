using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.DAO
{
    public class LogDAO
    {
        public static void Add(string type, string description, string notes)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                    throw new Exception("Parâmetro 'type' vazio");

                if (string.IsNullOrEmpty(description))
                    throw new Exception("Parâmetro 'description' vazio");

                if (string.IsNullOrEmpty(notes))
                    throw new Exception("Parâmetro 'notes' vazio");

                LogData row = new LogData
                {
                    IdLog = Guid.NewGuid(),
                    Type = type,
                    Description = description,
                    Notes = notes,
                    IdUser = User.Logged.IdUser,
                    RegisterDate = DateTime.Now,
                };

                using (Entities db = new Entities())
                {
                    db.LogData.Add(row);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        public static Log Find(Guid id)
        {
            try
            {
                if (id == null || Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                Log item = null;

                using (Entities db = new Entities())
                {
                    LogData row = db.LogData.FirstOrDefault(x => x.IdLog == id);

                    if (row == null)
                        throw new Exception("Log não encontrado");

                    item = Convert(row);
                }

                return item;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.LogDAO.Find", notes);
            }

            return null;
        }

        public static List<Log> List(Log filters = null)
        {
            try
            {
                List<Log> list = null;

                using (Entities db = new Entities())
                {
                    IEnumerable<LogData> query = db.LogData;

                    if (filters != null)
                    {
                        if (!string.IsNullOrEmpty(filters.Description))
                        {
                            filters.Description = filters.Description.Trim().ToLower();

                            query = query
                                .Where(x =>
                                    x.Description.ToLower().Contains(filters.Description)
                                    || x.Notes.ToLower().Contains(filters.Description));
                        }

                        if (!string.IsNullOrEmpty(filters.Type))
                        {
                            query = query
                                .Where(x => x.Type.ToLower().Contains(filters.Type.ToLower()));
                        }
                    }

                    query = query.OrderByDescending(x => x.RegisterDate);

                    if (filters != null && filters.Pagination != null)
                    {
                        filters.Pagination.Rows = query.Count();

                        if (filters.Pagination.Rows < 1)
                            return new List<Log>();

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
                Add(Log.TYPE_ERROR, "SistemaMatricula.DAO.LogDAO.List", e.Message);
            }

            return null;
        }

        public static Log Convert(LogData item)
        {
            if (item == null)
                return null;

            return new Log
            {
                IdLog = item.IdLog,
                Type = item.Type,
                Description = item.Description,
                Notes = item.Notes,
                IdUser = item.IdUser,
                RegisterDate = item.RegisterDate
            };
        }
    }
}