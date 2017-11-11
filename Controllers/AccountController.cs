using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mvcIpsa.Models;
using mvcIpsa.Models.AccountViewModels;
using mvcIpsa.Services;

namespace mvcIpsa.Controllers
{
    using mvcIpsa.Extensions;
    using mvcIpsa.DbModel;
   
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IPSAContext db;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            IPSAContext _db,

            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {       
            _emailSender = emailSender;
            _logger = logger;
            db = _db;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync();

            ViewData["ReturnUrl"] = returnUrl;
            return View("Login2");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                AppUser svcUser = null;
                var unEncrytp = UrlHelperExtensions.getPasswordHashed(model.Password);
                var result = db.Profile
                    .Where(p => p.Username == model.username && p.Password == unEncrytp)
                    .FirstOrDefault();

                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario y/o la contraseña es incorrecto.");
                    return View("Login2",model);
                }

                var roles = db.Profilerole.Where(r => r.Username == model.username).Select(r=>r.Idrole).ToArray();
                var caja = db.Caja.Find(result.Idcaja);
                svcUser = new AppUser()
                {
                    username = result.Username,
                    ncentrocosto = result.Ncentrocosto,
                    Token = model.Password,
                    roles = roles,
                    idcaja = result.Idcaja.Value,
                    description = caja.Description
                };

                await HttpContext.SignInAsync(svcUser.CreatePrincipal());

                return RedirectToLocal(returnUrl);               
            }            
            return View("Login2",model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Lockout()
        {
            await HttpContext.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(AccountController.Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Info()
        {
            var user = this.GetServiceUser();
            var Caja = db.Caja.Find(user.idcaja);

            ViewBag.Caja = Caja.Description;
            return PartialView("_Info");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

      
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
