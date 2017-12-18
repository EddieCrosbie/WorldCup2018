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
        public int GoalsScored { get; set; }
        public int GoalDiff { get; set; }

        public Team(string name)
        {
            Name = name;
            Points = 0;
            GoalsScored = 0;
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
            CalculateGoalDifference();
            CalculatePoints();
            AddGoalsScored();
        }

        public int GetScoreByTeamName(string Name)
        {
            return TeamA.Name == Name ? ScoreA : ScoreB;
        }

        public int GetPointsByTeamName(string Name)
        {
            if(ScoreA == ScoreB)
            {
                return 1;
            }

            var homeTeamScore = TeamA.Name == Name ? ScoreA : ScoreB;

            if(homeTeamScore == ScoreA)
            {
                if(homeTeamScore > ScoreB)
                {
                    return 3;
                }
                return 0;
            }

            if (homeTeamScore > ScoreA)
            {
                return 3;
            }

            return 0;
            
            
        }

        public int GetGoalDifferenceByTeamName(string Name)
        {
            int goalDiff = ScoreA - ScoreB;
            return Name == TeamA.Name ? goalDiff : goalDiff * -1;
        }

        private void CalculatePoints()
        {
            if(ScoreA == ScoreB)
            {
                TeamA.Points += 1;
                TeamB.Points += 1;
            }

            else if(ScoreA > ScoreB)
            {
                TeamA.Points += 3;
            }

            else
            {
                TeamB.Points += 3;
            }


        }

        private void CalculateGoalDifference()
        {
            TeamA.GoalDiff += ScoreA - ScoreB;
            TeamB.GoalDiff += ScoreB - ScoreA;
        }

        private void AddGoalsScored()
        {
            TeamA.GoalsScored += ScoreA;
            TeamB.GoalsScored += ScoreB;
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
        public MatchHistory MatchHistory { get; set; }

        public List<Team> ChooseWinners()
        {
            foreach(Match match in Matches)
            {
                match.CompleteMatch();
                MatchHistory.Matches.Add(match);
            }

            return new List<Team>();
        }

        private Team RankTeams(List<Team> teamsSubset)
        {
            List<Team> remainingTeams = Teams;
            remainingTeams = GetHighestPoints(remainingTeams);
            remainingTeams = GetHighestGoalDifference(remainingTeams);
            remainingTeams = GetGreatestNumberOfGoalsScored(remainingTeams);
            remainingTeams = HighestPointsTieBreaker(remainingTeams);
            remainingTeams = HighestGoalDifferenceTieBreaker(remainingTeams);
            remainingTeams = GetGreatestNumberOfPointsFromSubset(remainingTeams);
            
        }

        private List<Team> GetHighestPoints(List<Team> teamsSubset)
        {
            var highestPointTeams = new List<Team>();
            teamsSubset.OrderByDescending(m => m.Points);
            var scoreToTie = teamsSubset.ElementAt(0).Points;

            foreach(Team team in teamsSubset)
            {
                if(team.Points == scoreToTie)
                {
                    highestPointTeams.Add(team);
                }
            }

            return highestPointTeams;
        }

        private List<Team> GetHighestGoalDifference(List<Team> teamsSubset)
        {
            var highestGoalDifferenceTeam = new List<Team>();
            teamsSubset.OrderByDescending(m => m.GoalDiff);
            var goalDiffToTie = teamsSubset.ElementAt(0).GoalDiff;

            foreach(Team team in teamsSubset)
            {
                if(team.GoalDiff == goalDiffToTie)
                {
                    highestGoalDifferenceTeam.Add(team);
                }
            }

            return highestGoalDifferenceTeam;
        }

        private List<Team> GetGreatestNumberOfGoalsScored(List<Team> teamsSubset)
        {
            List<Team> toReturn = new List<Team>();
            teamsSubset.OrderByDescending(m => m.GoalsScored);
            int scoreToBeat = teamsSubset.ElementAt(0).GoalsScored;

            foreach(Team team in teamsSubset)
            {
                if(team.GoalsScored >= scoreToBeat)
                {
                    toReturn.Add(team);
                }
            }

            return toReturn;
           
        }

        private List<Team> GetGreatestNumberOfPointsFromSubset(List<Team> teamsSubset)
        {
            List<TeamPoints> scoreKeeper = new List<TeamPoints>();
            List<Match> tempMatches;
            List<Team> toReturn = new List<Team>();

            MatchHistory matchHistorySubset = new MatchHistory(MatchHistory.GetMatchesPlayedByTeamsInList(teamsSubset));

            foreach (Team team in teamsSubset)
            {
                scoreKeeper.Add(new TeamPoints(team));
            }

            foreach (TeamPoints teamPoints in scoreKeeper)
            {
                tempMatches = matchHistorySubset.GetMatchesBySingleTeam(teamPoints.Team);

                foreach (Match match in tempMatches)
                {
                    teamPoints.Points += match.GetGoalDifferenceByTeamName(teamPoints.Team.Name);
                }
            }

            scoreKeeper.OrderByDescending(team => team.Points);

            int scoreToBeat = scoreKeeper.ElementAt(0).Points;

            foreach (TeamPoints teamPoints in scoreKeeper)
            {
                if (teamPoints.Points >= scoreToBeat)
                {
                    toReturn.Add(teamPoints.Team);
                }
            }

            return new List<Team>();
        }

        private List<Team> HighestPointsTieBreaker(List<Team> teamsSubset)
        {
            List<TeamPoints> scoreKeeper = new List<TeamPoints>();
            List<Match> tempMatches;
            List<Team> toReturn = new List<Team>();

            MatchHistory matchHistorySubset = new MatchHistory(MatchHistory.GetMatchesPlayedByTeamsInList(teamsSubset));

            foreach(Team team in teamsSubset)
            {
                scoreKeeper.Add(new TeamPoints(team));
            }

            foreach(TeamPoints teamPoints in scoreKeeper)
            {
                tempMatches = matchHistorySubset.GetMatchesBySingleTeam(teamPoints.Team);

                foreach (Match match in tempMatches)
                {
                    teamPoints.Points += match.GetPointsByTeamName(teamPoints.Team.Name);
                }
            }

            scoreKeeper.OrderByDescending(team => team.Points);

            int scoreToBeat = scoreKeeper.ElementAt(0).Points;

            foreach(TeamPoints teamPoints in scoreKeeper)
            {
                if(teamPoints.Points >= scoreToBeat)
                {
                    toReturn.Add(teamPoints.Team);
                }
            }

            return toReturn;

        }

        private List<Team> HighestGoalDifferenceTieBreaker(List<Team> teamsSubset)
        {
            List<TeamPoints> scoreKeeper = new List<TeamPoints>();
            List<Match> tempMatches;
            List<Team> toReturn = new List<Team>();

            MatchHistory matchHistorySubset = new MatchHistory(MatchHistory.GetMatchesPlayedByTeamsInList(teamsSubset));

            foreach (Team team in teamsSubset)
            {
                scoreKeeper.Add(new TeamPoints(team));
            }

            foreach (TeamPoints teamPoints in scoreKeeper)
            {
                tempMatches = matchHistorySubset.GetMatchesBySingleTeam(teamPoints.Team);

                foreach (Match match in tempMatches)
                {
                    teamPoints.Points += match.GetGoalDifferenceByTeamName(teamPoints.Team.Name);
                }
            }

            scoreKeeper.OrderByDescending(teamPoints => teamPoints.Points);

            var scoreToBeat = scoreKeeper.ElementAt(0).Points;

            foreach (TeamPoints teamPoints in scoreKeeper)
            {
                if(teamPoints.Points >= scoreToBeat)
                {
                    toReturn.Add(teamPoints.Team);
                }
            }

            return toReturn;
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

    class TeamPoints
    {
        public Team Team { get; set; }
        public int Points { get; set; }

        public TeamPoints(Team team)
        {
            Team = team;
            Points = 0;
        }
    }


    class MatchHistory
    {
        public List<Match> Matches { get; set; }

        public MatchHistory()
        {
            Matches = new List<Match>();
        }

        public MatchHistory(List<Match> matches)
        {
            Matches = matches;
        }

        public int GoalDiffBetweenTwoTeams(Team teama, Team teamb)
        {
            var teamGoalDiff = 0;
            var matchesToCompare = GetMatchesByTeamNames(teama, teamb);
            foreach(Match match in matchesToCompare)
            {
                var score1 = match.GetScoreByTeamName(teama.Name);
                var score2 = match.GetScoreByTeamName(teamb.Name);

                teamGoalDiff += score1 - score2;
            }
            return teamGoalDiff;

        }

        public List<Match> GetMatchesBySingleTeam(Team teama)
        {
            return Matches.Where(m => m.TeamA.Name == teama.Name || m.TeamB.Name == teama.Name).ToList();
        }

        public List<Match> GetMatchesByTeamNames(Team teama, Team teamb)
        {
           return Matches.Where(m => m.TeamA.Name == teama.Name && m.TeamB.Name == teamb.Name ||
                                m.TeamB.Name == teama.Name && m.TeamA.Name == teamb.Name).ToList();
        }

        public List<Match> GetMatchesPlayedByTeamsInList(List<Team> teamsSubset)
        {
            List<String> teamNames = teamsSubset.Select(m => m.Name).ToList();
            return Matches.Where(m => teamNames.Contains(m.TeamA.Name) && teamNames.Contains(m.TeamB.Name)).ToList();
        }
    }

}
