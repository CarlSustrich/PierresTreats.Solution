using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PierresTreats.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

[Authorize]
public class TreatsController : Controller
{
  private readonly ProjectContext _db;

  public TreatsController(ProjectContext db)
  {
    _db = db;
  }
  [AllowAnonymous]
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
    {return RedirectToAction("ManageFlavors", new {id = newTreat.TreatId});}
    else
    {return RedirectToAction("Index");}
  }
  [AllowAnonymous]
  public ActionResult Details(int id)
  {
    Treat model = _db.Treats
      .Include(thing=>thing.Joins)
      .ThenInclude(thing=>thing.Flavor)
      .FirstOrDefault(otherthing => (otherthing.TreatId == id));
    return View(model);
  }

  public ActionResult ManageFlavors(int id)
  {
    Treat targetTreat = _db.Treats
      .Include(thing => thing.Joins)
      .ThenInclude(thing => thing.Flavor)
      .FirstOrDefault(book => book.TreatId == id);

    ViewBag.ExistingFlavors = targetTreat.Joins
      .Where(entry => entry.TreatId == targetTreat.TreatId)
      .Select(entry => entry.Flavor)
      .Distinct()
      .ToList();

    ViewBag.NonAssociatedFlavors = _db.Flavors
      .Include(item => item.Joins)
      .ThenInclude(item => item.Treat)
      .Where(item => !item.Joins.Any(entry => entry.TreatId == targetTreat.TreatId))
      .ToList();
  
    return View(targetTreat);
  }

  [HttpPost]
  public ActionResult ManageFlavors(List<int> flavorList, int treatId)
  {
    Treat targetTreat = _db.Treats.FirstOrDefault(item=>item.TreatId == treatId);
    IQueryable<PierresTreats.Models.FlavorTreat> flavorsToRemove = _db.FlavorTreats.Where(item => item.TreatId == treatId);
    _db.FlavorTreats.RemoveRange(flavorsToRemove);
    

    if(flavorList.Count != 0)
    {
      foreach(int flavor in flavorList)
      {
        _db.FlavorTreats.Add(new FlavorTreat() {FlavorId = flavor, TreatId = treatId });
        _db.SaveChanges();
      }
    }
    return Redirect($"/Treats/Details/{treatId}");
  }

  public ActionResult Delete(int id)
  {
    Treat model = _db.Treats
      .Include(thing=>thing.Joins)
      .ThenInclude(thing=>thing.Flavor)
      .FirstOrDefault(otherthing => (otherthing.TreatId == id));
    return View(model);
  }

  [HttpPost, ActionName("Delete")]
  public ActionResult DeleteConfirmed(int id)
  {
    Treat targetTreat = _db.Treats.FirstOrDefault(item => item.TreatId == id);
    _db.Treats.Remove(targetTreat);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }

  public ActionResult Edit(int id)
  {
    return View(_db.Treats.FirstOrDefault(thing=>thing.TreatId == id));
  }

  [HttpPost]
  public ActionResult Edit(Treat updatedTreat)
  {
    _db.Treats.Update(updatedTreat);
    _db.SaveChanges();
    TempData["Message"] = "Treat's Info Updated";
    return RedirectToAction("Index");
  }
}
