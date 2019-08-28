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
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (view.Lista != null && view.Lista.Length > 0)
                {

                }

                var cursos = Curso.Listar();

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

                var lista = DisciplinaSemestreAluno.ListarGrade(view.CursoSelecionado.Value);

                if (lista != null && lista.Count > 0)
                {
                    view.Lista = MatriculaIndexView.Converter(lista.ToArray());
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View(view);
        }
    }

    public class MatriculaIndexView
    {
        public SelectList slCursos { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public System.Guid? CursoSelecionado { get; set; }
        public MatriculaEditView[] Lista { get; set; }

        public static MatriculaEditView[] Converter(DisciplinaSemestreAluno[] lista)
        {
            try
            {
                MatriculaEditView[] novaLista = new MatriculaEditView[lista.Length];

                for (int i = 0; i < lista.Length; i++)
                {
                    MatriculaEditView item = MatriculaEditView.Converter(lista[i]);
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

    public class MatriculaEditView : DisciplinaSemestreAluno
    {
        public bool PrimeiraOpcao { get; set; }
        public bool SegundaOpcao { get; set; }

        public static MatriculaEditView Converter(DisciplinaSemestreAluno a)
        {
            try
            {
                return new MatriculaEditView
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