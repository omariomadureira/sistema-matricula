using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class ProfessorController : Controller
    {
        public ActionResult Index(FormCollection formulario)
        {
            try
            {
                if (formulario.Count > 0 && !string.IsNullOrWhiteSpace(formulario["palavra"]))
                {
                    ViewBag.Professores = Professor.Listar(formulario["palavra"]);
                    ViewBag.Palavra = formulario["palavra"];
                }
                else
                {
                    ViewBag.Professores = Professor.Listar();
                }

                if (ViewBag.Professores == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View();
        }

        public ActionResult Edit(FormCollection formulario, System.Guid? Id)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (formulario.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(formulario["nome"])
                        && !string.IsNullOrWhiteSpace(formulario["descricao"]))
                    {
                        if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                        {
                            if (Professor.Alterar(Id.Value, formulario["nome"], formulario["descricao"]))
                            {
                                return RedirectToAction("Index", "Professor");
                            }
                            else
                            {
                                ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                                ViewBag.HideScreen = true;
                                return View();
                            }
                        }

                        if (Professor.Incluir(formulario["nome"], formulario["descricao"]))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Não foi possível atualizar o registro. Um ou mais campos não foram preenchidos.";
                    }
                }

                if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                {
                    ViewBag.Professor = Professor.Consultar(Id.Value);

                    if (ViewBag.Professor == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }
                }
                else
                {
                    ViewBag.Professor = new Professor();
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View();
        }

        public ActionResult Delete(System.Guid? Id, bool? Delete)
        {
            try
            {
                if (Id.HasValue && !System.Guid.Equals(Id.Value, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Professor.Desativar(Id.Value))
                        {
                            return RedirectToAction("Index", "Professor");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    ViewBag.Professor = Professor.Consultar(Id.Value);

                    if (ViewBag.Professor == null)
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

            return View();
        }
    }
}