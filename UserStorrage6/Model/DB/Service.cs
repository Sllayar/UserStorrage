using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public class Service
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Required]
        public string? Key { get; set; }

        [Required]
        public string? Name { get; set; }

        public Status Status { get; set; }

        public string? Author { get; set; }

        public string? Contacts { get; set; }

        public virtual List<User>? Users { get; set; } = new List<User>();
    }
}
