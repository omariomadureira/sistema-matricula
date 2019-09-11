﻿using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemaMatricula.Models;
using SistemaMatricula.Helpers;

namespace SistemaMatricula.Controllers
{
    public class UserController : Controller
    {
        #region Auxiliary properties generated by the system
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
        public ActionResult Index(User view)
        {
            ModelState.Clear();

            try
            {
                if (view.Pagination == null)
                    view.Pagination = new Pagination();

                ViewBag.List = Models.User.List(view);

                if (ViewBag.List == null)
                    throw new Exception("Os usuários não foram listados");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.UserController.Index", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
            }

            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(RegisterViewModel view, bool error = false)
        {
            try
            {
                ViewBag.HideScreen = false;
                ViewBag.Title = "Manutenção de Usuários";

                if (User.IsInRole(Models.User.ROLE_ADMINISTRATOR) == false)
                {
                    view.Login = Models.User.Logged.Login;
                    ViewBag.Title = "Alteração de senha";
                }

                view.RoleSelectList = new SelectList(Models.User.Roles());

                if (error)
                {
                    ViewBag.Message = "Não foi possível salvar o registro. Analise os erros.";
                    return View("Edit", view);
                }

                ModelState.Clear();

                if (string.IsNullOrEmpty(view.Id))
                    return View("Edit", view);

                if (Equals(view.Id, System.Guid.Empty))
                    return View("Edit", view);

                ApplicationUser item = UserManager.FindById(view.Id);

                if (item == null)
                    throw new Exception("Usuário não encontrado");

                view.Login = item.UserName;
                view.Email = item.Email;

                var itens = UserManager.GetRoles(view.Id);

                if (itens.Count < 1)
                    throw new Exception("Usuário não possui role cadastrado");

                view.RoleSelectList = new SelectList(Models.User.Roles(), itens[0]);
                view.RoleSelected = itens[0];
            }
            catch (Exception e)
            {
                object[] parameters = { view, error };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.UserController.Edit", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit", view);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(RegisterViewModel view)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Edit(view, true);

                view.Email = view.Email.Trim();
                view.Login = view.Login.Trim();

                if (User.IsInRole(Models.User.ROLE_ADMINISTRATOR) == false)
                    view.Login = User.Identity.GetUserName();

                var user = new ApplicationUser { UserName = view.Login, Email = view.Email };

                if (string.IsNullOrWhiteSpace(view.Id))
                {
                    var insert = await UserManager.CreateAsync(user, view.Password);

                    if (insert.Succeeded == false)
                    {
                        AddErrors(insert);
                        return Edit(view, true);
                    }

                    user = await UserManager.FindByNameAsync(user.UserName);
                    UserManager.AddToRole(user.Id, view.RoleSelected);

                    return RedirectToAction("Index", "User");
                }

                user = UserManager.FindByName(view.Login);

                if (user == null)
                    throw new Exception("Usuário não encontrado");

                user.PasswordHash = UserManager.PasswordHasher.HashPassword(view.Password);

                var update = await UserManager.UpdateAsync(user);

                if (update.Succeeded == false)
                {
                    AddErrors(update);
                    return Edit(view, true);
                }

                return RedirectToAction("Index", "User");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(view, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.UserController.Update", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Edit");
        }

        [HttpGet]
        [Authorize(Roles = Models.User.ROLE_ADMINISTRATOR)]
        public ActionResult Delete(Guid id)
        {
            try
            {
                if (Guid.Equals(id, Guid.Empty))
                    throw new Exception("Parâmetro vazio");

                var deleted = Models.User.Delete(id);

                if (deleted == false)
                    throw new Exception("Usuário não deletado");

                return RedirectToAction("Index", "User");
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(id, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.UserController.Delete", notes);
                ViewBag.Message = "Não foi possível realizar a solicitação. Erro de execução.";
                ViewBag.HideScreen = true;
            }

            return View("Index");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;

            return View("Login", "_Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel view, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return View("Login", "_Login", view);

                User user = Models.User.Find(login: view.User);

                if (user == null)
                {
                    ModelState.AddModelError("", "Tentativa de login inválida.");
                    return View("Login", "_Login", view);
                }

                if (user.DeleteDate != null)
                {
                    ModelState.AddModelError("", "Usuário inativo");
                    return View("Login", "_Login", view);
                }

                var login = await SignInManager
                    .PasswordSignInAsync(view.User, view.Password, view.RememberMe, shouldLockout: false);

                if (login == SignInStatus.Success)
                {
                    HttpContext.Cache["IdUser"] = user.IdUser;

                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError("", "Tentativa de login inválida.");
            }
            catch (Exception e)
            {
                object[] parameters = { view, returnUrl };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Controllers.UserController.Login", notes);
            }

            return View("Login", "_Login", view);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.Cache.Remove("IdUser");

            return RedirectToAction("Login", "User");
        }

        #region Auxiliary properties generated by the system
        // Usado para proteção XSRF ao adicionar logons externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}