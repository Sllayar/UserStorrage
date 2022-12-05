using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public bool? IsNeedAprove { get; set; }

        [Required]
        public virtual Service? Service { get; set; }

        public string? Comment { get; set; }

        public DateTime CreateAT { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public virtual List<Permission>? SysPermitions { get; set; }
    }

    public class RoleRequest
    {
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public bool? IsNeedAprove { get; set; }

        [Required]
        public virtual string? ServiceKey { get; set; }

        public string? Comment { get; set; }
    }
}
