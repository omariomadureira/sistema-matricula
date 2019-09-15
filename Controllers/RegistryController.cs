using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Controllers
{
    [Authorize]
    public class RegistryController : Controller
    {
        [Authorize(Roles = Models.User.ROLE_STUDENT)]
        public ActionResult Index(RegistryIndexView view)
        {
            try
            {
                ModelState.Clear();

                var semesters = Semester.List();

                if (semesters == null)
                    throw new Exception("Os semestres não foram listados");

                Student logged = Student.FindLoggedUser();

                if (logged == null)
                    throw new Exception("Aluno não encontrado");

                if (Equals(view.SemesterSelected, Guid.Empty))
                    view.SemesterSelected = semesters[0].IdSemester;

                view.SemesterSelectList = new SelectList(semesters, "IdSemester", "Name", view.SemesterSelected);

                Registry filters = new Registry()
                {
                    Student = logged,
                    Grid = new Grid() { Semester = new Semester() { IdSemester = view.SemesterSelected } }
                };

                ViewBag.List = Registry.List(filters);

                if (ViewBag.List == null)
                    throw new Exception("As matrículas não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.RegistryController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [Authorize(Roles = Models.User.ROLE_STUDENT)]
        public ActionResult Edit(RegistryEditView view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;

                var courses = Course.List(sort: by => by.Name);

                if (courses == null)
                    throw new Exception("Os cursos não foram listados");

                if (Equals(view.CourseSelected, Guid.Empty))
                    view.CourseSelected = courses[0].IdCourse;

                view.CourseSelectList = new SelectList(courses, "IdCourse", "Name", view.CourseSelected);

                Student logged = Student.FindLoggedUser();

                if (logged == null)
                    throw new Exception("Aluno não encontrado");

                var list = Registry.GridList(logged.IdStudent, view.CourseSelected);

                if (list == null)
                    throw new Exception("As grades não foram listadas.");

                view.Registries = Convert(view.Registries, list.ToArray());

                if (error)
                    ViewBag.Message = "Não foi possível realizar a matrícula. Analise os erros.";
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.RegistryController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        [Authorize(Roles = Models.User.ROLE_STUDENT)]
        public ActionResult Update(RegistryEditView view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                Student logged = Student.FindLoggedUser();

                if (logged == null)
                    throw new Exception("Aluno não encontrado");

                foreach (RegistryView item in view.Registries)
                {
                    if (item.FirstOption || item.SecondOption)
                    {
                        item.Student = logged;

                        if (item.SecondOption)
                            item.Alternative = true;

                        var insert = Registry.Add(item);

                        if (insert == false)
                        {
                            throw new Exception(
                                string.Format("Matrícula não incluída. Grade: {0} Aluno: {1}",
                                    item.Grid.IdGrid, item.Student.IdStudent));
                        }
                    }
                }

                return RedirectToAction("Index", "Registry");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.RegistryController.Update", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit");
        }

        [HttpGet]
        [Authorize(Roles = Models.User.ROLE_STUDENT)]
        public ActionResult Delete(Guid id)
        {
            try
            {
                if (Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro inválido");

                var deleted = Registry.Delete(id);

                if (deleted == false)
                    throw new Exception("Matrícula não deletada");

                return RedirectToAction("Index", "Registry");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.RegistryController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }

        private RegistryView[] Convert(RegistryView[] list, Registry[] itens)
        {
            if (itens.Length < 1)
                return new RegistryView[0];

            if (list == null)
                list = new RegistryView[itens.Length];

            for (int index = 0; index < itens.Length; index++)
            {
                if (list[index] == null)
                    list[index] = new RegistryView();

                if (itens[index].Grid != null)
                    list[index].Grid = itens[index].Grid;

                if (itens[index].Student != null)
                    list[index].Student = itens[index].Student;
            }

            return list;
        }

        [Authorize(Roles = Models.User.ROLE_TEACHER)]
        public ActionResult List(RegistryListView view)
        {
            try
            {
                ModelState.Clear();

                var semesters = Semester.List();

                if (semesters == null)
                    throw new Exception("Os semestres não foram listados");

                Teacher logged = Teacher.FindLoggedUser();

                if (logged == null)
                    throw new Exception("Professor não encontrado");

                var classList = Class.List(logged.IdTeacher);

                if (classList == null)
                    throw new Exception("As disciplinas não foram listadas");

                if (Equals(view.SemesterSelected, Guid.Empty))
                    view.SemesterSelected = semesters[0].IdSemester;

                if (Equals(view.ClassSelected, Guid.Empty))
                    view.ClassSelected = classList[0].IdClass;

                view.SemesterSelectList = new SelectList(semesters, "IdSemester", "Name", view.SemesterSelected);
                view.ClassSelectList = new SelectList(classList, "IdClass", "Name", view.ClassSelected);

                Registry filters = new Registry()
                {
                    Grid = new Grid()
                    {
                        Semester = new Semester() { IdSemester = view.SemesterSelected },
                        Teacher = new Teacher() { IdTeacher = logged.IdTeacher },
                        Class = new Class() { IdClass = view.ClassSelected }
                    }
                };

                ViewBag.List = Registry.List(filters);

                if (ViewBag.List == null)
                    throw new Exception("As matrículas não foram listadas");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.RegistryController.List", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }
    }

    public class RegistryIndexView
    {
        public SelectList SemesterSelectList { get; set; }
        public Guid SemesterSelected { get; set; }
    }

    public class RegistryView : Registry
    {
        public bool FirstOption { get; set; }
        public bool SecondOption { get; set; }
    }

    public class RegistryEditView
    {
        public SelectList CourseSelectList { get; set; }
        public Guid CourseSelected { get; set; }
        [CustomValidation(typeof(Registry), "Allow")]
        public RegistryView[] Registries { get; set; }
    }

    public class RegistryListView
    {
        public SelectList SemesterSelectList { get; set; }
        public Guid SemesterSelected { get; set; }
        public SelectList ClassSelectList { get; set; }
        public Guid ClassSelected { get; set; }
    }
}