namespace DineConnect.App.Services.Interfaces
{
    public interface IInitializableService
    {
        Task EnsureInitializedAsync();
    }

}
