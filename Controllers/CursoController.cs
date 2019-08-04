using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class CursoController : Controller
    {
        public ActionResult Index(Curso item)
        {
            ModelState.Clear();

            try
            {
                ViewBag.Cursos = Curso.Listar(item.Nome);

                if (ViewBag.Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(item);
        }

        [HttpGet]
        public ActionResult Edit(Curso item)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(item.IdCurso, System.Guid.Empty))
                {
                    item = Curso.Consultar(item.IdCurso);

                    if (item == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }

                    return View(item);
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
        public ActionResult Update(Curso item)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    if (!Equals(item.IdCurso, System.Guid.Empty))
                    {
                        if (Curso.Alterar(item))
                        {
                            return RedirectToAction("Index", "Curso");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                        }
                    }
                    else
                    {
                        if (Curso.Incluir(item))
                        {
                            return RedirectToAction("Index", "Curso");
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

            return View("Edit", item);
        }

        public ActionResult Delete(Curso item, bool? Delete)
        {
            try
            {
                if (!Equals(item.IdCurso, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Curso.Desativar(item.IdCurso))
                        {
                            return RedirectToAction("Index", "Curso");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    item = Curso.Consultar(item.IdCurso);

                    if (item == null)
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

            return View(item);
        }
    }
}