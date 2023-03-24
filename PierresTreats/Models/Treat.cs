using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PierresTreats.Models;

public class Treat 
{
  public int TreatId {get;set;}
  public string TreatName {get;set;}
  public List<FlavorTreat> Joins {get;set;}
}
