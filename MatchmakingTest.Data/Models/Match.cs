using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchmakingTest.Data.Models
{
    public class Match
    {
        public string Id { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Ended { get; set; }
    }
}
