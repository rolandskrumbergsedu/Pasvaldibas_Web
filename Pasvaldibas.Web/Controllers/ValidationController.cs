using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Pasvaldibas.Web.Controllers.api;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers
{
    public class ValidationController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult GetAllEmpty()
        {
            var resultList = new Dictionary<string, List<string>>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                var possibleDates = new List<string>();

                for (var i = 1; i < 13; i++)
                {
                    for (var j = 2013; j < 2017; j++)
                    {
                        if (!(j == 2013 && i >= 1 && i <= 6))
                        {
                            possibleDates.Add($"{i}.{j}");
                        }
                    }
                }

                var deputati = _db.Deputati.Include("ApmekletasSedes").Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList();

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
                resultList.Add(pasvaldiba.Name, result);
            }

            return View(resultList);
        }

        public ActionResult GetEmptyMonthYearByMunicipality(string id)
        {
            var pasvaldiba = _db.Pasvaldibas.FirstOrDefault(x => x.CodeNr == id);

            ViewBag.Pasvaldiba = pasvaldiba?.Name;

            var possibleDates = new List<string>();

            for (var i = 1; i < 13; i++)
            {
                for (var j = 2013; j < 2017; j++)
                {
                    if (!(j == 2013 && i >= 1 && i <= 6))
                    {
                        possibleDates.Add($"{i}.{j}");
                    }
                }
            }

            var deputati = _db.Deputati.Include("ApmekletasSedes").Where(x => x.Pasvaldiba.CodeNr == id);

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

            return View(result);
        }

        public ActionResult GetDatesWhereNotModa(string id)
        {
            var pasvaldiba = _db.Pasvaldibas.FirstOrDefault(x => x.CodeNr == id);

            ViewBag.Pasvaldiba = pasvaldiba?.Name;

            var deputati = _db.Deputati.Where(x => x.Pasvaldiba.Code == id).ToList().Select(x => x.DeputatsId);

            var apmeklejumi = _db.Apmeklejumi
                .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                .Select(x => new
                {
                    Value = x.Count(),
                    Date = (DateTime)x.Key
                }).ToList();

            var o = apmeklejumi.GroupBy(x => x.Value)
                .OrderByDescending(y => y.Key)
                .First();

            var firstOrDefault = apmeklejumi.OrderByDescending(x => x.Value).FirstOrDefault();
            if (firstOrDefault != null)
            {
                var maxOccurance = firstOrDefault.Value;

                ViewBag.Moda = maxOccurance;

                var results = apmeklejumi.Where(x => x.Value != maxOccurance).Select(x => new OccuranceItem
                {
                    Value = x.Value.ToString(),
                    Date = x.Date.ToString(CultureInfo.InvariantCulture)
                }).ToList();

                return View(results);
            }

            return View(new List<OccuranceItem>());
        }

        public ActionResult GetAllDatesWhereNotModa()
        {
            var resultList = new Dictionary<string, List<OccuranceItem>>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                var deputati = _db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var apmeklejumi = _db.Apmeklejumi
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                    .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                    .Select(x => new
                    {
                        Value = x.Count(),
                        Date = (DateTime)x.Key
                    }).ToList();

                var t = new Dictionary<int, int>();
                foreach (var apmeklejums in apmeklejumi)
                {
                    if (t.ContainsKey(apmeklejums.Value))
                    {
                        t[apmeklejums.Value] = t[apmeklejums.Value] + 1;
                    }
                    else
                    {
                        t.Add(apmeklejums.Value, 1);
                    }
                }

                var maxValue = t.Max(x => x.Value);
                var maxOccurance = t.FirstOrDefault(x => x.Value == maxValue).Key;

                ViewBag.Moda = maxOccurance;

                var results = apmeklejumi.Where(x => x.Value != maxOccurance).Select(x => new OccuranceItem
                {
                    Value = x.Value.ToString(),
                    Date = x.Date.ToString(CultureInfo.InvariantCulture)
                }).ToList();

                resultList.Add(pasvaldiba.Name + " - " + maxOccurance, results);
            }

            return View(resultList);
        }

        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("api/validationattendancealldate/{id}")]
        //public IQueryable<DeputyItem> GetDeputyAttendencesAllDates(string id)
        //{
        //    var deputati = _db.Deputati.Where(x => x.Pasvaldiba.Code == id);
        //    var deputatuId = deputati.ToList().Select(x => x.DeputatsId);

        //    var apmeklejumi = _db.Apmeklejumi
        //        .Where(x => deputatuId.Contains(x.Deputats.DeputatsId))
        //        .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
        //        .Select(x => new
        //        {
        //            Date = (DateTime)x.Key
        //        });

        //    var results = new List<DeputyItem>();

        //    foreach (var deputats in deputati)
        //    {
        //        var resultItem = new DeputyItem
        //        {
        //            Name = deputats.Name,
        //            Attendances = new List<AttendanceItem>()
        //        };

        //        foreach (var sede in apmeklejumi)
        //        {
        //            var attendedItem = new AttendanceItem
        //            {
        //                Date = sede.Date.ToString()
        //            };

        //            var s = deputats.ApmekletasSedes.FirstOrDefault(x => x.Datums.Year == sede.Date.Year &&
        //                            x.Datums.Month == sede.Date.Month &&
        //                            x.Datums.Day == sede.Date.Day);

        //            if (s == null)
        //            {
        //                attendedItem.Attended = null;
        //            }
        //            else
        //            {
        //                attendedItem.Attended = s.Apmekleja;
        //            }

        //        }

        //        results.Add(resultItem);

        //    }

        //    return results.AsQueryable();
        //}
    }
}