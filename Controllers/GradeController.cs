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

        public ActionResult List(GradeIndexView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (view.SemestreSelecionado != null && view.CursoSelecionado != null
                    && !Equals(view.SemestreSelecionado, System.Guid.Empty) && !Equals(view.CursoSelecionado, System.Guid.Empty))
                {
                    var semestres = Semestre.Listar();

                    if (semestres == null)
                    {
                        ViewBag.Message = "Não foi possível listar os semestres. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    var cursos = Curso.Listar();

                    if (cursos == null)
                    {
                        ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    view.slSemestres = new SelectList(semestres, "IdSemestre", "Nome", view.SemestreSelecionado);
                    view.slCursos = new SelectList(cursos, "IdCurso", "Nome", view.CursoSelecionado);

                    DisciplinaSemestre filtros = new DisciplinaSemestre()
                    {
                        Semestre = new Semestre() { IdSemestre = view.SemestreSelecionado.Value },
                        Disciplina = new Disciplina() { Curso = new Curso { IdCurso = view.CursoSelecionado.Value } }
                    };

                    ViewBag.Grades = DisciplinaSemestre.Listar(filtros);

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
                    var semestre = Semestre.Consultar(view.IdSemestre);
                    var curso = Curso.Consultar(view.IdCurso);

                    if (semestre != null && curso != null)
                    {
                        var professores = Professor.Listar();

                        if (professores == null)
                        {
                            ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        var horarios = DisciplinaSemestre.Horarios(semestre.Periodo.Trim());

                        if (horarios == null)
                        {
                            ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        var dias = DisciplinaSemestre.Dias();

                        if (dias == null)
                        {
                            ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                            ViewBag.HideScreen = true;
                            return View();
                        }

                        DisciplinaSemestre filtro = new DisciplinaSemestre()
                        {
                            Semestre = semestre,
                            Disciplina = new Disciplina { Curso = curso }
                        };

                        DisciplinaSemestre[] lista = DisciplinaSemestre.Listar(filtro, true);

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
                            item.Professor = new Professor() { IdProfessor = item.ProfessorSelecionado };

                            if (!Equals(item.IdDisciplinaSemestre, System.Guid.Empty))
                            {
                                if (DisciplinaSemestre.Alterar(item))
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
                                if (DisciplinaSemestre.Incluir(item))
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

                var semestre = Semestre.Consultar(view.IdSemestre);
                var curso = Curso.Consultar(view.IdCurso);

                if (semestre != null && curso != null)
                {
                    var professores = Professor.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }

                    var horarios = DisciplinaSemestre.Horarios(semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("EditAll");
                    }

                    var dias = DisciplinaSemestre.Dias();

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
                view.Semestre = Semestre.Consultar(view.IdSemestre);

                if (view.Semestre != null)
                {
                    var professores = Professor.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    var horarios = DisciplinaSemestre.Horarios(view.Semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    var dias = DisciplinaSemestre.Dias();

                    if (dias == null)
                    {
                        ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View(view);
                    }

                    view.Disciplina = new Disciplina()
                    {
                        Curso = new Curso() { IdCurso = view.IdCurso }
                    };

                    var disciplinas = Disciplina.Listar(view.Disciplina);

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
                        var disciplina = DisciplinaSemestre.Consultar(view.IdDisciplinaSemestre);

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
                view.Semestre = Semestre.Consultar(view.IdSemestre);

                if (ModelState.IsValid)
                {
                    view.Disciplina = new Disciplina() { IdDisciplina = view.DisciplinaSelecionada };
                    view.DiaSemana = view.DiaSelecionado.Trim();
                    view.Horario = view.HorarioSelecionado.Trim();
                    view.Professor = new Professor() { IdProfessor = view.ProfessorSelecionado };

                    if (!Equals(view.IdDisciplinaSemestre, System.Guid.Empty))
                    {
                        if (DisciplinaSemestre.Alterar(view))
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
                        if (DisciplinaSemestre.Incluir(view))
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
                    var professores = Professor.Listar();

                    if (professores == null)
                    {
                        ViewBag.Message = "Não foi possível listar os professores. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    var horarios = DisciplinaSemestre.Horarios(view.Semestre.Periodo.Trim());

                    if (horarios == null)
                    {
                        ViewBag.Message = "Não foi possível listar os horários. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    var dias = DisciplinaSemestre.Dias();

                    if (dias == null)
                    {
                        ViewBag.Message = "Não foi possível listar os dias da semana. Erro de execução.";
                        ViewBag.HideScreen = true;
                        return View("Edit", view);
                    }

                    view.Disciplina = new Disciplina()
                    {
                        Curso = new Curso() { IdCurso = view.IdCurso }
                    };

                    var disciplinas = Disciplina.Listar(view.Disciplina);

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

        public ActionResult Delete(DisciplinaSemestre view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdDisciplinaSemestre, System.Guid.Empty))
                {
                    view = DisciplinaSemestre.Consultar(view.IdDisciplinaSemestre);

                    if (view == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        return View(view);
                    }

                    if (Delete.HasValue && Delete.Value)
                    {
                        if (DisciplinaSemestre.Desativar(view.IdDisciplinaSemestre))
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
        public System.Guid IdSemestre { get; set; }
        public System.Guid IdCurso { get; set; }
        public SelectList slProfessores { get; set; }
        public SelectList slHorarios { get; set; }
        public SelectList slDias { get; set; }
        public SelectList slDisciplinas { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid ProfessorSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string HorarioSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string DiaSelecionado { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid DisciplinaSelecionada { get; set; }

        public static GradeEditView Converter(DisciplinaSemestre a)
        {
            try
            {
                GradeEditView item = new GradeEditView()
                {
                    IdSemestre = a.Semestre == null ? System.Guid.Empty : a.Semestre.IdSemestre,
                    IdCurso = a.Disciplina == null ? System.Guid.Empty : a.Disciplina.Curso.IdCurso,
                    IdDisciplinaSemestre = a.IdDisciplinaSemestre,
                    Disciplina = a.Disciplina,
                    Semestre = a.Semestre,
                    Professor = a.Professor,
                    DiaSemana = a.DiaSemana == null ? string.Empty : a.DiaSemana.Trim(),
                    DiaSelecionado = a.DiaSemana == null ? string.Empty : a.DiaSemana.Trim(),
                    Horario = a.Horario == null ? string.Empty : a.Horario.Trim(),
                    HorarioSelecionado = a.Horario == null ? string.Empty : a.Horario.Trim(),
                    Status = a.Status == null ? string.Empty : a.Status.Trim(),
                    CadastroData = a.CadastroData,
                    CadastroPor = a.CadastroPor,
                    ExclusaoData = a.ExclusaoData,
                    ExclusaoPor = a.ExclusaoPor
                };

                if (a.Professor != null)
                    item.ProfessorSelecionado = a.Professor.IdProfessor;

                return item;
            }
            catch
            {
                return null;
            }
        }
    }

    public class GradeEditAllView
    {
        public System.Guid IdSemestre { get; set; }
        public string NomeSemestre { get; set; }
        public System.Guid IdCurso { get; set; }
        public string NomeCurso { get; set; }
        public GradeEditView[] itens { get; set; }

        public static GradeEditView[] Converter(DisciplinaSemestre[] lista, System.Collections.IEnumerable Professores, System.Collections.IEnumerable Horarios, System.Collections.IEnumerable Dias)
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