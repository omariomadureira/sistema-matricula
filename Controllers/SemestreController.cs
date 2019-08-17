using SistemaMatricula.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class SemestreController : Controller
    {
        public ActionResult Index(SemestreView view)
        {
            ModelState.Clear();

            try
            {
                view.slPeriodos = new SelectList(Semestre.Periodos());

                if (view.PeriodoSelecionado != null)
                    view.Periodo = view.PeriodoSelecionado;

                ViewBag.Semestres = Semestre.Listar(view.Periodo);

                if (ViewBag.Semestres == null)
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
        public ActionResult Edit(SemestreView view)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                view.slPeriodos = new SelectList(Semestre.Periodos());

                if (!Equals(view.IdSemestre, System.Guid.Empty))
                {
                    var semestre = Semestre.Consultar(view.IdSemestre);

                    if (semestre == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                        return View();
                    }

                    view = SemestreView.Converter(semestre);
                    view.slPeriodos = new SelectList(Semestre.Periodos(), semestre.Periodo.Trim());
                    view.PeriodoSelecionado = semestre.Periodo.Trim();
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
        public ActionResult Update(SemestreView view)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    view.Periodo = view.PeriodoSelecionado.Trim();

                    if (!Equals(view.IdSemestre, System.Guid.Empty))
                    {
                        if (Semestre.Alterar(view))
                        {
                            return RedirectToAction("Index", "Semestre");
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
                        if (Semestre.Incluir(view))
                        {
                            return RedirectToAction("Index", "Semestre");
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

                view.slPeriodos = new SelectList(Semestre.Periodos());
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        public ActionResult Delete(SemestreView view, bool? Delete)
        {
            try
            {
                if (!Equals(view.IdSemestre, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Semestre.Desativar(view.IdSemestre))
                        {
                            return RedirectToAction("Index", "Semestre");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    var semestre = Semestre.Consultar(view.IdSemestre);

                    if (semestre == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                    }

                    view = SemestreView.Converter(semestre);
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

    public class SemestreView : Semestre
    {
        public SelectList slPeriodos { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string PeriodoSelecionado { get; set; }

        public static SemestreView Converter(Semestre item)
        {
            try
            {
                return new SemestreView()
                {
                    IdSemestre = item.IdSemestre,
                    InicioData = item.InicioData,
                    Periodo = item.Periodo,
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