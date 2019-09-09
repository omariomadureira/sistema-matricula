using SistemaMatricula.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class LogController : Controller
    {
        public ActionResult Index(LogView view)
        {
            ModelState.Clear();

            try
            {
                view.TypeSelectList = new SelectList(Log.Types());

                if (view.TypeSelected != null)
                    view.Type = view.TypeSelected;

                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Log.List(view);

                if (ViewBag.List == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                }
            }
            catch (Exception e)
            {
                string notes = string.Format("Filtro: {0}. Erro: {1}", view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.LogController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }
    }

    public class LogView : Log
    {
        public SelectList TypeSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string TypeSelected { get; set; }

        public static LogView Convert(Log item)
        {
            if (item == null)
                return null;

            return new LogView()
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