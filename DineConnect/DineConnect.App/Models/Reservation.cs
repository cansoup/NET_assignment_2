using DineConnect.App.Util;

namespace DineConnect.App.Models
{
    /// <summary>
    /// Specifies the lifecycle states a reservation can be in.
    /// </summary>
    public enum ReservationStatus { Pending, Confirmed, Cancelled }

    /// <summary>
    /// Represents a reservation made by a user at a restaurant, including date, party size, and status.
    /// </summary>
    public class Reservation : IIdentifiable
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public DateTime At { get; set; }
        public int PartySize { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    }

}
