using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class AlunoController : Controller
    {
        public ActionResult Index(FormCollection formulario)
        {
            try
            {
                if (formulario.Count > 0 && !string.IsNullOrWhiteSpace(formulario["palavra"]))
                {
                    ViewBag.Alunos = Aluno.Listar(formulario["palavra"]);
                    ViewBag.Palavra = formulario["palavra"];
                }
                else
                {
                    ViewBag.Alunos = Aluno.Listar();
                }

                if (ViewBag.Alunos == null)
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
                        && !string.IsNullOrWhiteSpace(formulario["nascimento"])
                        && System.DateTime.TryParse(formulario["nascimento"], out _))
                    {
                        if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                        {
                            if (Aluno.Alterar(Id.Value, formulario["nome"], System.DateTime.Parse(formulario["nascimento"])))
                            {
                                return RedirectToAction("Index", "Aluno");
                            }
                            else
                            {
                                ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                                ViewBag.HideScreen = true;
                                return View();
                            }
                        }

                        if (Aluno.Incluir(formulario["nome"], System.DateTime.Parse(formulario["nascimento"])))
                        {
                            return RedirectToAction("Index", "Aluno");
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
                    ViewBag.Aluno = Aluno.Consultar(Id.Value);

                    if (ViewBag.Aluno == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }
                }
                else
                {
                    ViewBag.Aluno = new Aluno();
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
                        if (Aluno.Desativar(Id.Value))
                        {
                            return RedirectToAction("Index", "Aluno");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    ViewBag.Aluno = Aluno.Consultar(Id.Value);

                    if (ViewBag.Aluno == null)
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