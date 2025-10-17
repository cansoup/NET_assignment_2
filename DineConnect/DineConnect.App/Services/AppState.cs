using DineConnect.App.Models;

namespace DineConnect.App.Services
{
    /// <summary>
    ///     A simple static class to hold the application's current state.
    ///     such as the currently logged-in user
    /// </summary>
    public static class AppState
    {
        public static User? CurrentUser { get; set; }
    }
}
