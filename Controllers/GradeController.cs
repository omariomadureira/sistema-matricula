using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class GradeController : Controller
    {
        public ActionResult Index(GradeIndexView view)
        {
            ModelState.Clear();

            try
            {
                var Semestres = Semestre.Listar();

                if (Semestres == null)
                {
                    ViewBag.Message = "Não foi possível listar os semestres. Erro de execução.";
                    return View();
                }

                var Cursos = Curso.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    return View();
                }

                var StatusGrade = DisciplinaSemestre.StatusGrade();

                if (StatusGrade == null)
                {
                    ViewBag.Message = "Não foi possível listar os itens da lista. Erro de execução.";
                    return View();
                }

                view.slSemestres = new SelectList(Semestres, "IdSemestre", "Nome");
                view.slCursos = new SelectList(Cursos, "IdCurso", "Nome");
                view.slStatusGrade = new SelectList(StatusGrade);

                ViewBag.Grades = DisciplinaSemestre.ListarGrade(view.SemestreSelecionado, view.CursoSelecionado, view.StatusGradeSelecionado, view.PalavraChave);

                if (ViewBag.Grades == null)
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
        public ActionResult Edit(GradeEditView view)
        {
            /*
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                view.slPeriodos = new SelectList(Grade.Periodos());

                if (!Equals(view.IdGrade, System.Guid.Empty))
                {
                    var semestre = Grade.Consultar(view.IdGrade);

                    if (semestre == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    view = GradeView.Converter(semestre);
                    view.slPeriodos = new SelectList(Grade.Periodos(), semestre.Periodo.Trim());
                    view.PeriodoSelecionado = semestre.Periodo.Trim();
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View(view);
            */
            return View();
        }

        [HttpPost]
        public ActionResult Update(GradeEditView view)
        {
            /*
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    view.Periodo = view.PeriodoSelecionado.Trim();

                    if (!Equals(view.IdGrade, System.Guid.Empty))
                    {
                        if (Grade.Alterar(view))
                        {
                            return RedirectToAction("Index", "Grade");
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
                        if (Grade.Incluir(view))
                        {
                            return RedirectToAction("Index", "Grade");
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

                view.slPeriodos = new SelectList(Grade.Periodos());
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
            */
            return View();
        }

        public ActionResult Delete(GradeEditView view, bool? Delete)
        {
            /*
            try
            {
                if (!Equals(view.IdGrade, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Grade.Desativar(view.IdGrade))
                        {
                            return RedirectToAction("Index", "Grade");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    var semestre = Grade.Consultar(view.IdGrade);

                    if (semestre == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    }

                    view = GradeView.Converter(semestre);
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
            */
            return View();
        }
    }

    public class GradeIndexView
    {
        public SelectList slSemestres { get; set; }
        public SelectList slCursos { get; set; }
        public SelectList slStatusGrade { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid? SemestreSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid? CursoSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string StatusGradeSelecionado { get; set; }
        public string PalavraChave { get; set; }
    }

    public class GradeEditView : DisciplinaSemestre
    {
        public SelectList slSemestres { get; set; }
        public SelectList slDisciplinas { get; set; }
        public SelectList slProfessores { get; set; }
        public SelectList slCursos { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string DiaSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string HorarioSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid SemestreSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid DisciplinaSelecionada { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid ProfessorSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid CursoSelecionado { get; set; }

        public static GradeEditView Converter(DisciplinaSemestre a)
        {
            try
            {
                return new GradeEditView()
                {
                    IdDisciplinaSemestre = a.IdDisciplinaSemestre,
                    Disciplina = a.Disciplina,
                    Semestre = a.Semestre,
                    Professor = a.Professor,
                    DiaSemana = a.DiaSemana,
                    Horario = a.Horario,
                    Status = a.Status,
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };
            }
            catch
            {
                return null;
            }
        }
    }
}