using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class PasvaldibasApmeklejumiController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        [ResponseType(typeof(PasvaldibaViewModel))]
        public IHttpActionResult GetPasvaldibasApmeklejumi(string id)
        {
            var municipality = _db.Pasvaldibas.Include("Deputati").FirstOrDefault(x => x.Code == id);

            if (municipality != null)
            {
                var result = new PasvaldibaViewModel
                {
                    PasvaldibaName = municipality.Name,
                    PasvaldibaCode = municipality.Code,
                    Deputies = new List<DeputyViewModel>()
                };

                foreach (var deputats in municipality.Deputati)
                {
                    var deputyModel = new DeputyViewModel
                    {
                        Name = deputats.Name,
                        AttendedCount = 0,
                        NotAttendedCount = 0,
                        NotAttendedCountReasons = new Dictionary<string, int>()
                    };

                    _db.Entry(deputats).Collection(x => x.ApmekletasSedes).Load();

                    foreach (var apmekletaSede in deputats.ApmekletasSedes)
                    {
                        if (apmekletaSede.Apmekleja)
                        {
                            deputyModel.AttendedCount++;
                        }
                        else
                        {
                            deputyModel.NotAttendedCount++;
                            if (deputyModel.NotAttendedCountReasons.ContainsKey(apmekletaSede.NeapmeklesanasIemesls))
                            {
                                deputyModel.NotAttendedCountReasons[apmekletaSede.NeapmeklesanasIemesls] =
                                    deputyModel.NotAttendedCountReasons[apmekletaSede.NeapmeklesanasIemesls] + 1;
                            }
                            else
                            {
                                deputyModel.NotAttendedCountReasons.Add(apmekletaSede.NeapmeklesanasIemesls, 1);
                            }
                        }
                    }

                    deputyModel.AllCount = deputyModel.AttendedCount + deputyModel.NotAttendedCount;

                    result.Deputies.Add(deputyModel);
                }

                return Ok(result);
            };

            return null;
        }
    }
}