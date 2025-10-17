using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Data.Seeders
{
    public interface ISeeder
    {
        Task SeedAsync(DineConnectContext db);
    }
}
