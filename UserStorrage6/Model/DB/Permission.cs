using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string SysId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public bool? IsNeedAprove { get; set; }

        [Required]
        [IgnoreDataMember]
        public virtual Service Service { get; set; }


        [IgnoreDataMember]
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public string? Comment { get; set; }

        public DateTime CreateAT { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public DateTime SyncAt { get; set; } = DateTime.UtcNow;


        [IgnoreDataMember]
        public virtual List<User>? Users { get; set; }
    }


}
