using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace ODataServer.Controllers
{
    public class MaterialsController : ODataController
    {
        private List<Material> _materials = new List<Material>
        {
            new Material
            {
                Id = "AB12",
                Name = "Material AB"
            },
            new Material
            {
                Id = "CD34",
                Name = "Material CD",
                Dynamics = new Dictionary<string, object>
                {
                    { "Hardness", 50 }
                }
            }
        };

        [EnableQuery]
        public IQueryable<Material> Get()
        {
            return _materials.AsQueryable();
        }

        public IActionResult Get(string key)
        {
            Material material = _materials.FirstOrDefault(e => e.Id == key);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        public IActionResult GetName(string key)
        {
            Material material = _materials.FirstOrDefault(e => e.Id == key);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material.Name);
        }

        public ActionResult GetDynamicProperty(string key, string dynamicProperty)
        {
            Material material = _materials.FirstOrDefault(c => c.Id == key);
            if (material == null || !material.Dynamics.Keys.Contains(dynamicProperty))
            {
                return NotFound();
            }

            return Ok(material.Dynamics[dynamicProperty].ToString());
        }

        public IActionResult Post([FromBody]Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // For this sample, we aren't enforcing unique keys.
            _materials.Add(material);
            return Created(material);
        }
    }
}
