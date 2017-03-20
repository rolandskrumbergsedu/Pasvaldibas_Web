using System.Collections.Generic;

namespace Pasvaldibas.Web.Controllers.api
{
    public class DeputyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AttendedCount { get; set; }
        public int NotAttendedCount { get; set; }
        public Dictionary<string, int> NotAttendedCountReasons { get; set; }
        public int AllCount { get; set; }
    }
}