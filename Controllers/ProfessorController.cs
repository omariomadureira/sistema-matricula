using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class ProfessorController : Controller
    {
        public ActionResult Index(Professor view)
        {
            ModelState.Clear();

            try
            {
                ViewBag.Professores = Professor.Listar(view.Nome);

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
        public ActionResult Edit(Professor view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(view.IdProfessor, System.Guid.Empty))
                {
                    view = Professor.Consultar(view.IdProfessor);

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
        public ActionResult Update(Professor view)
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
                        if (Professor.Alterar(view))
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
                        if (Professor.Incluir(view))
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

        public ActionResult Delete(Professor view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdProfessor, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Professor.Desativar(view.IdProfessor))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    view = Professor.Consultar(view.IdProfessor);

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