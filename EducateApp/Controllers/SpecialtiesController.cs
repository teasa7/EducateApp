using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.Specialties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class SpecialtiesController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public SpecialtiesController(AppCtx context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.Specialties
                .Include(s => s.FormOfStudy)                 
                .Where(w => w.FormOfStudy.IdUser == user.Id)  
                .OrderBy(f => f.Code);                     
            return View(await appCtx.ToListAsync());           
        }

        // GET: Specialties/Create
        public async Task<IActionResult> CreateAsync()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            ViewData["IdFormOfStudy"] = new SelectList(_context.FormsOfStudy
                .Where(w => w.IdUser == user.Id), "Id", "FormOfEdu");
            return View();
        }

        // POST: Specialties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSpecialtyViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                Specialty specialty = new()
                {
                    Code = model.Code,
                    Name = model.Name,

                    IdFormOfStudy = model.IdFormOfStudy
                };

                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", model.IdFormOfStudy);
            return View(model);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            EditSpecialtyViewModel model = new()
            {
                Id = specialty.Id,
                Code = specialty.Code,
                Name = specialty.Name,
                IdFormOfStudy = specialty.IdFormOfStudy
            };

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", specialty.IdFormOfStudy);
            return View(model);
        }

        // POST: Specialties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditSpecialtyViewModel model)
        {
            Specialty specialty = await _context.Specialties.FindAsync(id);

            if (id != specialty.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    specialty.Code = model.Code;
                    specialty.Name = model.Name;
                    specialty.IdFormOfStudy = model.IdFormOfStudy;
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
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

            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", specialty.IdFormOfStudy);
            return View(model);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        private bool SpecialtyExists(short id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
