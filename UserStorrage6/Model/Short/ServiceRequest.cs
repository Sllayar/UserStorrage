using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Short
{
    public class ServiceRequest
    {
        public string? Key { get; set; }

        public string? Name { get; set; }

        public Status Status { get; set; }

        public string? Author { get; set; }

        public string? Contacts { get; set; }

        public List<UserShort> Users { get; set; }
    }
}
