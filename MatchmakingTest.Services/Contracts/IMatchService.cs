using MatchmakingTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingTest.Services.Services
{
    public interface IMatchService
    {
        Task<Match> GetMatch(string matchid);
        Task StartMatch(Match match);
        Task AddMatch(Match match);
        Task AddToQueue(string username);
        Task RemoveFromQueue(string username);
        Task<(Player p1, Player p2)> GetTwoPlayersAsync();
        Task EndMatch(string matchid);
    }
}
