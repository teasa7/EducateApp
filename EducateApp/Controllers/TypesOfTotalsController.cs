﻿using System;
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
using EducateApp.ViewModels.TypesOfTotals;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class TypesOfTotalsController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public TypesOfTotalsController(
             AppCtx context,
             UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: TypesOfTotals
        public async Task<IActionResult> Index(TypeOfTotalSortState sortOrder = TypeOfTotalSortState.CertificateNameAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var typesOfTotals = _context.TypesOfTotals
                .Include(i => i.User)
                .Where(w => w.IdUser == user.Id);     

            ViewData["CertificateNameSort"] = sortOrder == TypeOfTotalSortState.CertificateNameAsc ? TypeOfTotalSortState.CertificateNameDesc : TypeOfTotalSortState.CertificateNameAsc;

            typesOfTotals = sortOrder switch
            {
                TypeOfTotalSortState.CertificateNameDesc => typesOfTotals.OrderByDescending(s => s.CertificateName),
                _ => typesOfTotals.OrderBy(s => s.CertificateName),
            };

            return View(await typesOfTotals.AsNoTracking().ToListAsync());
        }

        // GET: TypesOfTotals/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfTotal = await _context.TypesOfTotals
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfTotal == null)
            {
                return NotFound();
            }

            return PartialView(typeOfTotal);
        }

        // GET: TypesOfTotals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypesOfTotals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTypeOfTotalViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.TypesOfTotals
                .Where(f => f.IdUser == user.Id &&
                    f.CertificateName == model.CertificateName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид промежуточной аттестации уже существует");
            }

            if (ModelState.IsValid)
            {
                TypeOfTotal typeOfTotal = new()
                {
                    CertificateName = model.CertificateName,
                    IdUser = user.Id
                };

                _context.Add(typeOfTotal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: TypesOfTotals/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfTotal = await _context.TypesOfTotals.FindAsync(id);
            if (typeOfTotal == null)
            {
                return NotFound();
            }

            EditTypeOfTotalViewModel model = new()
            {
                Id = typeOfTotal.Id,
                CertificateName = typeOfTotal.CertificateName,
                IdUser = typeOfTotal.IdUser
            };

            return View(model);
        }

        // POST: TypesOfTotals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditTypeOfTotalViewModel model)
        {
            TypeOfTotal typeOfTotal = await _context.TypesOfTotals.FindAsync(id);

            if (id != typeOfTotal.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.TypesOfTotals
               .Where(f => f.IdUser == user.Id &&
                   f.CertificateName == model.CertificateName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид промежуточной аттестации уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    typeOfTotal.CertificateName = model.CertificateName;
                    _context.Update(typeOfTotal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeOfTotalExists(typeOfTotal.Id))
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

        // GET: TypesOfTotals/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfTotal = await _context.TypesOfTotals
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfTotal == null)
            {
                return NotFound();
            }

            return View(typeOfTotal);
        }

        // POST: TypesOfTotals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var typeOfTotal = await _context.TypesOfTotals.FindAsync(id);
            _context.TypesOfTotals.Remove(typeOfTotal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeOfTotalExists(short id)
        {
            return _context.TypesOfTotals.Any(e => e.Id == id);
        }
    }
}
