using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class DisciplinaController : Controller
    {
        public ActionResult Index(DisciplinaView view)
        {
            ModelState.Clear();

            try
            {
                var Cursos = Course.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    return View();
                }

                view.Cursos = new SelectList(Cursos, "IdCurso", "Nome");

                if (view.CursoSelecionado != null)
                    view.Curso = new Course { IdCurso = view.CursoSelecionado };

                ViewBag.Disciplinas = Class.Listar(view);

                if (ViewBag.Disciplinas == null)
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
        public ActionResult Edit(DisciplinaView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                var Cursos = Course.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
                }

                view.Cursos = new SelectList(Cursos, "IdCurso", "Nome");

                if (!Equals(view.IdDisciplina, System.Guid.Empty))
                {
                    var disciplina = Class.Consultar(view.IdDisciplina);

                    if (disciplina == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    view = DisciplinaView.Converter(disciplina);
                    view.Cursos = new SelectList(Cursos, "IdCurso", "Nome", view.Curso.IdCurso);
                    view.CursoSelecionado = view.Curso.IdCurso;
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View(view);
        }

        [HttpPost]
        public ActionResult Update(DisciplinaView view)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    view.Curso = new Course { IdCurso = view.CursoSelecionado };
                    view.Nome = view.Nome.Trim();
                    view.Descricao = view.Descricao.Trim();

                    if (!Equals(view.IdDisciplina, System.Guid.Empty))
                    {
                        if (Class.Alterar(view))
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
                        if (Class.Incluir(view))
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

                var Cursos = Course.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
                }

                view.Cursos = new SelectList(Cursos, "IdCurso", "Nome");
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        public ActionResult Delete(DisciplinaView view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdDisciplina, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Class.Desativar(view.IdDisciplina))
                        {
                            return RedirectToAction("Index", "Disciplina");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    var disciplina = Class.Consultar(view.IdDisciplina);

                    if (disciplina == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    }

                    view = DisciplinaView.Converter(disciplina);
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

    public class DisciplinaView : Class
    {
        public SelectList Cursos { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid CursoSelecionado { get; set; }

        public static DisciplinaView Converter(Class item)
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