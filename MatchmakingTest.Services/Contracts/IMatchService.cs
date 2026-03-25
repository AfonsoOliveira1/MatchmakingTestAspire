using MatchmakingTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingTest.Services.Services
{
    public interface IMatchService
    {
        Task<Match> GetMatch(string matchid);
        Task AddToQueue(string username);
        Task RemoveFromQueue(string username);
    }
}
