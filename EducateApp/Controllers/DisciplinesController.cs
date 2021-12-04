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
        public async Task<IActionResult> Index(string IndexProfModule, string ProfModule, string Index, string name, string ShortName,
            int page = 1,
           DisciplineSortState sortOrder = DisciplineSortState.IndexProfModuleAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            int pageSize = 5;

            IQueryable<Disciplines> disciplines = _context.Disciplines
                .Include(d => d.User)
                .Where(w => w.IdUser == user.Id);

            if (!String.IsNullOrEmpty(IndexProfModule))
            {
                disciplines = disciplines.Where(p => p.IndexProfModule.Contains(IndexProfModule));
            }
            if (!String.IsNullOrEmpty(ProfModule))
            {
                disciplines = disciplines.Where(p => p.ProfModule.Contains(ProfModule));
            }
            if (!String.IsNullOrEmpty(Index))
            {
                disciplines = disciplines.Where(p => p.Index.Contains(Index));
            }
            if (!String.IsNullOrEmpty(name))
            {
                disciplines = disciplines.Where(p => p.Name.Contains(name));
            }
            if (!String.IsNullOrEmpty(ShortName))
            {
                disciplines = disciplines.Where(p => p.ShortName.Contains(ShortName));
            }

            switch (sortOrder)
            {
                case DisciplineSortState.IndexProfModuleDesc:
                    disciplines = disciplines.OrderByDescending(s => s.IndexProfModule);
                    break;
                case DisciplineSortState.ProfModuleAsc:
                    disciplines = disciplines.OrderBy(s => s.ProfModule);
                    break;
                case DisciplineSortState.ProfModuleDesc:
                    disciplines = disciplines.OrderByDescending(s => s.ProfModule);
                    break;
                case DisciplineSortState.IndexAsc:
                    disciplines = disciplines.OrderBy(s => s.Index);
                    break;
                case DisciplineSortState.IndexDesc:
                    disciplines = disciplines.OrderByDescending(s => s.Index);
                    break;
                case DisciplineSortState.NameAsc:
                    disciplines = disciplines.OrderBy(s => s.Name);
                    break;
                case DisciplineSortState.NameDesc:
                    disciplines = disciplines.OrderByDescending(s => s.Name);
                    break;
                case DisciplineSortState.ShortNameAsc:
                    disciplines = disciplines.OrderBy(s => s.ShortName);
                    break;
                case DisciplineSortState.ShortNameDesc:
                    disciplines = disciplines.OrderByDescending(s => s.ShortName);
                    break;
                case DisciplineSortState.IndexProfModuleAsc:
                    break;
                default:
                    disciplines = disciplines.OrderBy(s => s.IndexProfModule);
                    break;
            }

            var count = await disciplines.CountAsync();
            var items = await disciplines.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            IndexDisciplineViewModel viewModel = new()
            {
                PageViewModel = new(count, page, pageSize),
                SortDisciplineViewModel = new(sortOrder),
                FilterDisciplineViewModel = new(IndexProfModule, ProfModule, Index, name, ShortName),
                Disciplines = items
            };

            return View(viewModel);

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

            return PartialView(disciplines);
        }

        public async Task<FileResult> DownloadPattern()
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

                    worksheet.Cell("A" + i).Value = "Название";
                    worksheet.Cell("B" + i).Value = specialty.Name;

                    i += 3;

                    worksheet.Cell("A" + i).Value = "Индекс проф модуля";
                    worksheet.Cell("B" + i).Value = "Название";
                    worksheet.Cell("C" + i).Value = "Индекс";
                    worksheet.Cell("D" + i).Value = "Имя";
                    worksheet.Cell("E" + i).Value = "Краткое имя";

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
