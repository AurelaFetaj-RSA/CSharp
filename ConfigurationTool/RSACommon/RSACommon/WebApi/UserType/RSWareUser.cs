using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSACommon.WebApiDefinitions;
using RSACommon;
using RSWareCommands;

namespace WebApi
{

    public class RecipeCommand : BaseCommand
    {
        /// <summary>
        /// The constructor second parameters (can be multiple should be string
        /// </summary>
        public RecipeCommand() : base(RSWareCommand.SET_RECIPE, "{0}")
        {
        }
    }

    public class StartCommand : BaseCommand
    {
        /// <summary>
        /// The constructor second parameters (can be multiple should be string
        /// </summary>
        public StartCommand() : base(RSWareCommand.CSTRT)
        {
        }
    }

    public class StartAutomaticCommand : BaseCommand
    {
        /// <summary>
        /// The constructor second parameters (can be multiple should be string
        /// </summary>
        public StartAutomaticCommand() : base(RSWareCommand.CSTAA)
        {
        }
    }

    public class ParkingCommand : BaseCommand
    {
        /// <summary>
        /// The constructor second parameters (can be multiple should be string
        /// </summary>
        public ParkingCommand() : base(RSWareCommand.CPARK)
        {
        }
    }

    public class StopAutomaticCommand : BaseCommand
    {
        /// <summary>
        /// The constructor second parameters (can be multiple should be string
        /// </summary>
        public StopAutomaticCommand() : base(RSWareCommand.CSTOA)
        {
        }
    }
    public class RSWareUser: User
    {
        //to add
        public RSWareUser() 
        {
            AddCommandsType(new RecipeCommand());
            AddCommandsType(new StopAutomaticCommand());
            AddCommandsType(new ParkingCommand());
            AddCommandsType(new StartAutomaticCommand());
            AddCommandsType(new StartCommand());
        }
        public string FolderPath { get; set; } = string.Empty;
    }
}
