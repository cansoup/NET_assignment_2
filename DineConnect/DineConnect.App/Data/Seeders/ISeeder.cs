using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Data.Seeders
{
    /// <summary>
    /// Defines a interface for database seeders that populate initial data into the DineConnect database.
    /// </summary>
    public interface ISeeder
    {
        Task SeedAsync(DineConnectContext db);
    }
}
