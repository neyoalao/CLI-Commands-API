using System;
using System.Collections.Generic;
using System.Linq;
using CommandsAPI.Models;

namespace CommandsAPI.Data
{
    public class SqlCommandsAPIRepo : ICommandsAPIRepo
    {
        private readonly CommandsContext _context;

        public SqlCommandsAPIRepo(CommandsContext context)
        {
            _context = context;
        }

        public void CreateCommand(Commands cmd)
        {
            if(cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _context.CommandItems.Add(cmd);
        }

        public void DeleteCommand(Commands cmd)
        {
            if(cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }
            _context.CommandItems.Remove(cmd);
        }

        public IEnumerable<Commands> GetAllCommands()
        {
            return _context.CommandItems.ToList();
        }

        public Commands GetCommandById(int id)
        {
            return _context.CommandItems.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCommand(Commands cmd)
        {
            //This does nothing
        }
    }
}