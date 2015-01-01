using System.Linq;
using LeagueSharp;
using SharpDX;
using System.Collections;
using LeagueSharp.Common;
using System;


namespace RinneganSoraka
{
    internal static class Utils
    {
        public static void PrintMessage(string message)
        {
            Game.PrintChat("<font color='#1600DB'>Rinnegan" + Program.ChampName + ":</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        public static bool HasBuff(Obj_AI_Hero target, string buffName)
        {

            foreach (BuffInstance buff in target.Buffs)
            {
                //Utils.PrintMessage(buff.Name);
                if (buff.Name.ToLower() == buffName.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasBuff(Obj_AI_Hero target, string buffName, bool debug)
        {

            foreach (BuffInstance buff in target.Buffs)
            {
                if (debug)
                {
                    Utils.PrintMessage(buff.Name);
                }
                if (buff.Name.ToLower() == buffName.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {

            ArrayList enemies = new ArrayList();
            // Throw all the enemies in a list.
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => Vector3.Distance(ObjectManager.Player.Position, enemy.Position) < range && enemy.IsEnemy))
            {
                enemies.Add(enemy);
            }
            // Return true if the amount in range is good.
            if (enemies.Count >= numOfEnemy)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Obj_AI_Hero AllyBelowHP(int percentHP, float range)
        {
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (ally.IsAlly)
                {
                    if (Vector3.Distance(ObjectManager.Player.Position, ally.Position) < range && ((ally.Health / ally.MaxHealth) * 100) < percentHP)
                    {
                        return ally;
                    }
                }
            }

            return null;
        }

        
    }
}