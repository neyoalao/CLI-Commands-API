using CommandsAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CommandsAPI.Models
{
    public static class PrepDB
    {
        public static void CreateMigration(IApplicationBuilder application)
        {
            using(var serviceScope = application.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<CommandsContext>());
            }

            System.Console.WriteLine("Migrations Successful!!!!!!!");

        }

        public static void SeedData(CommandsContext context)
        {
            System.Console.WriteLine("Applying Migrations.........");

            context.Database.Migrate();
        }
    }
}