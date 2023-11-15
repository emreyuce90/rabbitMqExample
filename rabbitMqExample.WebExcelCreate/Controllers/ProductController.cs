using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using rabbitMqExample.Shared;
using rabbitMqExample.WebExcelCreate.Models;
using rabbitMqExample.WebExcelCreate.Services;

namespace rabbitMqExample.WebExcelCreate.Controllers {
    [Authorize]
    public class ProductController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RabbitMqPublisher _rabbitMqPublisher;
        public ProductController(UserManager<IdentityUser> userManager, AppDbContext context, RabbitMqPublisher rabbitMqPublisher) {
            _userManager = userManager;
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public IActionResult Index() {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateExcel() {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            var file = new UserFile {
                FileName = $"user-file-{Guid.NewGuid().ToString().Substring(1, 10)}",
                FileStatus = FileStatus.Creating,
                UserId = user.Id,
            };
            await _context.UserFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            //send to rabbit Mq
            _rabbitMqPublisher.Publish(new CreateExcelMessage {
                FileId=file.Id,
                UserId =user.Id,
            });
            return RedirectToAction("GetAllFilesByUser");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilesByUser() {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);

            return View( await _context.UserFiles.Where(uf => uf.UserId == user.Id).ToListAsync());
        }
    }
}
