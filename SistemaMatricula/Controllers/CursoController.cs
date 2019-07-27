using System.Web.Mvc;
using SistemaMatricula.Models;
using System.Collections.Generic;

namespace SistemaMatricula.Controllers
{
    public class CursoController : Controller
    {
        // GET: Curso
        public ActionResult Index()
        {
            Curso c = new Curso();
            List<Curso> l = new List<Curso>();
            l = c.Listar();

            return View();
        }
       
        public ActionResult Novo()
        {
            return View();
        }
    }
}