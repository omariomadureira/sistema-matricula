using SistemaMatricula.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class ClassController : Controller
    {
        public ActionResult Index(ClassView view)
        {
            ModelState.Clear();

            try
            {
                var courses = Course.List(sort: by => by.Name);

                if (courses == null)
                    throw new Exception("Os cursos não foram listados");

                view.CourseSelectList = new SelectList(courses, "IdCourse", "Name");

                if (view.CourseSelected != null)
                    view.Course = new Course { IdCourse = view.CourseSelected };

                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Class.List(view);

                if (ViewBag.List == null)
                    throw new Exception("As disciplinas não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.ClassController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(ClassView view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                var courses = Course.List(sort: by => by.Name);

                if (courses == null)
                    throw new Exception("Os cursos não foram listados");

                view.CourseSelectList = new SelectList(courses, "IdCourse", "Name");

                if (error)
                {
                    ViewBag.Message = "Não foi possível salvar o registro. Analise os erros.";
                    return View("Edit", view);
                }

                ModelState.Clear();

                if (Equals(view.IdClass, System.Guid.Empty))
                    return View("Edit", view);

                var item = Class.Find(view.IdClass);

                if (item == null)
                    throw new Exception("Disciplina não encontrada");

                view.CourseSelected = item.Course.IdCourse;
                view.CourseSelectList = new SelectList(courses, "IdCourse", "Name", item.Course.IdCourse);
                view.Description = item.Description;
                view.Name = item.Name;
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.ClassController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(ClassView view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.Course = new Course { IdCourse = view.CourseSelected };
                view.Name = view.Name.Trim();
                view.Description = view.Description.Trim();

                if (Equals(view.IdClass, System.Guid.Empty))
                {
                    var insert = Class.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Class");
                }

                var update = Class.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Class");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.ClassController.Update", notes);
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

                var deleted = Class.Delete(id);

                if (deleted == false)
                    throw new Exception("Disciplina não deletada");

                return RedirectToAction("Index", "Class");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.ClassController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }
    }

    public class ClassView : Class
    {
        public SelectList CourseSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid CourseSelected { get; set; }
    }
}