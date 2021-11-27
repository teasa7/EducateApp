using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducateApp.Models;
using EducateApp.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EducateApp.ViewModels.FormsOfStudy;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class FormsOfStudyController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public FormsOfStudyController(
            AppCtx context,
            UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: FormsOfStudy
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.FormsOfStudy
                .Include(f => f.User)               
                .Where(f => f.IdUser == user.Id)    
                .OrderBy(f => f.FormOfEdu);         

            return View(await appCtx.ToListAsync());
        }

        // GET: FormsOfStudy/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFormOfStudyViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.FormsOfStudy
                .Where(f => f.IdUser == user.Id &&
                    f.FormOfEdu == model.FormOfEdu).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная форма обучения уже существует");
            }

            if (ModelState.IsValid)
            {
                FormOfStudy formOfStudy = new()
                {
                    FormOfEdu = model.FormOfEdu,
                    IdUser = user.Id
                };

                _context.Add(formOfStudy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: FormsOfStudy/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy.FindAsync(id);
            if (formOfStudy == null)
            {
                return NotFound();
            }
            EditFormOfStudyViewModel model = new()
            {
                Id = formOfStudy.Id,
                FormOfEdu = formOfStudy.FormOfEdu,
                IdUser = formOfStudy.IdUser
            };

            return View(model);
        }

        // POST: Specialties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditFormOfStudyViewModel model)
        {
            FormOfStudy formOfStudy = await _context.FormsOfStudy.FindAsync(id);

            if (id != formOfStudy.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.FormsOfStudy
                .Where(f => f.IdUser == user.Id &&
                    f.FormOfEdu == model.FormOfEdu).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид дисциплины уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    formOfStudy.Id = model.Id;
                    formOfStudy.FormOfEdu = model.FormOfEdu;
                    formOfStudy.IdUser = model.IdUser;
                    _context.Update(formOfStudy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormOfStudyExists(formOfStudy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        

        // GET: FormsOfStudy/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formOfStudy == null)
            {
                return NotFound();
            }

            return View(formOfStudy);
        }

        // POST: FormsOfStudy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var formOfStudy = await _context.FormsOfStudy.FindAsync(id);
            _context.FormsOfStudy.Remove(formOfStudy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FormsOfStudy/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formOfStudy == null)
            {
                return NotFound();
            }

            return View(formOfStudy);
        }

        private bool FormOfStudyExists(short id)
        {
            return _context.FormsOfStudy.Any(e => e.Id == id);
        }
    }
}
