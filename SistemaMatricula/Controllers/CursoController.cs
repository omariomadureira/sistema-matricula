using SistemaMatricula.Models;
using System.Web.Mvc;
using System.Collections.Generic;

namespace SistemaMatricula.Controllers
{
    public class CursoController : Controller
    {
        public ActionResult Index(FormCollection formulario)
        {
            List<Curso> cursos = new List<Curso>();
            ViewBag.Pesquisa = string.Empty;

            try
            {
                if (formulario.Count > 0 && !string.IsNullOrWhiteSpace(formulario["pesquisa"]))
                {
                    cursos = Curso.Listar(formulario["pesquisa"]);
                    ViewBag.Pesquisa = formulario["pesquisa"];
                }
                else
                {
                    cursos = Curso.Listar(string.Empty);
                }

                if (cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            ViewBag.Cursos = cursos;

            return View();
        }

        public ActionResult Edit(FormCollection formulario, System.Guid? Id)
        {
            ViewBag.Curso = new Curso();
            ViewBag.HideScreen = false;

            try
            {
                if (formulario.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(formulario["nome"])
                        && !string.IsNullOrWhiteSpace(formulario["creditos"])
                        && int.TryParse(formulario["creditos"], out _)
                        && !string.IsNullOrWhiteSpace(formulario["descricao"]))
                    {
                        if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                        {
                            if (Curso.Alterar(Id.Value, formulario["nome"], formulario["descricao"], int.Parse(formulario["creditos"])))
                            {
                                return RedirectToAction("Index", "Curso");
                            }
                            else
                            {
                                ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                                ViewBag.HideScreen = true;
                                return View();
                            }
                        }

                        if (Curso.Incluir(formulario["nome"], formulario["descricao"], int.Parse(formulario["creditos"])))
                        {
                            return RedirectToAction("Index", "Curso");
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
                    Curso curso = Curso.Consultar(Id.Value);

                    if (curso == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }

                    ViewBag.Curso = curso;
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
            Curso curso = new Curso();

            try
            {
                if (Id.HasValue && !System.Guid.Equals(Id.Value, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Curso.Desativar(Id.Value))
                        {
                            return RedirectToAction("Index", "Curso");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    curso = Curso.Consultar(Id.Value);

                    if (curso == null)
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

            ViewBag.Curso = curso;

            return View();
        }
    }
}