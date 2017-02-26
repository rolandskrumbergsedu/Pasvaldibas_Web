using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers.api
{
    public class ValidationController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/validationempty/{id}")]
        public IQueryable<string> GetEmptyMonthYearByMunicipality(string id)
        {
            var possibleDates = new List<string>();

            for (var i = 1; i < 13; i++)
            {
                for (var j = 2013; j < 2017; j++)
                {
                    possibleDates.Add($"{i}.{j}");
                }
            }

            var deputati = _db.Deputati.Where(x => x.Pasvaldiba.Code == id);

            var datumi = new List<string>();

            foreach (var deputats in deputati)
            {
                foreach (var apmekletasSedes in deputats.ApmekletasSedes)
                {
                    var date = $"{apmekletasSedes.Datums.Month}.{apmekletasSedes.Datums.Year}";
                    if (!datumi.Contains(date))
                    {
                        datumi.Add(date);
                    }
                }
            }

            var result = new List<string>();

            foreach (var possibleDate in possibleDates)
            {
                if (!datumi.Contains(possibleDate))
                {
                    result.Add(possibleDate);
                }
            }

            return result.AsQueryable();
        }

        [HttpGet]
        [Route("api/validationnotequal/{id}")]
        public IQueryable<OccuranceItem> GetDatesWhereNotModa(string id)
        {
            var deputati = _db.Deputati.Where(x => x.Pasvaldiba.Code == id).ToList().Select(x => x.DeputatsId);
            
            var apmeklejumi = _db.Apmeklejumi
                .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                .Select(x => new
            {
                Value = x.Count(),
                Date = (DateTime)x.Key
            });

            var firstOrDefault = apmeklejumi.OrderByDescending(x => x.Value).FirstOrDefault();
            if (firstOrDefault != null)
            {
                var maxOccurance = firstOrDefault.Value;

                return apmeklejumi.Where(x => x.Value != maxOccurance).Select(x => new OccuranceItem
                {
                    Value = x.Value.ToString(),
                    Date = x.Date.ToString(CultureInfo.InvariantCulture)
                });
            }

            return null;
        }

        [HttpGet]
        [Route("api/validationattendancealldate/{id}")]
        public IQueryable<DeputyItem> GetDeputyAttendencesAllDates(string id)
        {
            var deputati = _db.Deputati.Where(x => x.Pasvaldiba.Code == id);
            var deputatuId = deputati.ToList().Select(x => x.DeputatsId);

            var apmeklejumi = _db.Apmeklejumi
                .Where(x => deputatuId.Contains(x.Deputats.DeputatsId))
                .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                .Select(x => new
                {
                    Date = (DateTime)x.Key
                });

            var results = new List<DeputyItem>();

            foreach (var deputats in deputati)
            {
                var resultItem = new DeputyItem
                {
                    Name = deputats.Name,
                    Attendances = new List<AttendanceItem>()
                };

                foreach (var sede in apmeklejumi)
                {
                    var attendedItem = new AttendanceItem
                    {
                        Date = sede.Date.ToString()
                    };

                    var s = deputats.ApmekletasSedes.FirstOrDefault(x => x.Datums.Year == sede.Date.Year &&
                                    x.Datums.Month == sede.Date.Month &&
                                    x.Datums.Day == sede.Date.Day);

                    if (s == null)
                    {
                        attendedItem.Attended = null;
                    }
                    else
                    {
                        attendedItem.Attended = s.Apmekleja;
                    }
                    
                }

                results.Add(resultItem);

            }

            return results.AsQueryable();
        }
    }

    public class OccuranceItem
    {
        public string Value { get; set; }
        public string Date { get; set; }
    }

    public class DeputyItem
    {
        public string Name { get; set; }
        
        public List<AttendanceItem> Attendances { get; set; }
    }

    public class AttendanceItem
    {
        public string Date { get; set; }
        public bool? Attended { get; set; }
    }
}
