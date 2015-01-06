using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// load LeagueSharp libraries
using LeagueSharp;
using LeagueSharp.Common;

namespace DSTManager
{
    class Program
    {
        // our settings
        public static Settings Settings;
        public static Random Rand = new Random();

        // teammates who killed & helped them kill enemies
        public static List<Obj_AI_Base> Killers = new List<Obj_AI_Base>();
        public static List<Obj_AI_Base> Assisters = new List<Obj_AI_Base>();
        public static List<Obj_AI_Base> DeadTeammates = new List<Obj_AI_Base>();

        // players and their assists
        public static Dictionary<int, int> Teammates;

        // our messenger responsible for messaging
        public static Messenger Messenger = new Messenger();

        // our team kills & deaths
        public static int Kills = 0;
        public static int Deaths = 0;

        // now the timers
        public static float CongratulateTimer = 0;
        public static float ApologizeTimer = 0;

        public static void Main(string[] args)
        {
            // on load we will create our menu
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;

            // on game Start we will initiate a greeting message
            Game.OnGameStart += Game_OnGameStart;

            // on game End we will send a nice GG WP message :)
            Game.OnGameEnd += Game_OnGameEnd;

            // here we will catch all the kills & assists
            Game.OnGameNotifyEvent += Game_OnGameNotifyEvent;

            // finally all the magic will happen here
            Game.OnGameUpdate += Game_OnGameUpdate;
        }


        private static void SetCongratulateTimer()
        {
            CongratulateTimer = Game.Time + GenerateDelay("congratulateRandMin", "congratulateRandMax");
        }

        private static void SetApologizeTimer()
        {
            ApologizeTimer = Game.Time + GenerateDelay("apologizeRandMin", "apologizeRandMax");
        }

        private static int GenerateDelay(string minMenuKey, string maxMenuKey)
        {
            return Rand.Next(Settings.GetSlider(minMenuKey), Settings.GetSlider(maxMenuKey));
        }

        static void Game_OnGameEnd(GameEndEventArgs args)
        {
            Messenger.Send("GG WP");
        }

        static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            switch (args.EventId)
            {
                case GameEventId.OnChampionDie: // somebody got a kill
                    ProcessChampionKill(args);
                    break;
                case GameEventId.OnChampionKill: // somebody died, 
                    ProcessChampionDeath(args);
                    break;
                case GameEventId.OnTurretDamage: // somebody destroyed a turret
                    ProcessTurretKill(args);
                    break;
            }
        }

        private static void ProcessTurretKill(GameNotifyEventArgs args)
        {
            // lets get the killer
            Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

            // we dont want minion help
            if (Killer.IsMinion) return;

            // was it us? if so, then lets not congratulate us unless someone else had done something notable aswell
            if (Killer.IsMe && Kills == 0) return;

            // was it enemy? if so then we dont care
            if (Killer.IsEnemy) return;

            if (Killers.Count(killer => killer.NetworkId == Killer.NetworkId) == 0) Killers.Add(Killer);

            Kills++;

            // lets reset our timer now
            SetCongratulateTimer();
        }


        // this callback doesnt work at the moment :(
        // so we will call it from a function. yay xd
        private static void ProcessChampionAssist(GameNotifyEventArgs args)
        {
            // lets get the killer
            Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

            // we dont want minion help
            if (Killer.IsMinion) return;

            // was it us? if so, then lets not congratulate us unless someone else had done something notable aswell
            if (Killer.IsMe && Kills == 0) return;

            // was it enemy? if so then we dont care
            if (Killer.IsEnemy) return;

            if (Assisters.Count(killer => killer.NetworkId == Killer.NetworkId) == 0) Assisters.Add(Killer);
        }

        private static void ProcessChampionDeath(GameNotifyEventArgs args)
        {
            // lets get the player who died
            Obj_AI_Base Dead = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

            if (!Dead.IsAlly) return;

            DeadTeammates.Add(Dead);

            // we are gonna apologize only for ourselves lol
            if (!Dead.IsMe) return;

            // lets reset our timer now
            SetApologizeTimer();
        }

        private static void ProcessChampionKill(GameNotifyEventArgs args)
        {
            // lets get the killer
            Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

            // was it teammate?
            if (Killer.IsAlly)
            {
                if (Killers.Count(killer => killer.NetworkId == Killer.NetworkId) == 0) Killers.Add(Killer);

                Kills++;
            }
            else
            {
                Deaths++;
            }

            // lets reset our timer now
            SetCongratulateTimer();


            // update assist table
            foreach (Obj_AI_Hero p in ObjectManager.Get<Obj_AI_Hero>().Where(player => player.IsAlly && Teammates[player.NetworkId] < player.Assists))
            {
                Teammates[p.NetworkId] = p.Assists;
                ProcessChampionAssist(new GameNotifyEventArgs(GameEventId.OnDeathAssist, (uint)p.NetworkId));
            }
        }

        static void Game_OnGameStart(EventArgs args)
        {
            float greetEnemies = Rand.Next(30, 60);
            float greetTeam = greetEnemies + Rand.Next(5, 20);

            // lets greet our enemies with peace
            Utility.DelayAction.Add((int)greetEnemies * 1000, Messenger.GreetEveryone);

            // and then lets smack them by getting our teammates morale boosted
            Utility.DelayAction.Add((int)greetTeam * 1000, Messenger.GreetTeam);
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            // create our menu, load helpers etc
            Settings = new Settings();
            Game.PrintChat("DST Manager LOADED. Are you ready for becoming a challenger? by Rinnegan");

            Teammates = new Dictionary<int, int>();

            // lets load the assist table
            foreach (Obj_AI_Hero p in ObjectManager.Get<Obj_AI_Hero>().Where(p => p.IsAlly))
            {
                Teammates.Add(p.NetworkId, p.Assists);
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            CheckCongratulate();
            CheckApologize();
        }

        private static void CheckCongratulate()
        {
            if (CongratulateTimer > 0 && CongratulateTimer < Game.Time)
            {
                if (Kills > Deaths)
                {
                    // lets remove ourself lol
                    Killers = (List<Obj_AI_Base>)Killers.Where(killer => !killer.IsMe);
                    Assisters = (List<Obj_AI_Base>)Assisters.Where(killer => !killer.IsMe);

                    if (Assisters.Count == 0 && Killers.Count == 0)
                    {
                        // so i did all this? nice, be lets not congratulate ourselves lol :D 
                    }
                    else
                    {
                        Messenger.CongratulateTeam((Assisters.Count + Killers.Count > 1));
                    }
                }
                else if (Kills < Deaths && !(Deaths == 1 && DeadTeammates.First().IsMe))
                {
                    // we did poorly, time to cheer em up
                    if (DeadTeammates.Count(killer => killer.NetworkId == ObjectManager.Player.NetworkId) > 0)
                    {
                        Deaths--;
                    }

                    if (Deaths == 1)
                    {
                        Messenger.MotivatePlayer();
                    }
                    else
                    {
                        Messenger.MotivateTeam();
                    }

                }

                Killers.Clear();
                Assisters.Clear();
                DeadTeammates.Clear();
                Kills = 0;
                Deaths = 0;
                CongratulateTimer = 0;
            }
        }

        private static void CheckApologize()
        {
            if (ApologizeTimer > 0 && ApologizeTimer < Game.Time)
            {
                if (Kills == 0 && Deaths < 3)
                {
                    if (Rand.Next(2) == 1)
                        Messenger.ApologizeForMistakes();
                    else
                        Messenger.MakeAnExcuseToEnemy();
                }

                ApologizeTimer = 0;
            }
        }


    }
}
