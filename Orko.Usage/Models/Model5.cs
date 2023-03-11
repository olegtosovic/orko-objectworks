using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orko.Usage.Models
{
    /// <summary>
    /// Test model with 5 properties.
    /// </summary>
    public class Model5
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? ChangedDate { get; set; }
    }
}
