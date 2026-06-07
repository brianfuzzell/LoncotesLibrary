using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models.DTOs;

public class CheckoutWithLateFeeDTO
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int PatronId { get; set; }
    public PatronDTO Patron { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public MaterialDTO Material { get; set; }
    private static readonly decimal _lateFeePerDay = .50M;
    public decimal? LateFee
    {
        get
        {
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckoutDays);
            DateTime returnDate = ReturnDate ?? DateTime.Today;
            int daysLate = (returnDate - dueDate).Days;
            decimal fee = daysLate * _lateFeePerDay;
            return daysLate > 0 ? fee : null;
        }
    }
}