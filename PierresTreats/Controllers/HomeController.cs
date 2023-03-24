using PierresTreats.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace PierresTreats.Controllers;

public class HomeController : Controller
{
  private readonly ProjectContext _db;

  public HomeController(ProjectContext db)
  {
    _db = db;
  }
  
  [HttpGet("/")]
  public ActionResult Index() 
  {
    if (TempData["Message"] != null)
    {
      ViewBag.Message = TempData["Message"];
      TempData.Remove("Message");
    }
    ViewBag.Treats = _db.Treats.ToList();
    ViewBag.Flavors = _db.Flavors.ToList();
    return View();
  }

}
