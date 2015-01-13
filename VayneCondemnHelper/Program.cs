using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace VayneCondemnHelper
{
    class Program
    {
        private static Spell E;
        private static List<Vector2> standPositions;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            E = new Spell(SpellSlot.E, 550f);
            Game.PrintChat("VayneCondemnHelper LOADED");
        }


        static void Game_OnGameUpdate(EventArgs args)
        {
            List<Vector2> Circles = new List<Vector2>();

            foreach( Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where((target) =>
            {
                Vector2 pos = Drawing.WorldToScreen(target.ServerPosition);
                return target.IsEnemy && !target.IsDead && target.IsValid && !target.IsImmovable && target.IsVisible && pos.X < Drawing.Width && pos.X > 0 && pos.Y < Drawing.Height && pos.Y > 0;
            }) )
            {
                Vector2 targetPos = target.ServerPosition.To2D();
                Vector2 rotatedVector = target.ServerPosition.To2D().Extend(new Vector2(0, 0), E.Range);


                // now lets rotate it 360 degrees and find the right angles for it :)
                for (int i = 0; i < 30; i++)
                {
                    Vector2 curRotatedVector = rotatedVector.RotateAroundPoint(targetPos, (float)(Math.PI / 180) * 12 * i);

                    // now lets do a basic check if we can condemn from here
                    for (float j = 30; j <= 450; j += 30)
                    {
                        Vector3 curPos = targetPos.Extend(curRotatedVector, -j).To3D();

                        if (curPos.IsWall())
                        {
                            Circles.Add(curRotatedVector);
                            break;
                        }
                    }
                }
            }

            standPositions = Circles;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            float radius = 10f;

            foreach( Vector2 standPos in standPositions.ToArray() )
            {
                Utility.DrawCircle(standPos.To3D(), radius, System.Drawing.Color.Yellow);
            }
        }
    }
}
