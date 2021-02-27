using Microsoft.EntityFrameworkCore;
using CommandsAPI.Models;

namespace CommandsAPI.Data
{
    public class CommandsContext : DbContext
    {
        public CommandsContext(DbContextOptions<CommandsContext> options) : base(options)
        {

        }

        public DbSet<Commands> CommandItems {get; set;}
    }
}