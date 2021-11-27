using ClosedXML.Excel;
using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.Disciplines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class DisciplinesController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public DisciplinesController(
            AppCtx context,
            UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Disciplines
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.Disciplines
                .Include(d => d.User)
                .Where(w => w.IdUser == user.Id)
                .OrderBy(o => o.Name);
            return View(await appCtx.ToListAsync());
        }

        // GET: Disciplines/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disciplines == null)
            {
                return NotFound();
            }

            return View(disciplines);
        }

        public async Task<FileResult> DownloadPattern(short? id)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.Specialties
                .Include(s => s.FormOfStudy)
                .Include(f => f.Groups)
                .Where(w => w.FormOfStudy.IdUser == user.Id)
                .OrderBy(f => f.FormOfStudy.FormOfEdu)
                .ThenBy(f => f.Code);

            int i = 1;      

            IXLRange rngBorder;    

            using (XLWorkbook workbook = new(XLEventTracking.Disabled))
            {
                foreach (Specialty specialty in appCtx)
                {
                    IXLWorksheet worksheet = workbook.Worksheets
                       .Add($"{specialty.FormOfStudy.FormOfEdu.Substring(0, 3)} {specialty.Code}");


                    worksheet.Cell("A" + i).Value = "Форма обучения";
                    worksheet.Cell("B" + i).Value = specialty.FormOfStudy.FormOfEdu;

                    i++;

                    worksheet.Cell("A" + i).Value = "Код специальности";
                    worksheet.Cell("B" + i).Value = $"'{specialty.Code}";

                    i++;

                    worksheet.Cell("C" + i).Value = "Название";
                    worksheet.Cell("D" + i).Value = specialty.Name;

                    i += 3;

                    IXLWorksheet worksheet = workbook.Worksheets
                       .Add($"{specialty.Groups.FormOfEdu.Substring(0, 3)} {specialty.Code}");

                    worksheet.Cell("A" + i).Value = "Индекс проф модуля";
                    worksheet.Cell("A" + 7).Value = specialty.Dis.Name;
                    worksheet.Cell("B" + i).Value = "Название";
                    worksheet.Cell("B" + 7).Value = discipline.ProfModule;
                    worksheet.Cell("C" + i).Value = "Индекс";
                    worksheet.Cell("C" + 7).Value = discipline.Index;
                    worksheet.Cell("D" + i).Value = "Имя";
                    worksheet.Cell("D" + 7).Value = discipline.Name;
                    worksheet.Cell("E" + i).Value = "Краткое имя";
                    worksheet.Cell("E" + 7).Value = discipline.ShortName;

                    rngBorder = worksheet.Range("A6:E6");      
                    rngBorder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;       

                    worksheet.Columns().AdjustToContents();

                    i = 1;
                }

                using (MemoryStream stream = new())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"disciplines_{DateTime.UtcNow.ToShortDateString()}.xlsx"    
                    };
                }
            }
        }

        // GET: Disciplines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Disciplines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDisciplineViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Disciplines
                .Where(f => f.IdUser == user.Id
                    && f.Name == model.Name
                    && f.IndexProfModule == model.IndexProfModule
                    && f.ProfModule == model.ProfModule
                    && f.Index == model.Index
                    && f.ShortName == model.ShortName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид дисциплины уже существует");
            }

            if (ModelState.IsValid)
            {
                Disciplines disciplines = new()
                {
                    IndexProfModule = model.IndexProfModule,
                    ProfModule = model.ProfModule,
                    Index = model.Index,
                    Name = model.Name,
                    ShortName = model.ShortName,
                    IdUser = user.Id
                };

                _context.Add(disciplines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Disciplines/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines.FindAsync(id);
            if (disciplines == null)
            {
                return NotFound();
            }

            EditDisciplineViewModel model = new()
            {
                Id = disciplines.Id,
                IndexProfModule = disciplines.IndexProfModule,
                ProfModule = disciplines.ProfModule,
                Index = disciplines.Index,
                Name = disciplines.Name,
                ShortName = disciplines.ShortName,
                IdUser = disciplines.IdUser
            };

            return View(model);
        }

        // POST: Disciplines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditDisciplineViewModel model)
        {
            Disciplines disciplines = await _context.Disciplines.FindAsync(id);

            if (id != disciplines.Id)
            {
                return NotFound();
            }



            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Disciplines
                .Where(f => f.IdUser == user.Id
                    && f.Name == model.Name
                    && f.IndexProfModule == model.IndexProfModule
                    && f.ProfModule == model.ProfModule
                    && f.Index == model.Index
                    && f.ShortName == model.ShortName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид дисциплины уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    disciplines.IndexProfModule = model.IndexProfModule;
                    disciplines.ProfModule = model.ProfModule;
                    disciplines.Index = model.Index;
                    disciplines.Name = model.Name;
                    disciplines.ShortName = model.ShortName;
                    _context.Update(disciplines);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplinesExists(disciplines.Id))
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

        // GET: Disciplines/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disciplines == null)
            {
                return NotFound();
            }

            return View(disciplines);
        }

        // POST: Disciplines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var disciplines = await _context.Disciplines.FindAsync(id);
            _context.Disciplines.Remove(disciplines);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisciplinesExists(short id)
        {
            return _context.Disciplines.Any(e => e.Id == id);
        }
    }
}
