using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PierresTreats.Models;

public class Flavor 
{
  public int FlavorId {get;set;}
  public string FlavorName {get;set;}
  public List<FlavorTreat> Joins {get;set;}
}
