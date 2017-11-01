using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCup2018
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Team
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public int GoalDiff { get; set; }

        public Team(string name)
        {
            Name = name;
            Points = 0;
            GoalDiff = 0;
        }
    }

    class Match
    {
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }

        public Match(Team teama, Team teamb)
        {
            TeamA = teama;
            TeamB = teamb;

            ScoreA = -1;
            ScoreB = -1;
        }

        public void CompleteMatch()
        {

        }
    }

    interface IBracket
    {
        string Name { get; set; }
        List<Team> Teams { get; set; }
        List<Match> Matches { get; set; }

        List<Team> ChooseWinners();
    }

    class RoundOf32Group: IBracket
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
        public List<Match> Matches { get; set; }
        public List<Team> ChooseWinners()
        {
            return new List<Team>();
        }

        private void InitMatches()
        {
            Matches = new List<Match>
            {
                new Match(Teams.ElementAt(0), Teams.ElementAt(1)),
                new Match(Teams.ElementAt(2), Teams.ElementAt(3)),
                new Match(Teams.ElementAt(0), Teams.ElementAt(2)),
                new Match(Teams.ElementAt(3), Teams.ElementAt(1)),
                new Match(Teams.ElementAt(3), Teams.ElementAt(0)),
                new Match(Teams.ElementAt(1), Teams.ElementAt(2)),
            };
        }
    }
}
