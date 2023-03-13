using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public class Role
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
        public virtual Service? Service { get; set; }

        public string? Comment { get; set; }

        public DateTime CreateAT { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public DateTime SyncAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime PartSyncAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();


        [IgnoreDataMember]
        public virtual List<User>? Users { get; set; } = new List<User>();

        public static implicit operator List<object>(Role v)
        {
            throw new NotImplementedException();
        }
    }
}
