using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class SemestreDAO
    {
        public static bool Incluir(Semestre item)
        {
            try
            {
                SemestreData Semestre = new SemestreData
                {
                    IdSemestre = Guid.NewGuid(),
                    Periodo = item.Periodo,
                    InicioData = item.InicioData,
                    CadastroData = DateTime.Now,
                    CadastroPor = Guid.Empty //TODO: Alterar para ID do usuário logado
                };

                Entities db = new Entities();
                db.SemestreData.Add(Semestre);
                db.SaveChanges();
                db.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Semestre Consultar(Guid IdSemestre)
        {
            try
            {
                Entities db = new Entities();

                SemestreData Semestre = db.SemestreData.FirstOrDefault(x => x.IdSemestre == IdSemestre);

                db.Dispose();

                return Converter(Semestre);
            }
            catch { }

            return null;
        }

        public static Semestre Ultimo()
        {
            try
            {
                Entities db = new Entities();

                SemestreData Semestre = db.SemestreData.Where(x => x.ExclusaoData == null).OrderByDescending(x => x.InicioData).FirstOrDefault();

                db.Dispose();

                return Converter(Semestre);
            }
            catch { }

            return null;
        }

        public static List<Semestre> Listar(string palavra)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<SemestreData> query = db.SemestreData.Where(x => x.ExclusaoData == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Periodo.ToLower().Contains(palavra.ToLower()));

                List<Semestre> Semestres = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return Semestres;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Alterar(Semestre item)
        {
            try
            {
                Entities db = new Entities();
                SemestreData Semestre = db.SemestreData.FirstOrDefault(x => x.IdSemestre == item.IdSemestre);

                if (Semestre != null)
                {
                    Semestre.Periodo = item.Periodo;
                    Semestre.InicioData = item.InicioData;

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static bool Desativar(Guid IdSemestre)
        {
            try
            {
                Entities db = new Entities();
                SemestreData Semestre = db.SemestreData.FirstOrDefault(x => x.IdSemestre == IdSemestre);

                if (Semestre != null)
                {
                    Semestre.ExclusaoData = DateTime.Now;
                    Semestre.ExclusaoPor = Guid.Empty; //TODO: Alterar para ID do usuário logado

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Semestre Converter(SemestreData a)
        {
            try
            {
                return new Semestre
                {
                    IdSemestre = a.IdSemestre,
                    Periodo = a.Periodo,
                    InicioData = a.InicioData,
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };
            }
            catch
            {
                return null;
            }
        }
    }
}