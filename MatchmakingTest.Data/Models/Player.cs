using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MatchmakingTest.Data.Models
{
    public class Player
    {
        public string Username { get; set; }
        public bool IsOnQueue { get; set; } = false;
        public bool OnMatch { get; set; } = false;
        public DateTime? QueueStart { get; set; } = null;

        [NotMapped]
        public List<Match> MatchHistory { get; set; } = new List<Match>();
    }
}
