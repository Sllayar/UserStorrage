using System.ComponentModel.DataAnnotations.Schema;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Short
{
    public class UserShort
    {
        public Status IsActive { get; set; }

        public string? SysLogin { get; set; }

        public string? DomainLogin { get; set; }

        public DB.Type Type { get; set; }

        public string? ServiceKey { get; set; }

        public string? Comment { get; set; }
    }
}
