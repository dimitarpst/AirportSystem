using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportSystem.Models;
using AirportSystem.Models.Data;

namespace AirportSystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Flights.Include(f => f.ArrivalAirport).Include(f => f.DepartureAirport);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        private void PopulateAirportsDropDown(object selectedArrivalAirport = null, object selectedDepartureAirport = null)
        {
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "Id", "Name", selectedArrivalAirport);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "Id", "Name", selectedDepartureAirport);
        }


        // GET: Flights/Create
        public IActionResult Create()
        {
            PopulateAirportsDropDown();
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightNumber,Airline,DepartureTime,ArrivalTime,DepartureAirportId,ArrivalAirportId")] Flight flight)
        {
            ModelState.Remove("ArrivalAirport");
            ModelState.Remove("DepartureAirport");

            if (flight.DepartureTime != default && flight.ArrivalTime != default)
            {
                if (flight.ArrivalTime <= flight.DepartureTime)
                {
                    ModelState.AddModelError(nameof(flight.ArrivalTime), "Arrival time must be after departure time.");
                }
                else if ((flight.ArrivalTime - flight.DepartureTime).TotalHours > 24)
                {
                    ModelState.AddModelError(nameof(flight.ArrivalTime), $"Flight duration cannot exceed 24 hours. Current: {(flight.ArrivalTime - flight.DepartureTime).TotalHours:F1}h");
                }
            }

            if (flight.DepartureAirportId == flight.ArrivalAirportId)
            {
                ModelState.AddModelError(nameof(flight.ArrivalAirportId), "Departure and Arrival airports must be different.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAirportsDropDown(flight.ArrivalAirportId, flight.DepartureAirportId);
            return View(flight);
        }


        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            PopulateAirportsDropDown(flight.ArrivalAirportId, flight.DepartureAirportId);

            return View(flight);
        }


        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,Airline,DepartureTime,ArrivalTime,DepartureAirportId,ArrivalAirportId")] Flight flight)
        {
            ModelState.Remove("ArrivalAirport");
            ModelState.Remove("DepartureAirport");

            if (id != flight.Id)
            {
                return NotFound();
            }

            if (flight.DepartureTime != default && flight.ArrivalTime != default)
            {
                if (flight.ArrivalTime <= flight.DepartureTime)
                {
                    ModelState.AddModelError(nameof(flight.ArrivalTime), "Arrival time must be after departure time.");
                }
                else if ((flight.ArrivalTime - flight.DepartureTime).TotalHours > 24)
                {
                    ModelState.AddModelError(nameof(flight.ArrivalTime), $"Flight duration cannot exceed 24 hours. Current: {(flight.ArrivalTime - flight.DepartureTime).TotalHours:F1}h");
                }
            }

            if (flight.DepartureAirportId == flight.ArrivalAirportId)
            {
                ModelState.AddModelError(nameof(flight.ArrivalAirportId), "Departure and Arrival airports must be different.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateAirportsDropDown(flight.ArrivalAirportId, flight.DepartureAirportId);
            return View(flight);
        }


        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> FlipAirports(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return Json(new { success = false, error = "Flight not found." });
            }

            // Swap airports
            var tempAirport = flight.DepartureAirportId;
            flight.DepartureAirportId = flight.ArrivalAirportId;
            flight.ArrivalAirportId = tempAirport;

            // Swap times
            var tempTime = flight.DepartureTime;
            flight.DepartureTime = flight.ArrivalTime;
            flight.ArrivalTime = tempTime;

            await _context.SaveChangesAsync();

            // Връщаме нови данни
            var depName = _context.Airports.Find(flight.DepartureAirportId)?.Name ?? "";
            var arrName = _context.Airports.Find(flight.ArrivalAirportId)?.Name ?? "";
            var depTimeStr = flight.DepartureTime.ToString("dd.MM.yyyy HH:mm");
            var arrTimeStr = flight.ArrivalTime.ToString("dd.MM.yyyy HH:mm");

            return Json(new
            {
                success = true,
                departureAirportName = depName,
                arrivalAirportName = arrName,
                departureTime = depTimeStr,
                arrivalTime = arrTimeStr
            });
        }


    }
}
