using MatchmakingTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingTest.Services.Services
{
    public interface IPlayerService
    {
        Task<List<Player>> GetAll();
        Task<Player> GetPlayer(string username);
        Task<List<Match>> GetPlayerMatches(string username);
        Task CreatePlayer(string username);
        Task DeletePlayer(string username);
    }
}
