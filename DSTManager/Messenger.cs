using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// leaguesharp
using LeagueSharp;
using LeagueSharp.Common;

namespace DSTManager
{
    class Messenger
    {
        // messages for congratulating team on success
        private List<string> CongratulationPhrases;

        // generic beginnings
        private List<string> BeginningPhrases;

        // messages for failure
        private List<string> TeamFailurePhrases;
        private List<string> MotivatingPhrases;
        private List<string> MyFailurePhrases;
        private List<string> MyAllChatFailurePhrases;

        // teammates
        private List<string> SingleTeammatePhrases;
        private List<string> MultipleTeammatePhrases;
        private List<string> EveryonePhrases;

        // messages for greeting
        private List<string> GreetingAllPhrases;
        private List<string> GreetingTeamPhrases;

        // smileys
        private List<string> HappySmileys;
        private List<string> WowSmileys;
        private List<string> SadSmileys;

        // replaceables
        private Dictionary<string, List<string>> Replaceables;

        // last message sent
        private float lastSent = 0;

        public Messenger()
        {
            // set up generic beginnings
            AddGenericBeginnings();

            AddCongratulationPhrases();
            AddTeamFailurePhrases();
            AddMotivatingPhrases();
            AddMyFailurePhrases();
            AddMyAllChatFailurePhrases();
            AddGreetingAllPhrases();
            AddGreetingTeamPhrases();
            
            // smileys
            AddSmileys();

            // teammate phrases
            AddTeammatePhrases();

            // add Replaceables
            AddReplaceables();
        }

        public string GetFormattedString( string text, int teammateMode = 1 )
        {
            // teammmates need extra handling

            string teammate = "";

            if (teammateMode == 1)
                teammate = this.GetRandom(SingleTeammatePhrases, 20);
            else if (teammateMode == 2)
                teammate = this.GetRandom(MultipleTeammatePhrases, 20);
            else if (teammateMode == 3)
                teammate = this.GetRandom(EveryonePhrases, 20);

            text = text.Replace("{T}", teammate);


            foreach( string key in this.Replaceables.Keys )
            {
                text = text.Replace( key, this.GetRandom( this.Replaceables[ key ], 20 ));
            }

            return text;
        }

        private bool CanSend()
        {
            float delay = Game.ClockTime - this.lastSent;
            if ( this.lastSent > 0 && delay < Program.Settings.GetSlider("minDelay") ) return false;

            return true;
        }

        public void GreetEveryone()
        {
            if (!Program.Settings.GetBool("greetEnemies")) return;

            this.Send(
                this.GetFormattedString(this.GetRandom(GreetingAllPhrases), 3)
            , true);
        }

        public void GreetTeam()
        {
            if (!Program.Settings.GetBool("greetTeam")) return;

            this.Send(
                this.GetFormattedString(this.GetRandom(GreetingTeamPhrases) + " :)", 2)
            );
        }

        public void MakeAnExcuseToEnemy()
        {
            if (!Program.Settings.GetBool("excuseForDeath") || !this.CanSend()) return;

            this.Send(
                this.GetFormattedString(this.GetRandom( MyAllChatFailurePhrases ))
            );

            lastSent = Game.ClockTime;
        }

        public void ApologizeForMistakes()
        {
            if (!Program.Settings.GetBool("apologizeTeam") || !this.CanSend()) return;

            this.Send(
                this.GetFormattedString(this.GetRandom(MyFailurePhrases) + " :(", 2)
            );

            lastSent = Game.ClockTime;
        }

        public void CongratulateTeam(bool plural)
        {
            if (!Program.Settings.GetBool("congratulateTeam") || !this.CanSend()) return;

            this.Send(
                this.GetFormattedString("{B} " + this.GetRandom(CongratulationPhrases) + " {T} :)", (plural ? 2 : 1))
            );


            lastSent = Game.ClockTime;
        }

        public void MotivateTeam()
        {
            if (!Program.Settings.GetBool("motivateTeam") || !this.CanSend()) return;

            this.Send(
                this.GetFormattedString(this.GetRandom(MotivatingPhrases) + " :)", 2)
            );


            lastSent = Game.ClockTime;
        }

        public void MotivatePlayer()
        {
            if (!Program.Settings.GetBool("motivateTeam") || !this.CanSend()) return;

            this.Send(
                this.GetFormattedString(this.GetRandom(TeamFailurePhrases) + " :)", 1)
            );

            lastSent = Game.ClockTime;
        }

        private void AddReplaceables()
        {
            this.Replaceables = new Dictionary<string, List<string>>
            {
                { ":)", HappySmileys },
                { ":(", SadSmileys },
                { ":O", WowSmileys },
                { "{B}", BeginningPhrases },
            };
        }

