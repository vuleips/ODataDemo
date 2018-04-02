using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ODataServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace ODataServer.Controllers
{
    public class MaterialsController : ODataController
    {
        const string SessionKeyName = "_Materials";
        List<Material> DefaultMaterials = new List<Material>
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

        private List<Material> GetMaterials()
        {
            var materials = HttpContext.Session.Get<List<Material>>(SessionKeyName);
            if (materials == null)
            {
                materials = DefaultMaterials;
            }

            return materials;
        }

        [EnableQuery]
        public IQueryable<Material> Get()
        {
            return GetMaterials().AsQueryable();
        }

        public IActionResult Get(string key)
        {
            Material material = GetMaterials().FirstOrDefault(e => e.Id == key);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        public IActionResult GetName(string key)
        {
            Material material = GetMaterials().FirstOrDefault(e => e.Id == key);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material.Name);
        }

        public ActionResult GetDynamicProperty(string key, string dynamicProperty)
        {
            Material material = GetMaterials().FirstOrDefault(c => c.Id == key);
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
            var materials = GetMaterials();
            materials.Add(material);
            HttpContext.Session.Set<List<Material>>(SessionKeyName, materials);
            return Created(material);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
}
