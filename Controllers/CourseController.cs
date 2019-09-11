using SistemaMatricula.Models;
using System.Web.Mvc;
using SistemaMatricula.Helpers;
using System;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class CourseController : Controller
    {
        public ActionResult Index(Course view)
        {
            ModelState.Clear();

            try
            {
                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Course.List(view);

                if (ViewBag.List == null)
                    throw new Exception("Os cursos não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.CourseController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(Course view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                if (error)
                {
                    ViewBag.Message = "Não foi possível salvar o registro. Analise os erros.";
                    return View("Edit", view);
                }

                ModelState.Clear();

                if (Equals(view.IdCourse, System.Guid.Empty))
                    return View("Edit");

                Course item = Course.Find(view.IdCourse);

                if (item == null)
                    throw new Exception("Curso não encontrado");

                view.Credits = item.Credits;
                view.Description = item.Description;
                view.Name = item.Name;
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.CourseController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(Course view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.Description = view.Description.Trim();
                view.Name = view.Name.Trim();

                if (Guid.Equals(view.IdCourse, System.Guid.Empty))
                {
                    var insert = Course.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Course");
                }

                var update = Course.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Course");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.CourseController.Update", notes);
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
                if (Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                var deleted = Course.Delete(id);

                if (deleted == false)
                    throw new Exception("Curso não deletado");

                return RedirectToAction("Index", "Course");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.CourseController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }
    }
}