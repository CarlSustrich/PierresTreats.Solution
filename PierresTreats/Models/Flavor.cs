using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PierresTreats.Models;

public class Flavor 
{
  public int FlavorId {get;set;}
  [Required(ErrorMessage = "Name Field May Not Be Empty")]
  public string FlavorName {get;set;}
  public List<FlavorTreat> Joins {get;set;}
}
