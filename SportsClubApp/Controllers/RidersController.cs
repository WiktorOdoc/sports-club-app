
using Microsoft.AspNetCore.Mvc;
using SportsClubApp.Models;
using SportsClubApp.Interfaces;

namespace SportsClubApp.Controllers
{
    public class RidersController : Controller
    {
        private readonly IRiderRepository _repository;

        public RidersController(IRiderRepository repository)
        {
            _repository = repository;
        }

        // GET: RIDERS
        public async Task<IActionResult> Index()
        {
            var riders = await _repository.GetAllAsync();
            return View(riders);
        }

        // GET: RIDERS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _repository.GetByIdAsync(id.Value);

            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // GET: RIDERS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RIDERS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,FirstName,LastName,BirthYear,RacingNumber")] Rider rider)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(rider);
                return RedirectToAction(nameof(Index));
            }

            return View(rider);
        }

        // GET: RIDERS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _repository.GetByIdAsync(id.Value);

            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // POST: RIDERS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int? id,
            [Bind("Id,FirstName,LastName,BirthYear,RacingNumber")] Rider rider)
        {
            if (id != rider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await _repository.ExistsAsync(rider.Id))
                {
                    return NotFound();
                }

                await _repository.UpdateAsync(rider);
                return RedirectToAction(nameof(Index));
            }

            return View(rider);
        }

        // GET: RIDERS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = await _repository.GetByIdAsync(id.Value);

            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // POST: RIDERS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
