using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class DeputatiController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        [ResponseType(typeof(ApmeklejumsViewModel))]
        public IHttpActionResult GetById(string id)
        {
            var idParsed = int.Parse(id);

            var apmeklejumi = _db.Apmeklejumi.Where(x => x.Deputats.DeputatsId == idParsed).OrderBy(x => x.Datums);
            var deputats = _db.Deputati.Include("Pasvaldiba").FirstOrDefault(x => x.DeputatsId == idParsed);

            if (deputats == null) return BadRequest();

            var result = new ApmeklejumsViewModel
            {
                Name = deputats.Name,
                Municipality = deputats.Pasvaldiba.Name,
                Apmeklejumi2013 = new List<ApmeklejumsItem>(),
                Apmeklejumi2014 = new List<ApmeklejumsItem>(),
                Apmeklejumi2015 = new List<ApmeklejumsItem>(),
                Apmeklejumi2016 = new List<ApmeklejumsItem>()
            };

            foreach (var apmeklejums in apmeklejumi)
            {
                var item = new ApmeklejumsItem
                {
                    Date = $"{apmeklejums.Datums.Day}.{apmeklejums.Datums.Month}.{apmeklejums.Datums.Year}",
                    Attended = apmeklejums.Apmekleja ? "1" : "0"
                };

                if (apmeklejums.Datums.Year == 2013)
                {
                    result.Apmeklejumi2013.Add(item);
                }

                if (apmeklejums.Datums.Year == 2014)
                {
                    result.Apmeklejumi2014.Add(item);
                }

                if (apmeklejums.Datums.Year == 2015)
                {
                    result.Apmeklejumi2015.Add(item);
                }

                if (apmeklejums.Datums.Year == 2016)
                {
                    result.Apmeklejumi2016.Add(item);
                }

            }

            return Ok(result);
        }
    }
}
