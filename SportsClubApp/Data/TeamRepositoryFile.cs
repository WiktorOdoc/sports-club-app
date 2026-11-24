
using System.Text.Json;
using SportsClubApp.Interfaces;
using SportsClubApp.Models;

namespace SportsClubApp.Data
{
    public class TeamRepositoryFile : ITeamRepository
    {
        private readonly string _filePath;
        private readonly string _ridersFilePath;

        private static readonly SemaphoreSlim _lock = new(1, 1);

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public TeamRepositoryFile(IWebHostEnvironment environment)
        {
            string directory = Path.Combine(
                environment.ContentRootPath,
                "DataFiles"
            );

            Directory.CreateDirectory(directory);

            _filePath = Path.Combine(directory, "teams.json");
            _ridersFilePath = Path.Combine(directory, "riders.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            if (!File.Exists(_ridersFilePath))
            {
                File.WriteAllText(_ridersFilePath, "[]");
            }
        }

        private async Task<List<Team>> ReadUnlockedAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);

            return JsonSerializer.Deserialize<List<Team>>(
                json,
                _jsonOptions
            ) ?? new List<Team>();
        }

        private async Task WriteUnlockedAsync(List<Team> teams)
        {
            string json = JsonSerializer.Serialize(
                teams,
                _jsonOptions
            );

            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<Team>> GetAllAsync()
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                string ridersJson = await File.ReadAllTextAsync(
                    _ridersFilePath
                );

                var riders = JsonSerializer.Deserialize<List<Rider>>(
                    ridersJson,
                    _jsonOptions
                ) ?? new List<Rider>();

                foreach (var team in teams)
                {
                    team.Riders = riders
                        .Where(r => r.TeamId == team.Id)
                        .ToList();
                }

                return teams;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                return teams.FirstOrDefault(t => t.Id == id);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AddAsync(Team team)
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                team.Id = teams.Count == 0
                    ? 1
                    : teams.Max(t => t.Id) + 1;

                teams.Add(team);

                await WriteUnlockedAsync(teams);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task UpdateAsync(Team team)
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                var existingTeam = teams.FirstOrDefault(
                    t => t.Id == team.Id
                );

                if (existingTeam == null)
                    return;

                existingTeam.Name = team.Name;
                existingTeam.City = team.City;
                existingTeam.League = team.League;

                await WriteUnlockedAsync(teams);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                var team = teams.FirstOrDefault(t => t.Id == id);

                if (team == null)
                    return;

                string ridersJson = await File.ReadAllTextAsync(
                    _ridersFilePath
                );

                var riders = JsonSerializer.Deserialize<List<Rider>>(
                    ridersJson,
                    _jsonOptions
                ) ?? new List<Rider>();

                foreach (var rider in riders)
                {
                    if (rider.TeamId == id)
                    {
                        rider.TeamId = null;
                    }
                }

                string updatedRidersJson = JsonSerializer.Serialize(
                    riders,
                    _jsonOptions
                );

                await File.WriteAllTextAsync(
                    _ridersFilePath,
                    updatedRidersJson
                );

                teams.Remove(team);

                await WriteUnlockedAsync(teams);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await _lock.WaitAsync();

            try
            {
                var teams = await ReadUnlockedAsync();

                return teams.Any(t => t.Id == id);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
