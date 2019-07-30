using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class CursoController : Controller
    {
        public ActionResult Index(FormCollection formulario)
        {
            ViewBag.Cursos = formulario.Count > 0 && !string.IsNullOrWhiteSpace(formulario["pesquisa"]) ? Curso.Listar(formulario["pesquisa"]) : Curso.Listar(string.Empty);

            return View();
        }

        public ActionResult Edit(FormCollection formulario, System.Guid? Id)
        {
            Curso curso = new Curso();

            if (formulario.Count > 0 && !string.IsNullOrWhiteSpace(formulario["nome"]))
            {
                if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                {
                    if (Curso.Alterar(Id.Value, formulario["nome"], formulario["descricao"], int.Parse(formulario["creditos"])))
                        return RedirectToAction("Index", "Curso");
                }

                if (Curso.Incluir(formulario["nome"], formulario["descricao"], int.Parse(formulario["creditos"])))
                    return RedirectToAction("Index", "Curso");
            }

            if (Id.HasValue && !System.Guid.Equals(Id, System.Guid.Empty))
                curso = Curso.Consultar(Id.Value);

            ViewBag.Curso = curso;

            return View();
        }

        public ActionResult Delete(System.Guid Id, bool? Delete)
        {
            Curso curso = new Curso();

            if (!System.Guid.Equals(Id, System.Guid.Empty))
            {
                if (Delete.HasValue && Delete.Value && Curso.Desativar(Id))
                {
                    return RedirectToAction("Index", "Curso");
                }

                curso = Curso.Consultar(Id);
            }

            ViewBag.Curso = curso;

            return View();
        }
    }
}