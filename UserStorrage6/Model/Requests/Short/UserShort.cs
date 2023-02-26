using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Requests.Short
{
    public class UserShort
    {
        [Required]
        public string SysId { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public string SysLogin { get; set; }

        public string? OwnerLogin { get; set; }

        public DB.Type Type { get; set; }

        [Required]
        public string ServiceKey { get; set; }

        public string? Comment { get; set; }

        public string[] Roles { get; set; } = new string[0];

        public string[] Permissions { get; set; } = new string[0];
    }
}
