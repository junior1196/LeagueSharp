using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace DSTMotivator
{
    class Program
    {
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static List<string> Messages = null;
        public static List<string> Starts = null;
        public static List<string> Endings = null;
        public static List<string> Smileys = null;
        public static Random rand = new Random();

        public static bool gameStarted = false;
        public static bool wroteGreeting = false;
        public static float greetingTime = 0;

        public static int kills = 0;
        public static int deaths = 0;
        public static float congratzTime = 0;

        static void Main(string[] args)
        {
            setupMessages();

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnGameStart += Game_OnGameStart;
            Game.OnGameNotifyEvent += Game_OnGameNotifyEvent;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        static void setupMessages()
        {
            Messages = new List<string>
            {
                "gj", "good job", "very gj", "very good job",
                "wp", "well played",
                "nicely played",
                "amazing",
                "nice", "nice1", "nice one",
                "well done",
                "sweet",                
            };

            Starts = new List<string>
            {
                "",
                "oh, ", "oh ",
                "that was ", 
                "wow ", 
                "wow, "
            };

            Endings = new List<string>
            {
                "",
                " m8", " mate",
                " team",  " guys", " friends",
                " friend"
            };

            Smileys = new List<string>
            {
                "",
                " xD",
                " ;D",
                " ^^",
                " :P", " :p",
                " :O", " :o"
            };
        }

        static string getRandomElement( List<string> collection, bool firstEmpty = true )
        {
            if (firstEmpty && rand.Next(3) == 0)
                return collection[0];

            return collection[rand.Next(collection.Count)];
        }

        static string generateMessage()
        {
            string message = getRandomElement(Starts);
            message += getRandomElement(Messages, false);
            message += getRandomElement(Endings);
            message += getRandomElement(Smileys);
            return message;
        }

        static string generateGreeting()
        {
            List<string> Greetings = new List<string>
            {
                "gl", "good luck",
                "hf", "have fun",
                "gl hf", "gl and hf", "gl & hf",
            };

            string greeting = getRandomElement(Greetings, false);
            greeting += getRandomElement(Smileys);
            return greeting;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color = \"#D6B600\">DST Motivator by Rinnegan</font>");
        }


        static void Game_OnGameStart(EventArgs args)
        {
            greetingTime = Game.Time + rand.Next(30, 90);
            gameStarted = true;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            // greeting message
            if( gameStarted && !wroteGreeting && greetingTime < Game.Time )
            {
                wroteGreeting = true;
                Game.Say( "/all " + generateGreeting() );
            }

            // champ kill message
            if( kills > deaths && congratzTime < Game.Time && congratzTime != 0 )
            {
                kills = 0;
                deaths = 0;
                congratzTime = 0;
                Game.Say( generateMessage() );
            }
            else if( kills != deaths && congratzTime < Game.Time )
            {
                kills = 0;
                deaths = 0;
                congratzTime = 0;
            }
        }

        static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            // is it a champion kill
            if (args.EventId == GameEventId.OnChampionDie)
            {
                Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

                Game.PrintChat( "Killer: " + Killer.Name );

                if (Killer.IsAlly)
                {
                    if ((kills == 0 && Killer.NetworkId != Player.NetworkId) || kills > 0)
                    {
                        kills++;
                    }
                }
                else
                {
                    deaths++;
                }
            }
            // maybe it is a turret kill?
            else if (args.EventId == GameEventId.OnTurretDamage )
            {
                Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((int)args.NetworkId);

                if (Killer.IsAlly && !Killer.IsMe )
                {
                    kills++; // turret kill worth one champ kill
                }
            }
            else
            {
                return;
            }

            congratzTime = Game.Time + rand.Next(5, 10);
        }
    }
}