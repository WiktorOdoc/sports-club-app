
using System.Text.Json;
using SportsClubApp.Interfaces;
using SportsClubApp.Models;

namespace SportsClubApp.Data
{
    public class RiderRepositoryFile : IRiderRepository
    {
        private readonly string _filePath;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public RiderRepositoryFile(IWebHostEnvironment environment)
        {
            string directory = Path.Combine(
                environment.ContentRootPath,
                "DataFiles"
            );

            Directory.CreateDirectory(directory);

            _filePath = Path.Combine(directory, "riders.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        private async Task<List<Rider>> ReadAsync()
        {
            await _lock.WaitAsync();

            try
            {
                string json = await File.ReadAllTextAsync(_filePath);

                return JsonSerializer.Deserialize<List<Rider>>(
                    json, _options
                ) ?? new List<Rider>();
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task WriteAsync(List<Rider> riders)
        {
            await _lock.WaitAsync();

            try
            {
                string json = JsonSerializer.Serialize(riders, _options);

                await File.WriteAllTextAsync(_filePath, json);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<List<Rider>> GetAllAsync()
        {
            return await ReadAsync();
        }

        public async Task<Rider?> GetByIdAsync(int id)
        {
            var riders = await ReadAsync();

            return riders.FirstOrDefault(r => r.Id == id);
        }

        public async Task AddAsync(Rider rider)
        {
            var riders = await ReadAsync();

            rider.Id = riders.Count == 0
                ? 1
                : riders.Max(r => r.Id) + 1;

            riders.Add(rider);

            await WriteAsync(riders);
        }

        public async Task UpdateAsync(Rider rider)
        {
            var riders = await ReadAsync();

            var existing = riders.FirstOrDefault(
                r => r.Id == rider.Id
            );

            if (existing == null)
                return;

            existing.FirstName = rider.FirstName;
            existing.LastName = rider.LastName;
            existing.BirthYear = rider.BirthYear;
            existing.RacingNumber = rider.RacingNumber;
            existing.TeamId = rider.TeamId;

            await WriteAsync(riders);
        }

        public async Task DeleteAsync(int id)
        {
            var riders = await ReadAsync();

            var rider = riders.FirstOrDefault(r => r.Id == id);

            if (rider != null)
            {
                riders.Remove(rider);

                await WriteAsync(riders);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var riders = await ReadAsync();

            return riders.Any(r => r.Id == id);
        }
    }
}
