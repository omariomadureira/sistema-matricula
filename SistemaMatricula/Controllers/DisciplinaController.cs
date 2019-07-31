using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class DisciplinaController : Controller
    {
        public ActionResult Index(FormCollection formulario)
        {
            try
            {
                if (formulario.Count > 0)
                {
                    string[] filtros = new string[2];

                    if (!string.IsNullOrWhiteSpace(formulario["palavra"]))
                        filtros.SetValue(formulario["palavra"], 0);

                    if (!string.IsNullOrWhiteSpace(formulario["curso"]))
                        filtros.SetValue(formulario["curso"], 1);

                    ViewBag.Disciplinas = Disciplina.Listar(filtros);
                    ViewBag.Palavra = formulario["palavra"];
                    ViewBag.IdCurso = formulario["curso"];
                }
                else
                {
                    ViewBag.Disciplinas = Disciplina.Listar();
                }

                if (ViewBag.Disciplinas == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                    return View();
                }

                ViewBag.Cursos = Curso.Listar();

                if (ViewBag.Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    return View();
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
                        && !string.IsNullOrWhiteSpace(formulario["curso"])
                        && System.Guid.TryParse(formulario["curso"], out _)
                        && !string.IsNullOrWhiteSpace(formulario["descricao"]))
                    {
                        if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                        {
                            if (Disciplina.Alterar(Id.Value, System.Guid.Parse(formulario["curso"]), formulario["nome"], formulario["descricao"]))
                            {
                                return RedirectToAction("Index", "Disciplina");
                            }
                            else
                            {
                                ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                                ViewBag.HideScreen = true;
                                return View();
                            }
                        }

                        if (Disciplina.Incluir(System.Guid.Parse(formulario["curso"]), formulario["nome"], formulario["descricao"]))
                        {
                            return RedirectToAction("Index", "Disciplina");
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
                    ViewBag.Disciplina = Disciplina.Consultar(Id.Value);

                    if (ViewBag.Disciplina == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Disciplina = new Disciplina();
                }

                ViewBag.Cursos = Curso.Listar();

                if (ViewBag.Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
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
                        if (Disciplina.Desativar(Id.Value))
                        {
                            return RedirectToAction("Index", "Disciplina");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    ViewBag.Disciplina = Disciplina.Consultar(Id.Value);

                    if (ViewBag.Disciplina == null)
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
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