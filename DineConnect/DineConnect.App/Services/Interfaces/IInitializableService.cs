using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Services.Interfaces
{
    public interface IInitializableService
    {
        Task EnsureInitializedAsync();
    }

}
