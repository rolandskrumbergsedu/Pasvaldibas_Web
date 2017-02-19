using System.Collections.Generic;

namespace Pasvaldibas.Web.Controllers.api
{
    public class PasvaldibaViewModel
    {
        public string PasvaldibaName { get; set; }

        public List<DeputyViewModel> Deputies { get; set; }
    }
}