        private string GetRandom(List<string> Texts, int chanceEmpty = 0)
        {
            if (chanceEmpty > 0 && Program.Rand.Next(100) < chanceEmpty)
                return "";

            return Texts[Program.Rand.Next(Texts.Count)];
        }

        private void AddGenericBeginnings()
        {
            this.BeginningPhrases = new List<string>
            {
                "That was", "wow", "wow,", "oh wow", "oh wow,", "oh"
            };
        }

        private void AddTeammatePhrases()
        {
            this.SingleTeammatePhrases = new List<string>
            {
                "m8", "mate", "friend", "dude", "buddy", "man"
            };

            this.MultipleTeammatePhrases = new List<string>
            {
                "mates", "friends", "team", "guys", "buddies", "all", "y'all", "yall"
            };

            this.EveryonePhrases = new List<string>
            {
                "guys", "friends", "guys & girls", "people", "all", "everyone", "everybody"
            };
        }

        private void AddSmileys()
        {
            this.HappySmileys = new List<string>
            {
                ":^)", ":P", "^^", "xD", ";D", ":D", "=)", ";P" 
            }; 

            this.WowSmileys = new List<string>
            {
                ":=O", ":O", ";O", ";o", ":o"
            }; 

            this.SadSmileys = new List<string>
            {
                ":(", ":S", ";(", ";S", ":((", "=("
            };
        }

        private void AddGreetingTeamPhrases()
        {
            this.GreetingTeamPhrases = new List<string>
            {
                "Good Luck, Let's win this {T}",
                "Best of Luck to you {T}",
                "Have fun {T}",
                "Let's wreck them {T}",
                "Lets play together like the best team in the world {T}",
                "Have fun & Dont flame & Dont Blame {T}",
                "Wish you all the best {T}",
                "Let the games begin! Have fun {T}",
                "C`mon, lets have a nice game and beat them {T}",
                "Good Luck at your lanes {T}",
                "This game will be ours {T}"
            };
        }

        private void AddGreetingAllPhrases()
        {
            this.GreetingAllPhrases = new List<string>
            {
                "Hi, good luck and have fun {T}!",
                "Good luck & Have fun {T}!",
                "Let's have a nice game of LoL {T}",
                "Are you ready to feel my wrath? GL and HF {T}!",
                "Do you ever feel like a plastic bag?",
                "Have a nice game {T}",
                "Good Luck {T}",
                "Have fun {T}",
                "gl & hf {T}",
                "GL & HF",
                "GL HF"
            };
        }

        private void AddMyAllChatFailurePhrases()
        {
            this.MyAllChatFailurePhrases = new List<string>
            {
                "you just gut lucky",
                "lucker",
                "what a luck",
                "wow so lucky",
                "just wow",
                "damn lags",
                "omg rito fix these damn laggs",
                "rito pls fix lags",
                "I blame it on Rito laggs",
                "cmon i was lagging",
                "if it wasnt for these damn laggs"
            };
        }

        private void AddMyFailurePhrases()
        {
            this.MyFailurePhrases = new List<string>
            {
                "sorry {T}",
                "lol sorry {T}",
                "he just got lucky {T}",
                "it wont happen again {T}",
                "ahh it was so close",
                "lol",
                "haha",
                "that was stupid of me",
                "oh man",
                "my bad {T}",
                "lucker",
                "i wont let it happen again {T}",
                "ima do better now {T}"
            };
        }

        private void AddMotivatingPhrases()
        {
            this.MotivatingPhrases = new List<string>
            {
                "dont worry {T}", 
                "we still got dis {T}", 
                "we still got this {T}",
                "dont worry {T}, we still got dis", 
                "dont worry, we still got this {T}",
                "we can still win {T}", 
                "we will win no matter what {T}", 
                "cmon, we can do this", 
                "cmon, we can do dis", 
                "lets rethink our strategy and win them",
                "cmon {T}, i know we can beat them",
                "cmon, i know we can win them",
                "{T} dont give up, we can still win",
                "just dont give up, we are gonna smash them",
                "they just got lucky",
                "luck was on their side, dont give up {T}",
                "i believe in us, we will win this one {T}"
            };
        }

        private void AddTeamFailurePhrases()
        {
            this.TeamFailurePhrases = new List<string>
            {
                "lol {T}", 
                "haha {T}", 
                "that was funny {T}", 
                "lol that was lame {T}", 
                "dont worry ull get him next time {T}", 
                "he just got lucky", 
                "damn he is in luck", 
                "we`ll get that noob"
            };
        }

        private void AddCongratulationPhrases()
        {
            this.CongratulationPhrases = new List<string>
            {
                "wp", "gj", "nice1", "nice one", "well done", "well played", "good job", "nicely done", "nicely played", "nice job"
            };
        }

        public void Send( string message, bool allchat = false )
        {
            if (allchat)
                message = "/all " + message;

            Game.Say(message);
        }
    }
}
