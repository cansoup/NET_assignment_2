using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DineConnect.Core.Models;

namespace DineConnect.Core.Interfaces;
public record ReservationEventArgs(Reservation Reservation);
public interface IReservationService
{
    event EventHandler<ReservationEventArgs>? ReservationCreated;
    Task<Reservation> BookAsync(int restaurantId, int userId, DateTime at, int partySize);
}
