using UnityEngine;
using Assets.Scripts.Game.Types;

namespace Assets.Scripts.Game.UI.Colors
{
    /// <summary>
    /// Provides color information for different teams.
    /// </summary>
    public static class TeamColors
    {
        public static Color GetColor(Team team)
        {
            switch (team)
            {
                case Team.Red:
                    return UIColors.redDefaultColor;
                case Team.Blue:
                    return UIColors.blueDefaultColor;
                case Team.Green:
                    return UIColors.greenDefaultColor;
                case Team.Yellow:
                    return UIColors.yellowDefaultColor;
                default:
                    return Color.white;
            }
        }

        public static Color GetHoverColor(Team team)
        {
            switch (team)
            {
                case Team.Red:
                    return UIColors.redHoverColor;
                case Team.Blue:
                    return UIColors.blueHoverColor;
                case Team.Green:
                    return UIColors.greenHoverColor;
                case Team.Yellow:
                    return UIColors.yellowHoverColor;
                default:
                    return Color.white;
            }
        }
    }
}