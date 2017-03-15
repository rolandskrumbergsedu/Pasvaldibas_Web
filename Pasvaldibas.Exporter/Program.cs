using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Exporter
{
    class Program
    {
        static readonly ApplicationDbContext db = new ApplicationDbContext();

        static void Main(string[] args)
        {
            var pasvaldibas = db.Pasvaldibas.ToList();

            foreach (var pasvaldiba in pasvaldibas)
            {

                var xlApp = new Microsoft.Office.Interop.Excel.Application();

                var xlWb = (Microsoft.Office.Interop.Excel._Workbook)(xlApp.Workbooks.Add(""));
                var xlSheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWb.ActiveSheet;

                xlSheet.Cells[1, 1] = "Vārds, uzvārds";
                xlSheet.Cells[1, 2] = "Datums";
                xlSheet.Cells[1, 3] = "Apmeklējums";
                xlSheet.Cells[1, 4] = "Iemesls";


                var deputati = db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);
                var apmeklejumi = db.Apmeklejumi.Include("Deputats")
                    .Where(x => deputati.Contains(x.Deputats.DeputatsId))
                    .OrderBy(x => x.Datums)
                    .ThenBy(x => x.Deputats.Name).ToList();

                for (var i = 0; i < apmeklejumi.Count; i++)
                {
                    xlSheet.Cells[i + 2, 1] = apmeklejumi[i].Deputats.Name;
                    xlSheet.Cells[i + 2, 2] = apmeklejumi[i].Datums;
                    xlSheet.Cells[i + 2, 3] = apmeklejumi[i].Apmekleja ? "1" : "0";
                    xlSheet.Cells[i + 2, 4] = !string.IsNullOrEmpty(apmeklejumi[i].NeapmeklesanasIemesls) ? apmeklejumi[i].NeapmeklesanasIemesls : string.Empty;
                }


                xlWb.SaveAs(@"C:\Work_misc\Protokoli\Gatavie\Datubaze\" + pasvaldiba.Code + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
            false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                xlWb.Close();

                //quit and release
                xlApp.Quit();

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
