using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineConnect.App.Models
{
    public enum ReservationStatus { Pending, Confirmed, Cancelled }

    public class Reservation
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public DateTime At { get; set; }
        public int PartySize { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

        // Navigation — Name will be accessible via Restaurant.Name
        public Restaurant Restaurant { get; set; } = null!;
    }

}
