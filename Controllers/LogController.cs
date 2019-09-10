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
                    throw new Exception("Os logs não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
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
    }
}