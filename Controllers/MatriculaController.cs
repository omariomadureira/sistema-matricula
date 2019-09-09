using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ALUNO)]
    public class MatriculaController : Controller
    {
        public ActionResult Index(MatriculaIndexView view)
        {
            try
            {
                Student logado = Student.ConsultarLogado();

                if (logado == null)
                {
                    ViewBag.Message = "Não foi possível listar as matrículas. Erro de execução.";
                    return View();
                }

                Registry filtros = new Registry()
                {
                    Aluno = logado
                };

                var lista = Registry.Listar(filtros);

                if (lista == null)
                {
                    ViewBag.Message = "Não foi possível listar as matrículas. Erro de execução.";
                    return View(view);
                }

                ViewBag.Matriculas = lista.FindAll(x => x.DisciplinaSemestre.Semestre.Ativo);
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        public ActionResult Edit(MatriculaEditView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                var cursos = Course.Listar();

                if (cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View();
                }

                if (view.CursoSelecionado == null)
                {
                    view.CursoSelecionado = cursos[0].IdCurso;
                }

                view.slCursos = new SelectList(cursos, "IdCurso", "Nome", view.CursoSelecionado);

                Student logado = Student.ConsultarLogado();

                var lista = Registry.ListarGrade(view.CursoSelecionado.Value, logado);

                if (lista != null && lista.Count > 0)
                {
                    view.Lista = MatriculaEditView.Converter(lista.ToArray());
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
        public ActionResult Update(MatriculaEditView view)
        {
            try
            {
                ViewBag.HideScreen = false;

                if (ModelState.IsValid)
                {
                    foreach (MatriculaView item in view.Lista)
                    {
                        if (item.PrimeiraOpcao || item.SegundaOpcao)
                        {
                            item.Alternativa = item.SegundaOpcao;
                            item.Aluno = Student.ConsultarLogado();

                            Registry existe = Registry.Consultar(item);

                            if (existe == null)
                            {
                                if (Registry.Incluir(item))
                                {
                                    continue;
                                }
                                else
                                {
                                    ViewBag.Message = "Não foi possível realizar a matrícula. Erro de execução.";
                                    ViewBag.HideScreen = true;
                                    return View("Edit");
                                }
                            }
                            else
                            {
                                if (Registry.Alterar(item))
                                {
                                    continue;
                                }
                                else
                                {
                                    ViewBag.Message = "Não foi possível realizar a matrícula. Erro de execução.";
                                    ViewBag.HideScreen = true;
                                    return View("Edit");
                                }
                            }
                        }
                    }

                    return RedirectToAction("Index", "Matricula");
                }
                else
                {
                    ViewBag.Message = "Não foi possível realizar as matrículas.";
                }

                var cursos = Course.Listar();

                if (cursos == null)
                {
                    ViewBag.Message = "Não foi possível listar os cursos. Erro de execução.";
                    ViewBag.HideScreen = true;
                    return View("Edit", view);
                }

                if (view.CursoSelecionado == null)
                {
                    view.CursoSelecionado = cursos[0].IdCurso;
                }

                view.slCursos = new SelectList(cursos, "IdCurso", "Nome", view.CursoSelecionado);

                Student logado = Student.ConsultarLogado();

                var lista = Registry.ListarGrade(view.CursoSelecionado.Value, logado);

                if (lista != null && lista.Count > 0)
                {
                    view.Lista = MatriculaEditView.Converter(lista.ToArray());
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
                    Registry filtros = new Registry()
                    {
                        Aluno = Student.ConsultarLogado(),
                        DisciplinaSemestre = view
                    };

                    view = Registry.Consultar(filtros).DisciplinaSemestre;

                    if (view == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        return View(view);
                    }

                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Registry.Desativar(filtros))
                        {
                            return RedirectToAction("Index", "Matricula");
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

        [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
        public ActionResult Finish()
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                ViewBag.Grades = Grid.ListarGrade(Grid.DISCIPLINA_LIBERADA);

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
        [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
        public ActionResult UpdateState()
        {
            ViewBag.HideScreen = false;

            try
            {
                if (Grid.AlterarStatus(Grid.DISCIPLINA_LIBERADA))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Não foi possível realizar a liberação das disciplinas para matrícula. Erro de execução.";
                    ViewBag.HideScreen = true;
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Start");
        }
    }

    public class MatriculaIndexView
    {
        public SelectList slSemestres { get; set; }
        public System.Guid? SemestreSelecionado { get; set; }
    }

    public class MatriculaEditView
    {
        public SelectList slCursos { get; set; }
        public System.Guid? CursoSelecionado { get; set; }
        [CustomValidation(typeof(MatriculaEditView), "PermitirMatricula")]
        public MatriculaView[] Lista { get; set; }

        public static MatriculaView[] Converter(Registry[] lista)
        {
            try
            {
                MatriculaView[] novaLista = new MatriculaView[lista.Length];

                for (int i = 0; i < lista.Length; i++)
                {
                    MatriculaView item = MatriculaView.Converter(lista[i]);
                    novaLista.SetValue(item, i);
                }

                return novaLista;
            }
            catch
            {
                return null;
            }
        }

        public static ValidationResult PermitirMatricula(MatriculaView[] lista)
        {
            if (lista == null)
            {
                return new ValidationResult("Tente novamente mais tarde.");
            }

            int primeira = 0, segunda = 0;

            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i].PrimeiraOpcao)
                {
                    primeira++;
                }
                else if (lista[i].SegundaOpcao)
                {
                    segunda++;
                }
            }

            Registry filtros = new Registry()
            {
                Aluno = Student.ConsultarLogado()
            };

            var matriculas =
                Registry.Listar(filtros)
                .FindAll(x => x.DisciplinaSemestre.Semestre.Ativo);

            int restantePrimeira = 4 - matriculas.FindAll(x => !x.Alternativa.HasValue || !x.Alternativa.Value).Count;
            int restanteSegunda = 2 - matriculas.FindAll(x => x.Alternativa.HasValue && x.Alternativa.Value).Count;

            if (restantePrimeira < 1 && restanteSegunda < 1)
            {
                return new ValidationResult("A quantidade máxima de matrículas foi atingida.");
            }

            if (restantePrimeira > 0 && primeira == 0)
            {
                return new ValidationResult("Escolha pelo menos 1 disciplina para primeira opção.");
            }

            if (restantePrimeira > 0 && primeira > restantePrimeira)
            {
                return new ValidationResult(string.Format("Escolha no máximo {0} disciplina(s) para primeira opção.", restantePrimeira));
            }

            if (restantePrimeira == 0 && primeira > restantePrimeira)
            {
                return new ValidationResult("A quantidade máxima de matrículas para primeira opção foi atingida.");
            }

            if (restanteSegunda > 0 && segunda == 0)
            {
                return new ValidationResult("Escolha pelo menos 1 disciplina para segunda opção.");
            }

            if (restanteSegunda > 0 && segunda > restanteSegunda)
            {
                return new ValidationResult(string.Format("Escolha no máximo {0} disciplina(s) para segunda opção.", restanteSegunda));
            }

            return ValidationResult.Success;
        }
    }

    public class MatriculaView : Registry
    {
        public bool PrimeiraOpcao { get; set; }
        public bool SegundaOpcao { get; set; }

        public static MatriculaView Converter(Registry a)
        {
            try
            {
                return new MatriculaView
                {
                    DisciplinaSemestre = a.DisciplinaSemestre,
                    Aluno = a.Aluno,
                    Alternativa = a.Alternativa,
                    SegundaOpcao = a.Alternativa != null ? a.Alternativa.Value : false,
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