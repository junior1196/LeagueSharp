using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp.Common;

namespace DSTManager
{
    // helper class for menus
    class Settings
    {
        public LeagueSharp.Common.Menu Menu;

        public Settings()
        {
            Menu = DSTManager.Menu.CreateMenu();
        }

        public bool GetBool( string name )
        {
            return Menu.Item(name).GetValue<bool>();
        }

        public int GetSlider(string name)
        {
            return Menu.Item(name).GetValue<Slider>().Value;
        }
    }
}
