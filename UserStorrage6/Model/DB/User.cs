﻿using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{

    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? SysId { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public string? SysLogin { get; set; }

        public string? OwnerLogin { get; set; }

        [JsonIgnore]
        [Required]
        public virtual Service? Service { get; set; }

        [Required]
        public Type Type { get; set; }

        public string? Comment { get; set; }

        public DateTime CreateAT { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public DateTime SyncAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime PartSyncAt { get; set; } = DateTime.UtcNow;

        public virtual List<Role> Roles { get; set; } = new List<Role>();

        public virtual List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
