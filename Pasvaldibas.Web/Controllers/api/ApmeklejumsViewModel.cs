using System.Collections.Generic;

namespace Pasvaldibas.Web.Controllers.api
{
    public class ApmeklejumsViewModel
    {
        public string Name { get; set; }
        public string Municipality { get; set; }

        public List<ApmeklejumsItem> Apmeklejumi2013 { get; set; }
        public List<ApmeklejumsItem> Apmeklejumi2014 { get; set; }
        public List<ApmeklejumsItem> Apmeklejumi2015 { get; set; }
        public List<ApmeklejumsItem> Apmeklejumi2016 { get; set; }

        public Dictionary<string, int> NotAttendedCountReasons { get; set; }
    }

    public class ApmeklejumsItem
    {
        public string Date { get; set; }
        public string Attended { get; set; }
        public string Reason { get; set; }
    }
}