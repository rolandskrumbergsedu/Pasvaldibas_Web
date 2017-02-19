﻿using System.Collections.Generic;

namespace Pasvaldibas.Web.Controllers.api
{
    public class DeputyViewModel
    {
        public string Name { get; set; }

        public int AttendedCount { get; set; }

        public int NotAttendedCount { get; set; }

        public Dictionary<string, int> NotAttendedCountReasons { get; set; }
    }
}