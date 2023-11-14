using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rabbitMqExample.Watermark.Web.Models;
using rabbitMqExample.Watermark.Web.Services;

namespace rabbitMqExample.Watermark.Web.Controllers {
    public class ProductsController : Controller {
        private readonly AppDbContext _context;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        public ProductsController(AppDbContext context, RabbitMQPublisher rabbitMQPublisher) {
            _context = context;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        // GET: Products
        public async Task<IActionResult> Index() {
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Products'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile pictureFile) {
            if (!ModelState.IsValid) return View(product);

            if (pictureFile is { Length: > 0 }) {
                //yeni isim belirleme

                var newName = Guid.NewGuid().ToString() + Path.GetExtension(pictureFile.FileName); //sdfksdjbkfbsdkfjsdbf.jpg
                //C:dddcdcd/wwwroot/images
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images",newName);

                await using FileStream fs = new(path, FileMode.Create);
                await pictureFile.CopyToAsync(fs);
                product.PictureUrl = newName;

                //RabbitMQ Publish
                _rabbitMQPublisher.Publish(new productImageCreatedEvent { PictureUrl = newName });
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock,PictureUrl")] Product product) {
            if (id != product.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!ProductExists(product.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.Products == null) {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Products == null) {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null) {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id) {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
