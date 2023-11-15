using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace rabbitMqExample.WebExcelCreate.Controllers {
    public class AuthController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email,string password) {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return View();
            var result =await _signInManager.PasswordSignInAsync(user, password,true,false);
            if (!result.Succeeded) return View();

            return RedirectToAction(controllerName:"Home",actionName:"Index");
        }
    }
}
