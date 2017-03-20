using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class DeputatiController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

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
                Apmeklejumi2016 = new List<ApmeklejumsItem>(),
                NotAttendedCountReasons = new Dictionary<string, int>()
            };

            foreach (var apmeklejums in apmeklejumi)
            {
                var item = new ApmeklejumsItem
                {
                    Date = $"{apmeklejums.Datums.Day.ToString().PadLeft(2, '0')}.{apmeklejums.Datums.Month.ToString().PadLeft(2, '0')}",
                    Attended = apmeklejums.Apmekleja ? "1" : "0",
                    Reason = apmeklejums.Apmekleja ? "Ieradās" : !string.IsNullOrEmpty(apmeklejums.NeapmeklesanasIemesls) ? apmeklejums.NeapmeklesanasIemesls : "Nav zināms"
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

                string iemesls;

                if (apmeklejums.Apmekleja)
                {
                    iemesls = "Ieradās";
                }
                else
                {
                    iemesls = apmeklejums.NeapmeklesanasIemesls.Length > 3
                    ? apmeklejums.NeapmeklesanasIemesls
                    : "Nav zināms";
                };

                if (result.NotAttendedCountReasons.ContainsKey(iemesls))
                {
                    result.NotAttendedCountReasons[iemesls] =
                        result.NotAttendedCountReasons[iemesls] + 1;
                }
                else
                {
                    result.NotAttendedCountReasons.Add(iemesls, 1);
                }
            }

            return Ok(result);
        }
    }
}
