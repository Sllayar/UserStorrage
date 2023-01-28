using System.ComponentModel.DataAnnotations;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Requests.Short
{
    public class ServiceShort
    {
        [Required]
        public string? Key { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public Status Status { get; set; }

        public string? Author { get; set; }

        public string? Contacts { get; set; }

        [Required]
        public List<UserShort> Users { get; set; }
    }
}
