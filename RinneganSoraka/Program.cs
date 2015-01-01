using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace RinneganSoraka
{
    class Program
    {
        public static string ChampName = "Soraka";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;

        public static Menu MainMenu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Utils.PrintMessage("Script loaded!");

            // skins
            Q = new Spell(SpellSlot.Q, 970);
            W = new Spell(SpellSlot.W, 450f);
            E = new Spell(SpellSlot.E, 925f);
            R = new Spell(SpellSlot.R, float.MaxValue);

            // set skillshot types


            // draw menus
            DrawMenus();


            // add callbacks
            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)
        }

        static void DrawMenus()
        {
            //Base menu
            MainMenu = new Menu("Rinnegan" + ChampName, "Rinnegan" + ChampName, true);

            //Orbwalker and menu
            MainMenu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(MainMenu.SubMenu("Orbwalker"));

            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            MainMenu.AddSubMenu(ts);

            //Combo menu
            MainMenu.AddSubMenu(new Menu("Combo", "Combo"));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Exploits
            MainMenu.AddItem(new MenuItem("Packets", "Use Packet Casting").SetValue(true));

            //Make the menu visible
            MainMenu.AddToMainMenu();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (MainMenu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            // te kaut kas buus;
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (target == null) return;



            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, MainMenu.Item("Packets").GetValue<bool>());

            }

            if (target.IsValidTarget(W.Range) && W.IsReady())
            {
                W.Cast(target, MainMenu.Item("Packets").GetValue<bool>());
            }

            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target, MainMenu.Item("Packets").GetValue<bool>());
            }

            if (target.IsValidTarget(R.Range) && R.IsReady())
            {
                R.Cast(target, MainMenu.Item("Packets").GetValue<bool>());
            }
        }
    }
}