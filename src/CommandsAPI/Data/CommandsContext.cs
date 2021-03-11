using Microsoft.EntityFrameworkCore;
using CommandsAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CommandsAPI.Data
{
    public class CommandsContext : IdentityDbContext
    {
        public CommandsContext(DbContextOptions<CommandsContext> options) : base(options)
        {

        }

        public DbSet<Identities> IdentitiesItems { get; set; }
        public DbSet<Commands> CommandItems { get; set; }
    }
}