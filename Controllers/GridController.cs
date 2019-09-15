using SistemaMatricula.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
    public class GridController : Controller
    {
        public ActionResult Index(GridIndexView view)
        {
            try
            {
                ModelState.Clear();

                var courses = Course.List(sort: by => by.Name);

                if (courses == null)
                    throw new Exception("Os cursos não foram listados");

                var semesters = Semester.List();

                if (semesters == null)
                    throw new Exception("Os semestres não foram listados");

                if (Equals(view.CourseSelected, Guid.Empty))
                    view.CourseSelected = courses[0].IdCourse;

                if (Equals(view.SemesterSelected, Guid.Empty))
                    view.SemesterSelected = semesters[0].IdSemester;

                view.CourseSelectList = new SelectList(courses, "IdCourse", "Name", view.CourseSelected);
                view.SemesterSelectList = new SelectList(semesters, "IdSemester", "Name", view.SemesterSelected);

                var semester = semesters.Find(x => x.IdSemester == view.SemesterSelected);

                view.Class = new Class { Course = new Course() { IdCourse = view.CourseSelected } };
                view.Semester = semester;

                ViewBag.List = Grid.List(view);

                if (ViewBag.List == null)
                    throw new Exception("As grades não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        public ActionResult Edit(GridEditView view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                if (Equals(view.IdGrid, Guid.Empty))
                {
                    if (Equals(view.IdSemester, Guid.Empty))
                        throw new Exception("Parâmetro 'IdSemester' inválido");

                    if (Equals(view.IdCourse, Guid.Empty))
                        throw new Exception("Parâmetro 'IdCourse' inválido");
                }

                var semester = Semester.Find(view.IdSemester);

                if (semester == null)
                    throw new Exception("Semestre não encontrado");

                Class filter = new Class()
                {
                    Course = new Course() { IdCourse = view.IdCourse }
                };

                var classList = Class.List(filter, by => by.Name);

                if (classList == null)
                    throw new Exception("As disciplinas não foram listadas");

                var teachers = Teacher.List(sort: by => by.Name);

                if (teachers == null)
                    throw new Exception("Os professores não foram listados");

                view.TeacherSelectList = new SelectList(teachers, "IdTeacher", "Name");
                view.ClassSelectList = new SelectList(classList, "IdClass", "Name");
                view.TimeSelectList = new SelectList(Grid.TimeList(semester.Period.Trim()));
                view.WeekdaySelectList = new SelectList(Grid.WeekdayList(), "Weekday", "WeekdayName");
                view.StatusSelectList = new SelectList(Grid.StatusList(), Grid.REGISTERED);
                view.StatusSelected = Grid.REGISTERED;

                if (error)
                {
                    ViewBag.Message = "Não foi possível salvar o registro. Analise os erros.";
                    return View("Edit", view);
                }

                ModelState.Clear();

                if (Equals(view.IdGrid, Guid.Empty))
                    return View("Edit", view);

                var item = Grid.Find(view.IdGrid);

                if (item == null)
                    throw new Exception("Grade não encontrada");

                if (item.Semester.IsActual == false)
                    throw new Exception("Semestre não é atual");

                view.TeacherSelectList = new SelectList(teachers, "IdTeacher", "Name", item.Teacher.IdTeacher);
                view.TeacherSelected = item.Teacher.IdTeacher;
                view.ClassSelectList = new SelectList(classList, "IdClass", "Name", item.Class.IdClass);
                view.ClassSelected = item.Class.IdClass;
                view.TimeSelectList = new SelectList(Grid.TimeList(semester.Period), item.Time.Trim());
                view.TimeSelected = item.Time.Trim();
                view.WeekdaySelectList = new SelectList(Grid.WeekdayList(), "Weekday", "WeekdayName", item.Weekday);
                view.WeekdaySelected = item.Weekday;
                view.StatusSelectList = new SelectList(Grid.StatusList(), item.Status.Trim());
                view.StatusSelected = item.Status.Trim();
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        public ActionResult Update(GridEditView view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                var semester = Semester.Find(view.IdSemester);

                if (semester.IsActual == false)
                    throw new Exception("Semestre não é atual");

                view.Class = new Class() { IdClass = view.ClassSelected };
                view.Weekday = view.WeekdaySelected;
                view.Time = view.TimeSelected.Trim();
                view.Teacher = new Teacher() { IdTeacher = view.TeacherSelected };
                view.Semester = new Semester() { IdSemester = view.IdSemester };
                view.Status = view.StatusSelected.Trim();

                if (Equals(view.IdGrid, Guid.Empty))
                {
                    var insert = Grid.Add(view);

                    if (insert == false)
                        return Edit(view, true);

                    return RedirectToAction("Index", "Grid",
                        new GridIndexView { CourseSelected = view.IdCourse, SemesterSelected = view.Semester.IdSemester });
                }

                var update = Grid.Update(view);

                if (update == false)
                    return Edit(view, true);

                return RedirectToAction("Index", "Grid",
                    new GridIndexView { CourseSelected = view.IdCourse, SemesterSelected = view.Semester.IdSemester });
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Update", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit");
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            try
            {
                if (Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                var grid = Grid.Find(id);

                if (grid == null)
                    throw new Exception(string.Format("Grade {0} não existe", id));

                if (grid.Semester.IsActual == false)
                    throw new Exception("Semestre não é atual");

                var deleted = Grid.Delete(id);

                if (deleted == false)
                    throw new Exception("Grade não deletada");

                return RedirectToAction("Index", "Grid");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }

        public ActionResult Start()
        {
            try
            {
                Grid filters = new Grid()
                {
                    Status = Grid.REGISTERED
                };

                ViewBag.List = Grid.List(filters);

                if (ViewBag.List == null)
                    throw new Exception("As grades não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Start", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Release()
        {
            try
            {
                var released = Grid.Release();

                if (released == false)
                    throw new Exception("Grade não liberada");

                return RedirectToAction("Start");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Release", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View("Start");
        }

        public ActionResult Finish()
        {
            try
            {
                Grid filters = new Grid()
                {
                    Status = Grid.RELEASED
                };

                ViewBag.List = Grid.List(filters);

                if (ViewBag.List == null)
                    throw new Exception("As grades não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Finish", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Close()
        {
            try
            {
                var closed = Grid.Close();

                if (closed == false)
                    throw new Exception("Grade não encerrada");

                return RedirectToAction("Finish");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Close", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View("Finish");
        }

        [HttpGet]
        public ActionResult Copy(GridCopyView view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                var semesters = Semester.List();

                if (semesters == null)
                    throw new Exception("Os semestres não foram listados");

                if (Equals(view.OrigenSelected, Guid.Empty))
                    view.OrigenSelected = semesters[0].IdSemester;

                if (Equals(view.DestinySelectList, Guid.Empty))
                    view.DestinySelected = semesters[0].IdSemester;

                view.OrigenSelectList = new SelectList(semesters, "IdSemester", "Name", view.OrigenSelected);
                view.DestinySelectList = new SelectList(semesters, "IdSemester", "Name", view.DestinySelected);

                if (error)
                {
                    ViewBag.Message = "Não foi possível realizar a matrícula. Analise os erros.";
                    return View("Copy");
                }

                ModelState.Clear();
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Copy.Get", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Copy", view);
        }

        [HttpPost]
        public ActionResult Copy(GridCopyView view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Copy(view, true);

                var copy = Grid.Copy(view.OrigenSelected, view.DestinySelected);

                if (copy == false)
                    return Copy(view, true);

                return RedirectToAction("Index", "Grid");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.GridController.Copy.Post", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Copy");
        }

        /*
        [HttpGet]
        public ActionResult EditAll(GradeEditAllView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(view.IdSemestre, Guid.Empty) && !Equals(view.IdCurso, Guid.Empty))
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

                            if (!Equals(item.IdDisciplinaSemestre, Guid.Empty))
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
        */
    }

    public class GridIndexView : Grid
    {
        public SelectList SemesterSelectList { get; set; }
        public SelectList CourseSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid SemesterSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid CourseSelected { get; set; }
    }

    public class GridEditView : Grid
    {
        public Guid IdCourse { get; set; }
        public Guid IdSemester { get; set; }
        public SelectList TeacherSelectList { get; set; }
        public SelectList TimeSelectList { get; set; }
        public SelectList WeekdaySelectList { get; set; }
        public SelectList ClassSelectList { get; set; }
        public SelectList StatusSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid TeacherSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string TimeSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public int WeekdaySelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid ClassSelected { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string StatusSelected { get; set; }
    }

    public class GridCopyView
    {
        public SelectList OrigenSelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid OrigenSelected { get; set; }
        public SelectList DestinySelectList { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public Guid DestinySelected { get; set; }
    }
}