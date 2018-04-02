using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ODataServer.Models
{
    public class Material
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> Dynamics { get; set; }
    }
}
