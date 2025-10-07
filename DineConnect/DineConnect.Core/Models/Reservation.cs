using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.Core.Models;
public enum ReservationStatus { Pending, Confirmed, Cancelled }
public class Reservation
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public int UserId { get; set; }
    public DateTime At { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
}

