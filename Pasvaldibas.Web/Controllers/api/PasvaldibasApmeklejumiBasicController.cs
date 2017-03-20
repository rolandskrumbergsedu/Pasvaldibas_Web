using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class PasvaldibasApmeklejumiBasicController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [System.Web.Http.HttpGet]
        [ResponseType(typeof(PasvaldibaViewModel))]
        public IHttpActionResult GetPasvaldibasApmeklejumiBasic(string id)
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
                        Id = deputats.DeputatsId,
                        Name = deputats.Name,
                        AttendedCount = 0,
                        NotAttendedCount = 0
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