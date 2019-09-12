using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class GridController : Controller
    {
        public ActionResult Index(GradeIndexView view)
        {
            ModelState.Clear();

            try
            {
                var Semestres = Semester.Listar();

                if (Semestres == null)
                {
                    ViewBag.Message = "Não foi possível listar os semestres. Erro de execução.";
                    return View();
                }

                var Cursos = Course.Listar();

                if (Cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    return View();
                }

                var StatusGrade = Grid.StatusGrade();

                if (StatusGrade == null)
                {
                    ViewBag.Message = "Não foi possível listar os itens da lista. Erro de execução.";
                    return View();
                }

                view.slSemestres = new SelectList(Semestres, "IdSemestre", "Nome");
                view.slCursos = new SelectList(Cursos, "IdCurso", "Nome");
                view.slStatusGrade = new SelectList(StatusGrade);

                ViewBag.Grades = Grid.ListarCursos(view.SemestreSelecionado, view.CursoSelecionado, view.StatusGradeSelecionado, view.PalavraChave);

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

        public ActionResult Start()
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                Grid filtros = new Grid()
                {
                    Status = Grid.DISCIPLINA_CADASTRADA
                };

                ViewBag.Grades = Grid.Listar(filtros);

                if (ViewBag.Grades == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                    ViewBag.HideScreen = true;
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
        public ActionResult UpdateState()
        {
            ViewBag.HideScreen = false;

            try
            {
                Grid filtros = new Grid()
                {
                    Status = Grid.DISCIPLINA_CADASTRADA
                };

                Grid[] lista = Grid.Listar(filtros);

                if (lista == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View("Start");
                }

                foreach (Grid item in lista)
                {
                    item.Status = Grid.DISCIPLINA_LIBERADA;

                    if (Grid.Alterar(item))
                    {
                        continue;
                    }
                    else
                    {
                        ViewBag.Message = "Não foi possível realizar a liberação das disciplinas para matrícula. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Start");
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Start");
        }

        public ActionResult List(GradeIndexView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (view.SemestreSelecionado != null && view.CursoSelecionado != null
                    && !Equals(view.SemestreSelecionado, System.Guid.Empty) && !Equals(view.CursoSelecionado, System.Guid.Empty))
                {
                    var semestres = Semester.Listar();

                    if (semestres == null)
                    {
                        ViewBag.Message = "Não foi possível listar os semestres. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    var cursos = Course.Listar();

                    if (cursos == null)
                    {
                        ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    view.slSemestres = new SelectList(semestres, "IdSemestre", "Nome", view.SemestreSelecionado);
                    view.slCursos = new SelectList(cursos, "IdCurso", "Nome", view.CursoSelecionado);

                    Grid filtros = new Grid()
                    {
                        Semestre = new Semester() { IdSemestre = view.SemestreSelecionado.Value },
                        Disciplina = new Class() { Curso = new Course { IdCurso = view.CursoSelecionado.Value } }
                    };

                    ViewBag.Grades = Grid.Listar(filtros);

                    if (ViewBag.Grades == null)
                    {
                        ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                        ViewBag.HideScreen = true;
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível realizar a solicitação. Identificação inválida.";
                    ViewBag.HideScreen = true;
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult EditAll(GradeEditAllView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(view.IdSemestre, System.Guid.Empty) && !Equals(view.IdCurso, System.Guid.Empty))
                {
                    var semestre = Semester.Consultar(view.IdSemestre);
                    var curso = Course.Consultar(view.IdCurso);

                    if (semestre != null && curso != null)
                    {
                        var professores = Teacher.Listar();

                        if (professores == null)
                        {
                            ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        var horarios = Grid.Horarios(semestre.Periodo.Trim());

                        if (horarios == null)
                        {
                            ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        var dias = Grid.Dias();

                        if (dias == null)
                        {
                            ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        Grid filtro = new Grid()
                        {
                            Semestre = semestre,
                            Disciplina = new Class { Curso = curso }
                        };

                        Grid[] lista = Grid.Listar(filtro, true);

                        if (lista.Length == 0)
                        {
                            ViewBag.Message = "Não foi possível localizar as disciplinas. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        view.itens = GradeEditAllView.Converter(lista, professores, horarios, dias);
                        view.NomeCurso = curso.Nome;
                        view.NomeSemestre = semestre.Nome;
                    }
                    else
                    {
                        ViewBag.Message = "Não foi possível realizar a solicitação. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível realizar a solicitação. Identificação inválida.";
                    ViewBag.HideScreen = true;
                    return View();
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
        public ActionResult UpdateAll(GradeEditAllView view)
        {
            ViewBag.HideScreen = false;

            for (int i = 0; i < view.itens.Length; i++)
            {
                ModelState[string.Format("itens[{0}].Disciplina.Nome", i)].Errors.Clear();
                ModelState[string.Format("itens[{0}].Disciplina.Descricao", i)].Errors.Clear();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (view.itens != null && view.itens.Length > 0)
                    {
                        foreach (GradeEditView item in view.itens)
                        {
                            item.DiaSemana = item.DiaSelecionado.Trim();
                            item.Horario = item.HorarioSelecionado.Trim();
                            item.Professor = new Teacher() { IdProfessor = item.ProfessorSelecionado };

                            if (!Equals(item.IdDisciplinaSemestre, System.Guid.Empty))
                            {
                                if (Grid.Alterar(item))
                                {
                                    continue;
                                }
                                else
                                {
                                    ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                                    ViewBag.HideScreen = true;
                                    return View("EditAll");
                                }
                            }
                            else
                            {
                                if (Grid.Incluir(item))
                                {
                                    continue;
                                }
                                else
                                {
                                    ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                                    ViewBag.HideScreen = true;
                                    return View("EditAll");
                                }
                            }
                        }

                        return RedirectToAction("Index", "Grade");
                    }
                    else
                    {
                        ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível atualizar os registros. Revise o preenchimento dos campos.";
                }

                var semestre = Semester.Consultar(view.IdSemestre);
                var curso = Course.Consultar(view.IdCurso);

                if (semestre != null && curso != null)
                {
                    var professores = Teacher.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }

                    var horarios = Grid.Horarios(semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }

                    var dias = Grid.Dias();

                    if (dias == null)
                    {
                        ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }

                    view.itens = GradeEditAllView.Converter(view.itens, professores, horarios, dias);
                    view.NomeCurso = curso.Nome;
                    view.NomeSemestre = semestre.Nome;
                }
                else
                {
                    ViewBag.Message = "Não foi possível realizar a solicitação. Identificação inválida.";
                    ViewBag.HideScreen = true;
                    return View("EditAll");
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("EditAll", view);
        }

        [HttpGet]
        public ActionResult Edit(GradeEditView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                view.Semestre = Semester.Consultar(view.IdSemestre);

                if (view.Semestre != null)
                {
                    var professores = Teacher.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    var horarios = Grid.Horarios(view.Semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    var dias = Grid.Dias();

                    if (dias == null)
                    {
                        ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    view.Disciplina = new Class()
                    {
                        Curso = new Course() { IdCurso = view.IdCurso }
                    };

                    var disciplinas = Class.Listar(view.Disciplina);

                    if (disciplinas == null)
                    {
                        ViewBag.Message = "Não foi possível listar os disciplinas. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    view.slProfessores = new SelectList(professores, "IdProfessor", "Nome");
                    view.slHorarios = new SelectList(horarios);
                    view.slDias = new SelectList(dias);
                    view.slDisciplinas = new SelectList(disciplinas, "IdDisciplina", "Nome");

                    if (!Equals(view.IdDisciplinaSemestre, System.Guid.Empty))
                    {
                        var disciplina = Grid.Consultar(view.IdDisciplinaSemestre);

                        if (disciplina == null)
                        {
                            ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                            ViewBag.HideScreen = true;
                            return View(view);
                        }

                        view = GradeEditView.Converter(disciplina);
                        view.slProfessores = new SelectList(professores, "IdProfessor", "Nome", disciplina.Professor.IdProfessor);
                        view.ProfessorSelecionado = disciplina.Professor.IdProfessor;
                        view.slHorarios = new SelectList(horarios, disciplina.Horario.Trim());
                        view.HorarioSelecionado = view.Horario;
                        view.slDias = new SelectList(dias, disciplina.DiaSemana.Trim());
                        view.DiaSemana = disciplina.DiaSemana.Trim();
                        view.slDisciplinas = new SelectList(disciplinas, "IdDisciplina", "Nome", disciplina.Disciplina.IdDisciplina);
                        view.DisciplinaSelecionada = disciplina.Disciplina.IdDisciplina;
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    ViewBag.HideScreen = true;
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
        public ActionResult Update(GradeEditView view)
        {
            ViewBag.HideScreen = false;

            try
            {
                view.Semestre = Semester.Consultar(view.IdSemestre);

                if (ModelState.IsValid)
                {
                    view.Disciplina = new Class() { IdDisciplina = view.DisciplinaSelecionada };
                    view.DiaSemana = view.DiaSelecionado.Trim();
                    view.Horario = view.HorarioSelecionado.Trim();
                    view.Professor = new Teacher() { IdProfessor = view.ProfessorSelecionado };

                    if (!Equals(view.IdDisciplinaSemestre, System.Guid.Empty))
                    {
                        if (Grid.Alterar(view))
                        {
                            return RedirectToAction("List", "Grade", new GradeIndexView { CursoSelecionado = view.IdCurso, SemestreSelecionado = view.IdSemestre });
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View("Edit", view);
                        }
                    }
                    else
                    {
                        if (Grid.Incluir(view))
                        {
                            return RedirectToAction("List", "Grade", new GradeIndexView { CursoSelecionado = view.IdCurso, SemestreSelecionado = view.IdSemestre });
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View("Edit", view);
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível atualizar o registro. Revise o preenchimento dos campos.";
                }

                if (view.Semestre != null)
                {
                    var professores = Teacher.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    var horarios = Grid.Horarios(view.Semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    var dias = Grid.Dias();

                    if (dias == null)
                    {
                        ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    view.Disciplina = new Class()
                    {
                        Curso = new Course() { IdCurso = view.IdCurso }
                    };

                    var disciplinas = Class.Listar(view.Disciplina);

                    if (disciplinas == null)
                    {
                        ViewBag.Message = "Não foi possível listar os disciplinas. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    view.slProfessores = new SelectList(professores, "IdProfessor", "Nome", view.ProfessorSelecionado);
                    view.slHorarios = new SelectList(horarios, view.HorarioSelecionado);
                    view.slDias = new SelectList(dias, view.DiaSelecionado);
                    view.slDisciplinas = new SelectList(disciplinas, "IdDisciplina", "Nome", view.DisciplinaSelecionada);
                }
                else
                {
                    ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    ViewBag.HideScreen = true;
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        public ActionResult Delete(Grid view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdDisciplinaSemestre, System.Guid.Empty))
                {
                    view = Grid.Consultar(view.IdDisciplinaSemestre);

                    if (view == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        return View(view);
                    }

                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Grid.Desativar(view.IdDisciplinaSemestre))
                        {
                            return RedirectToAction("List", "Grade", new GradeIndexView { CursoSelecionado = view.Disciplina.Curso.IdCurso, SemestreSelecionado = view.Semestre.IdSemestre });
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                        }
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

    public class GridIndexView
    {
        public SelectList SemesterSelectList { get; set; }
        public SelectList CourseSelectList { get; set; }
        public SelectList StatusSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid? SemesterSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid? CourseSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string StatusSelected { get; set; }
        public string Search { get; set; }
    }

    public class GridEditView : Grid
    {
        public System.Guid IdSemester { get; set; }
        public System.Guid IdCourse { get; set; }
        public SelectList TeacherSelectList { get; set; }
        public SelectList TimeSelectList { get; set; }
        public SelectList WeekdaySelectList { get; set; }
        public SelectList ClassSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid TeacherSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string TimeSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string WeekdaySelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid ClassSelected { get; set; }
    }

    public class GradeEditAllView
    {
        public System.Guid IdSemestre { get; set; }
        public string NomeSemestre { get; set; }
        public System.Guid IdCurso { get; set; }
        public string NomeCurso { get; set; }
        public GradeEditView[] itens { get; set; }

        public static GradeEditView[] Converter(Grid[] lista, System.Collections.IEnumerable Professores, System.Collections.IEnumerable Horarios, System.Collections.IEnumerable Dias)
        {
            try
            {
                GradeEditView[] novaLista = new GradeEditView[lista.Length];

                for (int i = 0; i < lista.Length; i++)
                {
                    GradeEditView item = GradeEditView.Converter(lista[i]);
                    item.slProfessores = new SelectList(Professores, "IdProfessor", "Nome", item.ProfessorSelecionado);
                    item.slHorarios = new SelectList(Horarios, item.HorarioSelecionado);
                    item.slDias = new SelectList(Dias, item.DiaSelecionado);

                    novaLista.SetValue(item, i);
                }

                return novaLista;
            }
            catch
            {
                return null;
            }
        }
    }
}