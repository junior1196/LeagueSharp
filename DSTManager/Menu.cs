using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace DSTManager
{
    class Menu
    {
        public static LeagueSharp.Common.Menu CreateMenu()
        {
            // create the menu here
            LeagueSharp.Common.Menu Menu = new LeagueSharp.Common.Menu("DST Manager", "DSTManager", true);
            
            // greetings
            Menu.AddItem(new MenuItem("greetEnemies", "Say hello to enemies").SetValue<bool>(true));
            Menu.AddItem(new MenuItem("greetTeam", "Say hello to team").SetValue<bool>(true));

            // congratulate team on kills & towers
            Menu.AddItem(new MenuItem("congratulateTeam", "Congratulate team on kills").SetValue<bool>(true));
            Menu.AddItem(new MenuItem("motivateTeam", "Motivate teammates when playing bad").SetValue<bool>(true));
            Menu.AddItem(new MenuItem("congratulateRandMin", "Minimum of rand delay").SetValue<Slider>(new Slider(5, 1, 300)));
            Menu.AddItem(new MenuItem("congratulateRandMax", "Minimum of rand delay").SetValue<Slider>(new Slider(15, 1, 300)));

            // Apologize to team
            Menu.AddItem(new MenuItem("apologizeTeam", "Apologize to team for dying").SetValue<bool>(true));
            Menu.AddItem(new MenuItem("excuseForDeath", "Make excuse for death in all chat").SetValue<bool>(true));
            Menu.AddItem(new MenuItem("apologizeRandMin", "Minimum of rand delay").SetValue<Slider>(new Slider(5, 1, 300)));
            Menu.AddItem(new MenuItem("apologizeRandMax", "Minimum of rand delay").SetValue<Slider>(new Slider(15, 1, 300)));

            // minimum delay
            Menu.AddItem(new MenuItem("minDelay", "Minimum interval between messages").SetValue<Slider>(new Slider(30, 1, 600)));

            // add debug info
            Menu.AddItem(new MenuItem("debug", "Debug").SetValue<bool>(true));

            // add it to the main menu
            Menu.AddToMainMenu();

            return Menu;
        }

    }
}
