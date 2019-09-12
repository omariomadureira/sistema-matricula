using SistemaMatricula.Models;
using System.Web.Mvc;
using SistemaMatricula.Helpers;
using System;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class TeacherController : Controller
    {
        public ActionResult Index(Teacher view)
        {
            ModelState.Clear();

            try
            {
                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Teacher.List(view);

                if (ViewBag.List == null)
                    throw new Exception("Os professores não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.TeacherController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(Teacher view, bool error = false)
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

                if (Equals(view.IdTeacher, System.Guid.Empty))
                    return View("Edit");

                Teacher item = Teacher.Find(view.IdTeacher);

                if (item == null)
                    throw new Exception("Professor não encontrado");

                view.Birthday = item.Birthday;
                view.CPF = item.CPF;
                view.Email = item.Email;
                view.Name = item.Name;
                view.Resume = item.Resume;
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.TeacherController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(Teacher view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.CPF = view.CPF.Trim();
                view.Email = view.Email.Trim();
                view.Name = view.Name.Trim();
                view.Resume = view.Resume.Trim();

                if (Equals(view.IdTeacher, System.Guid.Empty))
                {
                    var insert = Teacher.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Teacher");
                }

                var update = Teacher.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Teacher");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.TeacherController.Update", notes);
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

                var deleted = Teacher.Delete(id);

                if (deleted == false)
                    throw new Exception("Professor não deletado");

                return RedirectToAction("Index", "Teacher");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.TeacherController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }
    }
}