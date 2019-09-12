using SistemaMatricula.Models;
using System.Web.Mvc;
using SistemaMatricula.Helpers;
using System;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class StudentController : Controller
    {
        public ActionResult Index(Student view)
        {
            ModelState.Clear();

            try
            {
                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Student.List(view);

                if (ViewBag.List == null)
                    throw new Exception("Os alunos não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.StudentController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(Student view, bool error = false)
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

                if (Equals(view.IdStudent, System.Guid.Empty))
                    return View("Edit");

                Student item = Student.Find(view.IdStudent);

                if (item == null)
                    throw new Exception("Aluno não encontrado");

                view.Birthday = item.Birthday;
                view.CPF = item.CPF;
                view.Email = item.Email;
                view.Name = item.Name;
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.StudentController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(Student view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.CPF = view.CPF.Trim();
                view.Email = view.Email.Trim();
                view.Name = view.Name.Trim();

                if (Equals(view.IdStudent, System.Guid.Empty))
                {
                    var insert = Student.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Student");
                }

                var update = Student.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Student");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.StudentController.Update", notes);
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

                var deleted = Student.Delete(id);

                if (deleted == false)
                    throw new Exception("Aluno não deletado");

                return RedirectToAction("Index", "Student");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.StudentController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }
    }
}