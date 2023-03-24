using PierresTreats.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace PierresTreats.Controllers;

public class TreatsController : Controller
{
  private readonly ProjectContext _db;

  public TreatsController(ProjectContext db)
  {
    _db = db;
  }

  public ActionResult Index()
  {
    if (TempData["Message"] != null)
    {
      ViewBag.Message = TempData["Message"];
      TempData.Remove("Message");
    }
    return View(_db.Treats.Include(thing=>thing.Joins).ThenInclude(thing=>thing.Flavor).ToList());
  }

  public ActionResult Create()
  {
    return View();
  }

  [HttpPost]
  public ActionResult Create(Treat newTreat, bool splashOrDetails)
  {
    if(!ModelState.IsValid) {return View();}
    _db.Treats.Add(newTreat);
    _db.SaveChanges();
    if(splashOrDetails)
    {return RedirectToAction("AddFlavore", new {id = newTreat.TreatId});}
    else
    {return RedirectToAction("Index");}
  }

  public ActionResult Details(int id)
  {
    Treat model = _db.Treats
      .Include(thing=>thing.Joins)
      .ThenInclude(thing=>thing.Flavor)
      .FirstOrDefault(otherthing => (otherthing.TreatId == id));
    return View(model);
  }
}
