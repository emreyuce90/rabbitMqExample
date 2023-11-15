using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rabbitMqExample.WebExcelCreate.Models;

namespace rabbitMqExample.WebExcelCreate.Controllers {
    public class UserController : Controller {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager) {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(UserAddDto userAddDto) {
           var result= await _userManager.CreateAsync(new IdentityUser {
                UserName = "emreyuce",
                Email = "emreyuce90@hotmail.com"
            },"Emre123*");
            await _userManager.CreateAsync(new IdentityUser {
                UserName = "zeynepyuce",
                Email = "zeynepyuce90@hotmail.com"
            }, "Zeynep123*");
            
            return View();
        }
    }
}
