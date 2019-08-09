using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    public class DisciplinaController : Controller
    {
        public ActionResult Index(DisciplinaView item)
        {
            ModelState.Clear();

            try
            {
                var Cursos = Curso.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    return View();
                }

                item.Cursos = new SelectList(Cursos, "IdCurso", "Nome");

                if (item.CursoSelecionado != null)
                    item.Curso = new Curso { IdCurso = item.CursoSelecionado };

                ViewBag.Disciplinas = Disciplina.Listar(item);

                if (ViewBag.Disciplinas == null)
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
        public ActionResult Edit(DisciplinaView item)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                var Cursos = Curso.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
                }

                item.Cursos = new SelectList(Cursos, "IdCurso", "Nome");

                if (!Equals(item.IdDisciplina, System.Guid.Empty))
                {
                    var disciplina = Disciplina.Consultar(item.IdDisciplina);

                    if (disciplina == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    item = DisciplinaView.Converter(disciplina);
                    item.Cursos = new SelectList(Cursos, "IdCurso", "Nome", item.Curso.IdCurso);
                    item.CursoSelecionado = item.Curso.IdCurso;
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View(item);
        }

        [HttpPost]
        public ActionResult Update(DisciplinaView item)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    item.Curso = new Curso { IdCurso = item.CursoSelecionado };

                    if (!Equals(item.IdDisciplina, System.Guid.Empty))
                    {
                        if (Disciplina.Alterar(item))
                        {
                            return RedirectToAction("Index", "Disciplina");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View("Edit");
                        }
                    }
                    else
                    {
                        if (Disciplina.Incluir(item))
                        {
                            return RedirectToAction("Index", "Disciplina");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View("Edit");
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível atualizar o registro. Revise o preenchimento dos campos.";
                }

                var Cursos = Curso.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
                }

                item.Cursos = new SelectList(Cursos, "IdCurso", "Nome");
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", item);
        }

        public ActionResult Delete(DisciplinaView item, bool? Delete)
        {
            try
            {
                if (!Equals(item.IdDisciplina, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Disciplina.Desativar(item.IdDisciplina))
                        {
                            return RedirectToAction("Index", "Disciplina");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    var disciplina = Disciplina.Consultar(item.IdDisciplina);

                    if (disciplina == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    }

                    item = DisciplinaView.Converter(disciplina);
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

    public class DisciplinaView : Disciplina
    {
        public SelectList Cursos { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid CursoSelecionado { get; set; }

        public static DisciplinaView Converter(Disciplina item)
        {
            try
            {
                return new DisciplinaView()
                {
                    IdDisciplina = item.IdDisciplina,
                    Nome = item.Nome,
                    Descricao = item.Descricao,
                    Curso = item.Curso,
                    CadastroData = item.CadastroData,
                    CadastroPor = item.CadastroPor,
                    ExclusaoData = item.ExclusaoData,
                    ExclusaoPor = item.ExclusaoPor
                };
            }
            catch
            {
                return null;
            }
        }
    }
}