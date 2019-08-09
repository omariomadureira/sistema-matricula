using System;
using System.Collections.Generic;
using System.Linq;
using SistemaMatricula.Database;
using SistemaMatricula.Models;

namespace SistemaMatricula.DAO
{
    public class UsuarioDAO
    {
        public static Usuario Consultar(Guid IdUsuario)
        {
            try
            {
                Entities db = new Entities();

                AspNetUsers Usuario = db.AspNetUsers.FirstOrDefault(x => x.Id == IdUsuario.ToString());

                db.Dispose();

                return Converter(Usuario);
            }
            catch { }

            return null;
        }

        public static List<Usuario> Listar(string palavra)
        {
            try
            {
                Entities db = new Entities();

                IEnumerable<AspNetUsers> query = db.AspNetUsers.Where(x => x.ExclusaoData == null);

                if (!string.IsNullOrWhiteSpace(palavra))
                    query = query.Where(x => x.Email.ToLower().Contains(palavra.ToLower()));

                List<Usuario> Usuarios = query.Select(x => Converter(x)).ToList();

                db.Dispose();

                return Usuarios;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool Desativar(Guid IdUsuario)
        {
            try
            {
                Entities db = new Entities();
                AspNetUsers Usuario = db.AspNetUsers.FirstOrDefault(x => x.Id == IdUsuario.ToString());

                if (Usuario != null)
                {
                    Usuario.ExclusaoData = DateTime.Now;
                    Usuario.ExclusaoPor = Guid.Empty; //TODO: Alterar para ID do usuário logado

                    db.SaveChanges();
                    db.Dispose();

                    return true;
                }

                db.Dispose();
            }
            catch { }

            return false;
        }

        public static Usuario Converter(AspNetUsers a)
        {
            try
            {
                return new Usuario
                {
                    IdUsuario = Guid.Parse(a.Id),
                    Email = a.Email,
                    CadastroData = a.CadastroData,
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