using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebShop.Models;
using WebShop.Models.DAL;
namespace WebShop.Controllers
{
    [Authorize]
    public class AccountController : UserController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
   
       
        public AccountController(IUnitOfWork unitIn):base(unitIn)
        {
            
        }
        public AccountController()
        {
            
        }
       [Authorize(Roles = "Manager")]
        public ActionResult AdminView()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [Authorize(Roles = "Manager")]
        public async Task<JsonResult> RemoveUser(string Id)
        {
           
            Customer customer = unit.GetCustomers.Get()
                .Where(u => u.ApplicationUserId == Id).FirstOrDefault();
            ApplicationUser user = UserManager.Users.Where(u => u.Id == Id).FirstOrDefault();
            if (user == null)
                return Json(new { success = false, Msg = "Данного пользователя не существует в системе!" });
            if (customer == null && !UserManager.IsInRole(Id, "Manager"))
            {
                return Json(new { success = false, Msg = "Данный пользователь не менеджер и не является заказчиком!" });
            }
            else if (customer != null)
            {
                try{
                unit.GetOrders.Get().Where(c => c.CustomerId == customer.Id)
                    .ToList().ForEach(
                    o=>Delete(o,unit)
                    );
                }
                catch(Exception ex)
                {
                    return Json(new { success = false, Msg = ex.Message });
                }
            }
            var roles = await UserManager.GetRolesAsync(Id);
            var logins = await UserManager.GetLoginsAsync(Id);
            IdentityResult IsSuccess = null;
            foreach (var login in logins)
            {
                IsSuccess = await UserManager.RemoveLoginAsync(Id, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                if (!IsSuccess.Succeeded)
                    return Json(new { success = false, Msg = String.Join(";", IsSuccess.Errors) });
            }
            foreach (var role in roles)
            {
                IsSuccess = await UserManager.RemoveFromRoleAsync(Id, role);
                if (!IsSuccess.Succeeded)
                    return Json(new { success = false, Msg = String.Join(";", IsSuccess.Errors) });
            }
            if (customer != null)
            {
                customer.User = null;
                customer.ApplicationUserId = Guid.Empty.ToString();
                unit.GetCustomers.DeleteById(customer.Id);
                unit.Save();
            }
           
            
            user.Customer = null;
           
            var RemoveUser = context.Users.Where(u=>u.Id == Id).FirstOrDefault();
            context.Users.Remove(RemoveUser);
            context.SaveChanges();
            if (!IsSuccess.Succeeded)
                return Json(new { success = false, Msg = String.Join(";", IsSuccess.Errors) });
            return Json(new { success = true });
        }
       /* public async Task<JsonResult> UpdateRole(string Role, string Id)
        {
            ApplicationUser user = UserManager.Users
                 .Where(u => u.Id == Id).FirstOrDefault();
            if (user != null)
            {
                user.Roles.Clear();
                IdentityResult IsValid = await UserManager.AddToRoleAsync(Id, Role);
                if (IsValid.Succeeded)
                    return Json(new { success = true });
                return Json(new { success = false, Msg = String.Join(";",IsValid.Errors) });
            }
            else
                return Json(new { success = false, Msg = "Пользователь не найден!" });
            
        }*/
        [Authorize(Roles = "Manager")]
        public async Task<JsonResult> UpdatePwd(string Pwd, string Id)
        {
            ApplicationUser user = UserManager.Users
                .Where(u => u.Id == Id).FirstOrDefault();
            if (user != null)
            {
                IdentityResult IsValid = await UserManager.PasswordValidator.ValidateAsync(Pwd);
                if (IsValid.Succeeded)
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(Pwd);
                    IsValid = await UserManager.UpdateAsync(user);
                    if (IsValid.Succeeded)
                        return Json(new { success = true });
                }
                return Json(new { success = false, Msg = String.Join(";",IsValid.Errors) });


            }
            else
                return Json(new { success = false, Msg = "Пользователь не найден!" });
            
          
        }
        [Authorize(Roles = "Manager")]
        public  async Task<JsonResult> UpdateLogin( string Login, string Id)
        {
           ApplicationUser user =  UserManager.Users
               .Where(u => u.Id == Id).FirstOrDefault();
         

           if (user != null)
           {
               user.UserName = Login;
               user.Email = Login;
               if (UserManager.IsInRole(user.Id, "user"))
               {
                   var customer = unit.GetCustomers.Get()
                       .Where(c => c.ApplicationUserId == user.Id).First();
                   customer.Name = Login;
                   unit.GetCustomers.Update(customer);
               }
               
           }
           else
           {
               return Json(new { success = false, Msg = "Пользователь не найден!" });
           }
           IdentityResult IsValid   =  await UserManager.UserValidator.ValidateAsync(user);
           if (IsValid.Succeeded)
           {
               IsValid = await UserManager.UpdateAsync(user);

               if (IsValid.Succeeded)
               {
                   unit.Save();
                   return Json(new { success = true });
               }
           }

           return Json(new { success = false, Msg = String.Join(";",IsValid.Errors) });
            
        }
        [Authorize(Roles = "Manager")]
        public JsonResult GetUsers()
        {
           
            var Customers = unit.GetCustomers.Get();
            var Users = UserManager.Users.ToArray()
                .Select(s =>new
                        {
                            Id = s.Id,
                            Login = s.UserName,
                            Role = UserManager.IsInRole(s.Id, "user")?"user":"Manager",
                            Code =UserManager.IsInRole(s.Id,"user") ? 
                                Customers.Where(c => c.ApplicationUserId == s.Id).First().Code : "",
                            Discount = UserManager.IsInRole(s.Id, "user") ? 
                                Customers.Where(c => c.ApplicationUserId == s.Id).First().Discount: 0

                        
                    }).ToArray();
            return Json(new { Data = Users }, JsonRequestBehavior.AllowGet);
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //  [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result =  SignInManager.PasswordSignIn(model.Email, model.Password, false, false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = UserManager.Users.Where(u=>u.UserName ==model.Email ).First();
                    if (UserManager.IsInRole(user.Id,"Manager"))
                        return Json(new { Url = "/Item/Index", success = true });
                    else if (UserManager.IsInRole(user.Id,"user"))
                        return Json(new { Url = "/Item/GetWithoutOrderView", success = true });
                    return Json(new { Url = returnUrl, success = false });
                //  case SignInStatus.LockedOut:
                //    return View("Lockout");
                //   case SignInStatus.RequiresVerification:
                //     return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:


                default:
                    return Json(new { Url = returnUrl, success = false });
            }
            return Json(new { Url = returnUrl, success = false });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public JsonResult CreateUser(string Login, string Pwd, string Role)
        {
            var user = new ApplicationUser { UserName = Login, Email = Login };
            var result = UserManager.Create(user, Pwd);
            if (result.Succeeded)
            {
                result = UserManager.AddToRole(user.Id, Role);
                UserManager.Update(user);
            }
            else
                return Json(new { success = false, Msg = String.Join(";", result.Errors) });
            if (Role == "user")
            {
                try
                {
                    CreateCustomer(user.UserName,unit);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Msg = ex.Message });
                }
            }
           
            return Json(new {  success = true });
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //   [ValidateAntiForgeryToken]
        public JsonResult Register(RegisterViewModel model)
        {
          
            
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = UserManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "user");
                    SignInManager.SignIn(user, isPersistent: true, rememberBrowser: false);
                    CreateCustomer(user.UserName, unit);
                   
                    return Json(new { Url = "/Item/GetWithoutOrderView", success = true });// RedirectToAction("GetWithoutOrderView", "Item"); 
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return Json(new { Url = String.Empty, success = false });
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
      
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
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