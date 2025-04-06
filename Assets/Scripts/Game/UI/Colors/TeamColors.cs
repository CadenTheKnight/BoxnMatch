using UnityEngine;
using Assets.Scripts.Game.Types;

namespace Assets.Scripts.Game.UI.Colors
{
    public static class TeamColors
    {
        public static Color GetColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.One;
            if (team == Team.Blue) return UIColors.Blue.One;
            if (team == Team.Green) return UIColors.Green.One;
            else return UIColors.Orange.One;
        }

        public static Color GetHoverColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.Two;
            if (team == Team.Blue) return UIColors.Blue.Two;
            if (team == Team.Green) return UIColors.Green.Two;
            else return UIColors.Orange.Two;
        }

        public static Color GetSelectedColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.Three;
            if (team == Team.Blue) return UIColors.Blue.Three;
            if (team == Team.Green) return UIColors.Green.Three;
            else return UIColors.Orange.Three;
        }

        public static Color GetDisabledColor(Team team)
        {
            return UIColors.Primary.Eight;
        }
    }
}