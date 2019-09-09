using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class ProfessorController : Controller
    {
        public ActionResult Index(Teacher view)
        {
            ModelState.Clear();

            try
            {
                ViewBag.Professores = Teacher.Listar(view.Nome);

                if (ViewBag.Professores == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(Teacher view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(view.IdProfessor, System.Guid.Empty))
                {
                    view = Teacher.Consultar(view.IdProfessor);

                    if (view == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }

                    return View(view);
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(Teacher view)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    view.CPF = view.CPF.Trim();
                    view.Email = view.Email.Trim();
                    view.Nome = view.Nome.Trim();
                    view.Curriculo = view.Curriculo.Trim();

                    if (!Equals(view.IdProfessor, System.Guid.Empty))
                    {
                        if (Teacher.Alterar(view))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                        }
                    }
                    else
                    {
                        if (Teacher.Incluir(view))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível atualizar o registro. Revise o preenchimento dos campos.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        public ActionResult Delete(Teacher view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdProfessor, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Teacher.Desativar(view.IdProfessor))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    view = Teacher.Consultar(view.IdProfessor);

                    if (view == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }
    }
}