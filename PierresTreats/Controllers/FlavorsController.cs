using PierresTreats.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace PierresTreats.Controllers;

public class FlavorsController : Controller
{
  private readonly ProjectContext _db;

  public FlavorsController(ProjectContext db)
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
    return View(_db.Flavors.Include(thing=>thing.Joins).ThenInclude(thing=>thing.Treat).ToList());
  }

  public ActionResult Create()
  {
    return View();
  }

  [HttpPost]
  public ActionResult Create(Flavor newFlavor, bool splashOrDetails)
  {
    if(!ModelState.IsValid) {return View();}
    _db.Flavors.Add(newFlavor);
    _db.SaveChanges();
    if(splashOrDetails)
    {return RedirectToAction("AddTreat", new {id = newFlavor.FlavorId});}
    else
    {return RedirectToAction("Index");}
  }

  public ActionResult Details(int id)
  {
    Flavor model = _db.Flavors
      .Include(thing=>thing.Joins)
      .ThenInclude(thing=>thing.Treat)
      .FirstOrDefault(otherthing => (otherthing.FlavorId == id));
    return View(model);
  }

  public ActionResult ManageTreats(int id)
  {
    Flavor targetFlavor = _db.Flavors
      .Include(thing => thing.Joins)
      .ThenInclude(thing => thing.Treat)
      .FirstOrDefault(book => book.FlavorId == id);

    ViewBag.ExistingTreats = targetFlavor.Joins
      .Where(entry => entry.FlavorId == targetFlavor.FlavorId)
      .Select(entry => entry.Treat)
      .Distinct()
      .ToList();

    ViewBag.NonAssociatedTreats = _db.Treats
      .Include(item => item.Joins)
      .ThenInclude(item => item.Flavor)
      .Where(item => !item.Joins.Any(entry => entry.FlavorId == targetFlavor.FlavorId))
      .ToList();
  
    return View(targetFlavor);
  }
  
  [HttpPost]
  public ActionResult ManageTreats(List<int> treatList, int flavorId)
  {
    Flavor targetFlavor = _db.Flavors.FirstOrDefault(item=>item.FlavorId == flavorId);
    IQueryable<PierresTreats.Models.FlavorTreat> treatsToRemove = _db.FlavorTreats.Where(item => item.FlavorId == flavorId);
    _db.FlavorTreats.RemoveRange(treatsToRemove);
    

    if(treatList.Count != 0)
    {
      foreach(int treat in treatList)
      {
        _db.FlavorTreats.Add(new FlavorTreat() {FlavorId = flavorId, TreatId = treat });
        _db.SaveChanges();
      }
    }
    return Redirect($"/Flavors/Details/{flavorId}");
  }

  public ActionResult Delete(int id)
  {
    Flavor model = _db.Flavors
      .Include(thing=>thing.Joins)
      .ThenInclude(thing=>thing.Treat)
      .FirstOrDefault(otherthing => (otherthing.FlavorId == id));
    return View(model);
  }

  [HttpPost, ActionName("Delete")]
  public ActionResult DeleteConfirmed(int id)
  {
    Flavor targetFlavor = _db.Flavors.FirstOrDefault(item => item.FlavorId == id);
    _db.Flavors.Remove(targetFlavor);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }

  public ActionResult Edit(int id)
  {
    return View(_db.Flavors.FirstOrDefault(thing=>thing.FlavorId == id));
  }

  [HttpPost]
  public ActionResult Edit(Flavor updatedFlavor)
  {
    _db.Flavors.Update(updatedFlavor);
    _db.SaveChanges();
    TempData["Message"] = "Flavor's Info Updated";
    return RedirectToAction("Index");
  }
}
