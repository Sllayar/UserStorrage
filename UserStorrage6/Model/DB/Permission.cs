using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Key]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public bool? IsNeedAprove { get; set; }

        [Required]
        [Key]
        public virtual Service? Service { get; set; }

        public string? Comment { get; set; }

        public DateTime CreateAT { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public virtual List<User>? Users { get; set; }
    }

    public class PermissionRequest
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Status Status { get; set; }

        public bool? IsNeedAprove { get; set; }

        public virtual string? ServiceKey { get; set; }

        public string? Comment { get; set; }
    }
}
