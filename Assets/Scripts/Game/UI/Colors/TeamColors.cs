using UnityEngine;
using Assets.Scripts.Game.Types;

namespace Assets.Scripts.Game.UI.Colors
{
    public static class TeamColors
    {
        public static Color GetColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.One;
            else return UIColors.Blue.One;
        }

        public static Color GetHoverColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.Two;
            else return UIColors.Blue.Two;
        }

        public static Color GetSelectedColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.Three;
            else return UIColors.Blue.Three;
        }

        public static Color GetDisabledColor(Team team)
        {
            if (team == Team.Red) return UIColors.Red.Four;
            else return UIColors.Blue.Four;
        }
    }
}