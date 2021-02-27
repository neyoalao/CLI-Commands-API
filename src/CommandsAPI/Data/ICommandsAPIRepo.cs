
using System.Collections.Generic;
using CommandsAPI.Models;

namespace CommandsAPI.Data
{
    public interface ICommandsAPIRepo
    {
        bool SaveChanges();

        IEnumerable<Commands> GetAllCommands();
        Commands GetCommandById(int id);
        void CreateCommand(Commands cmd);
        void UpdateCommand(Commands cmd);
        void DeleteCommand(Commands cmd);
    }
}