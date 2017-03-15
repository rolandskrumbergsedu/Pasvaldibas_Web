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
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

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

        public ActionResult GetAllDatesWhereNotModa()
        {
            var resultList = new Dictionary<string, List<OccuranceItem>>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

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

        public ActionResult GetAllDatesWhereNotCorrectCount()
        {
            //var resultList = new Dictionary<string, List<OccuranceItem>>();
            var res2 = new List<OccuranceItemViewModel>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

                var deputati = _db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var apmeklejumi = _db.Apmeklejumi
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                    .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                    .Select(x => new
                    {
                        Value = x.Count(),
                        Date = (DateTime)x.Key
                    }).ToList();

                var results = apmeklejumi.Where(x => x.Value != pasvaldiba.DeputatuSkaits).Select(x => new OccuranceItem
                {
                    Value = x.Value.ToString(),
                    Date = x.Date.ToString(CultureInfo.InvariantCulture)
                }).ToList();

                res2.Add(new OccuranceItemViewModel
                {
                    Name = pasvaldiba.Name,
                    Count = pasvaldiba.DeputatuSkaits.ToString(),
                    Status = pasvaldiba.Status,
                    Occurances = results
                });

                //resultList.Add(pasvaldiba.Name + " - " + pasvaldiba.DeputatuSkaits, results);
            }

            return View(res2);
        }

        public ActionResult GetAllDatesWhereDoubleDeputies()
        {
            var resultList = new Dictionary<string, List<DuplicateItem>>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

                var deputati = _db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var sedes = _db.Apmeklejumi
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                    .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                    .Select(x => (DateTime)x.Key).ToList();

                var resultItemList = new List<DuplicateItem>();

                foreach (var sede in sedes)
                {
                    var d = _db.Apmeklejumi
                        .Where(
                            x =>
                                deputati.Contains(x.Deputats.DeputatsId) && x.Datums.Day == sede.Day &&
                                x.Datums.Month == sede.Month && x.Datums.Year == sede.Year)
                        .GroupBy(y => y.Deputats.DeputatsId)
                        .Select(z => new
                        {
                            Value = z.Count(),
                            DeputyId = z
                        }).ToList();

                    foreach (var dd in d)
                    {
                        if (dd.Value > 1)
                        {
                            // Add to results
                            var duplicate = _db.Deputati.FirstOrDefault(x => x.DeputatsId == dd.DeputyId.Key);

                            resultItemList.Add(new DuplicateItem
                            {
                                Date = sede.ToString(CultureInfo.InvariantCulture),
                                Deputy = duplicate?.Name
                            });
                        }
                    }
                }

                resultList.Add(pasvaldiba.Name, resultItemList);
            }

            return View(resultList);
        }

        public ActionResult GetAllDatesWhereNotCorrectOverallCount()
        {
            var pasvaldibas = _db.Pasvaldibas.ToList();
            var uniqueSedes = new Dictionary<string, List<string>>();

            foreach (var pasvaldiba in pasvaldibas)
            {
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

                var deputati = _db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var sedes = _db.Apmeklejumi
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId));

                var datumi = new List<string>();

                foreach (var sede in sedes)
                {
                    var rawDate = $"{sede.Datums.Day}.{sede.Datums.Month}.{sede.Datums.Year}";
                    if (!datumi.Contains(rawDate))
                    {
                        datumi.Add(rawDate);
                    }
                }

                uniqueSedes.Add(pasvaldiba.CodeNr, datumi);
            }

            var results = new List<string>();

            foreach (var uniqueSede in uniqueSedes)
            {
                var pasvaldiba = _db.Pasvaldibas.FirstOrDefault(x => x.CodeNr == uniqueSede.Key);

                results.Add($"{pasvaldiba?.Name}: {uniqueSede.Value.Count} / {pasvaldiba?.ProtokoluSkaits}");
            }

            return View(results);
        }

        public ActionResult GetAll()
        {
            var resultList = new Dictionary<string, List<OccuranceItem>>();

            var pasvaldibas = _db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {
                ViewData[pasvaldiba.Name] = pasvaldiba.Status;

                var deputati = _db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var apmeklejumi = _db.Apmeklejumi
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                    .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                    .Select(x => new
                    {
                        Value = x.Count(),
                        Date = (DateTime)x.Key
                    }).ToList();

                var results = apmeklejumi.Select(x => new OccuranceItem
                {
                    Value = x.Value.ToString(),
                    Date = $"{x.Date.Day}.{x.Date.Month}.{x.Date.Year}"
                }).ToList();

                resultList.Add(pasvaldiba.Name + " - " + pasvaldiba.DeputatuSkaits, results);
            }

            return View(resultList);
        }
    }

    public class DuplicateItem
    {
        public string Date { get; set; }
        public string Deputy { get; set; }
    }

    public class OccuranceItemViewModel
    {
        public string Name { get; set; }
        public string Count { get; set; }
        public string Status { get; set; }
        public List<OccuranceItem> Occurances { get; set; }

    }
}