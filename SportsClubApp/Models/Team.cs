namespace SportsClubApp.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? League { get; set; }

        public ICollection<Rider> Riders { get; set; }
            = new List<Rider>();
    }
}
