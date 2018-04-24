using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data
{
    // Class for user-feature mapping
    public class UserFeatureMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MappingId { get; set; }

        public int FeatureId { get; set; }
        public Feature Feature { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
