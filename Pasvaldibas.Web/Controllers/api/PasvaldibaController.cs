using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class PasvaldibaController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        public IQueryable<MunicipalityItem> GetAll()
        {
            return _db.Pasvaldibas.Select(x => new MunicipalityItem
            {
                Name = x.Name,
                Code = x.Code,
                Latitude = x.Latitude,
                Longtitude = x.Longtitude
            });
        }
    }
}