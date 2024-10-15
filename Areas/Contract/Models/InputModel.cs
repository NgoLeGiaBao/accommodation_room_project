using App.Models;
namespace App.Areas.Contract
{
    public class InputModel
    {
        public AppUser appUser { get; set; }
        public RentalContract rentalContract { get; set; }
        public int RoomId { get; set; }
    }
}