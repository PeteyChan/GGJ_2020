using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    static GameSettings()
    {
        Players = new List<PlayerInfo>();
        for (int i = 0; i < 4; ++i)
            Players.Add(new PlayerInfo());
    }

    static List<PlayerInfo> Players;
    public static PlayerInfo GetPlayerInfo(int player) =>(player >= 0 && player <= 3) ? Players[player] : new PlayerInfo();
    public enum Team
    {
        Red,
        Blue
    }
    
    public class PlayerInfo
    {
        public bool Playing;
        public Team Team = Team.Red;
    }

    public static List<(int player, PlayerInfo info)> GetPlayers(Team team)
    {
        List<(int, PlayerInfo)> info = new List<(int, PlayerInfo)>();
        for (int i = 0; i < Players.Count; ++i)
            if (Players[i].Team == team && Players[i].Playing)
                info.Add((i, Players[i]));
        return info;

    }

    public static bool CanStart()
    {
        int redCount = 0;
        int blueCount = 0;

        foreach (var player in Players)
            if (player.Playing)
            {
                if (player.Team == Team.Red)
                    redCount++;
                else blueCount++;
            }
        return redCount > 0 && blueCount > 0;
    }
}
