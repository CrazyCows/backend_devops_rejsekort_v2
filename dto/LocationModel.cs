using backend_devops_rejsekort_v2.dal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_devops_rejsekort_v2.dto
{
    public class Location
    {
        private DateTime createdAt = DateTime.UtcNow;

        [Key]
        public int? Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }

        public override string ToString()
        {
            return $"Location(Id: {Id}, Latitude: {Latitude}, Longitude: {Longitude}, CreatedAt: {CreatedAt})";
        }
    }

    public class LocationPair
    {
        [Key]
        public int Id { get; set; } // Non-nullable

        [Required]
        public Location SignInLocation { get; set; }

        public Location? SignOutLocation { get; set; }

        public DateTime SignInTime { get; set; } = DateTime.UtcNow;
        public DateTime? SignOutTime { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public override string ToString()
        {
            return $"LocationPair(Id: {Id}, SignInLocation: {SignInLocation}, SignOutLocation: {SignOutLocation}, " +
                   $"SignInTime: {SignInTime}, SignOutTime: {SignOutTime}, UserId: {UserId})";
        }
    }
}
