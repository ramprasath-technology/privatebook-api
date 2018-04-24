using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data
{
    // Class for stock
    public partial class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockMappingId { get; set; }      
        [MaxLength(10)]
        public string StockSymbol { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
