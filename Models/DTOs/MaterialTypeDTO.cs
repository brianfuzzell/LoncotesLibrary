using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models.DTOs;

public class MaterialTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CheckoutDays { get; set; }
}