using AirportSystem.Models.Data;
using AirportSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(string searchTerm)
    {
        var flights = _context.Flights
            .Include(f => f.ArrivalAirport)
            .Include(f => f.DepartureAirport)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();

            flights = flights.Where(f =>
                f.FlightNumber.ToLower().Contains(searchTerm) ||
                f.Airline.ToLower().Contains(searchTerm) ||
                f.DepartureAirport.Name.ToLower().Contains(searchTerm) ||
                f.ArrivalAirport.Name.ToLower().Contains(searchTerm) ||
                f.DepartureTime.ToString().ToLower().Contains(searchTerm) ||
                f.ArrivalTime.ToString().ToLower().Contains(searchTerm)
            );
        }

        return View(await flights.ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
