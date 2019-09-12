using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SistemaMatricula.Helpers;
using System;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class SemesterController : Controller
    {
        public ActionResult Index(SemesterView view)
        {
            ModelState.Clear();

            try
            {
                view.PeriodSelectList = new SelectList(Semester.Periods());

                if (view.PeriodSelected != null)
                    view.Period = view.PeriodSelected;

                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Semester.List(view);

                if (ViewBag.List == null)
                    throw new Exception("Os semestres não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.SemesterController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(SemesterView view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                view.PeriodSelectList = new SelectList(Semester.Periods());

                if (error)
                {
                    ViewBag.Message = "Não foi possível salvar o registro. Analise os erros.";
                    return View("Edit", view);
                }

                ModelState.Clear();

                if (Equals(view.IdSemester, System.Guid.Empty))
                    return View("Edit", view);

                var item = Semester.Find(view.IdSemester);

                if (item == null)
                    throw new Exception("Semestre não encontrado");

                view.PeriodSelected = item.Period.Trim();
                view.PeriodSelectList = new SelectList(Semester.Periods(), item.Period.Trim());
                view.InitialDate = item.InitialDate;
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.SemesterController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(SemesterView view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.Period = view.PeriodSelected.Trim();

                if (Equals(view.IdSemester, System.Guid.Empty))
                {
                    var insert = Semester.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Semester");
                }

                var update = Semester.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Semester");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.SemesterController.Update", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit");
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            try
            {
                if (Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                var deleted = Semester.Delete(id);

                if (deleted == false)
                    throw new Exception("Semestre não deletado");

                return RedirectToAction("Index", "Semester");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.SemesterController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }
    }

    public class SemesterView : Semester
    {
        public SelectList PeriodSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string PeriodSelected { get; set; }
    }
}