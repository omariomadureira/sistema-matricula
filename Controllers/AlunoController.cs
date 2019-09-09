﻿using SistemaMatricula.Models;
using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize(Roles = Usuario.ROLE_ADMINISTRADOR)]
    public class AlunoController : Controller
    {
        public ActionResult Index(Student item)
        {
            ModelState.Clear();

            try
            {
                ViewBag.Alunos = Student.Listar(item.Nome);

                if (ViewBag.Alunos == null)
                {
                    ViewBag.Message = "Não foi possível listar os registros. Erro de execução.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(item);
        }

        [HttpGet]
        public ActionResult Edit(Student item)
        {
            ViewBag.HideScreen = false;
            ModelState.Clear();

            try
            {
                if (!Equals(item.IdAluno, System.Guid.Empty))
                {
                    item = Student.Consultar(item.IdAluno);

                    if (item == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
                        ViewBag.HideScreen = true;
                    }

                    return View(item);
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
        public ActionResult Update(Student item)
        {
            ViewBag.HideScreen = false;

            try
            {
                if (ModelState.IsValid)
                {
                    item.CPF = item.CPF.Trim();
                    item.Email = item.Email.Trim();
                    item.Nome = item.Nome.Trim();

                    if (!Equals(item.IdAluno, System.Guid.Empty))
                    {
                        if (Student.Alterar(item))
                        {
                            return RedirectToAction("Index", "Aluno");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível atualizar o registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                        }
                    }
                    else
                    {
                        if (Student.Incluir(item))
                        {
                            return RedirectToAction("Index", "Aluno");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível incluir um novo registro. Erro de execução.";
                            ViewBag.HideScreen = true;
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Não foi possível atualizar o registro. Revise o preenchimento dos campos.";
                }
            }
            catch
            {
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", item);
        }

        public ActionResult Delete(Student item, bool? Delete)
        {
            try
            {
                if (!Equals(item.IdAluno, System.Guid.Empty))
                {
                    if (Delete.HasValue && Delete.Value)
                    {
                        if (Student.Desativar(item.IdAluno))
                        {
                            return RedirectToAction("Index", "Aluno");
                        }
                        else
                        {
                            ViewBag.Message = "Não foi possível apagar o registro. Erro de execução.";
                            return View();
                        }
                    }

                    item = Student.Consultar(item.IdAluno);

                    if (item == null)
                    {
                        ViewBag.Message = "Não foi possível localizar o registro. Identificação inválida.";
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

            return View(item);
        }
    }
}