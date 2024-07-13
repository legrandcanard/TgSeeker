
using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Contexts;

namespace TgSeeker.Persistent
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync()
        {
            try
            {
                using var dbContext = new ApplicationContext();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Database initialization failed", ex);
            }
        }
    }
}
