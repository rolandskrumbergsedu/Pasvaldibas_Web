using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Overview
{
    class Program
    {
        static readonly ApplicationDbContext db = new ApplicationDbContext();

        static void Main(string[] args)
        {
            var pasvaldibas = db.Pasvaldibas.ToList();

            var pasvaldibasOverallCount = new Dictionary<string, int>();
            var pasvaldibasAverageCount = new Dictionary<string, double>();

            foreach (var pasvaldiba in pasvaldibas)
            {
                var deputaties = db.Deputati.Where(x => x.Pasvaldiba.CodeNr == pasvaldiba.CodeNr).ToList().Select(x => x.DeputatsId);

                var apmeklejumi = db.Apmeklejumi
                    .Where(x => deputaties.Contains(x.Deputats.DeputatsId));

                //no 2013.g. jūlija - 2016.g. oktobrim? Vizualizācijās 
                var sakumaDatums = new DateTime(2013, 6, 30);
                var beiguDatums = new DateTime(2016, 11, 1);

                var apmeklejumiGouped = apmeklejumi
                    .Where(x => x.Datums > sakumaDatums && x.Datums < beiguDatums)
                    .GroupBy(x => DbFunctions.TruncateTime(x.Datums))
                    .Select(x => new
                    {
                        Value = x.Count(),
                        Date = (DateTime)x.Key
                    }).ToList();

                var doubleDates = 0;

                foreach (var a in apmeklejumiGouped)
                {
                    if (a.Value > pasvaldiba.DeputatuSkaits + 3)
                    {
                        doubleDates++;
                    }
                }

                pasvaldibasOverallCount.Add(pasvaldiba.CodeNr, apmeklejumiGouped.Count + doubleDates);

                var apmekletasSedes = apmeklejumi.Count();
                var neapmekletasSedes = apmeklejumi.Count(x => x.Apmekleja);
                var videjais = (double)neapmekletasSedes / apmekletasSedes;
                pasvaldibasAverageCount.Add(pasvaldiba.CodeNr, videjais);
            }
            
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            var xlWb = (Microsoft.Office.Interop.Excel._Workbook)(xlApp.Workbooks.Add(""));
            var xlSheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWb.ActiveSheet;

            xlSheet.Cells[1, 1] = "Pašvaldība";
            xlSheet.Cells[1, 2] = "Vārds, uzvārds";
            xlSheet.Cells[1, 3] = "Pašvaldības sēžu skaits";
            xlSheet.Cells[1, 4] = "Apmeklējamo sēžu skaits";
            xlSheet.Cells[1, 5] = "Apmeklēto sēžu skaits";
            xlSheet.Cells[1, 6] = "Neapmeklēto sēžu skaits";
            xlSheet.Cells[1, 7] = "Procentuālais apmeklējums";
            xlSheet.Cells[1, 8] = "Procentuālais apmeklējums pašvaldībā";
            xlSheet.Cells[1, 9] = "Iemesli";

            var deputati = db.Deputati.Include("ApmekletasSedes").Include("Pasvaldiba").ToList();
            var i = 2;
            foreach (var deputats in deputati)
            {
                var pasvaldiba = deputats.Pasvaldiba.Name;
                var vards = deputats.Name;
                var kopejaisSezuSkaits = pasvaldibasOverallCount[deputats.Pasvaldiba.CodeNr];
                var apmeklejamoSezuSkaits = deputats.ApmekletasSedes.Count;
                var apmekletoSezuSkaits = deputats.ApmekletasSedes.Count(x => x.Apmekleja);
                var neapmekletoSezuSkaits = deputats.ApmekletasSedes.Count(x => !x.Apmekleja);
                var videjaisPasvaldiba = pasvaldibasAverageCount[deputats.Pasvaldiba.CodeNr];

                double procenti;

                if (apmeklejamoSezuSkaits != 0)
                {
                    procenti = (double) apmekletoSezuSkaits/apmeklejamoSezuSkaits;
                }
                else
                {
                    procenti = 0;
                }

                var iemesli = string.Empty;

                foreach (var apmekletasSede in deputats.ApmekletasSedes)
                {
                    if (!apmekletasSede.Apmekleja && apmekletasSede.NeapmeklesanasIemesls.Trim().Length > 3)
                    {
                        iemesli = iemesli + apmekletasSede.NeapmeklesanasIemesls.Trim()  + "; ";
                    }
                }

                xlSheet.Cells[i, 1] = pasvaldiba;
                xlSheet.Cells[i, 2] = vards;
                xlSheet.Cells[i, 3] = kopejaisSezuSkaits;
                xlSheet.Cells[i, 4] = apmeklejamoSezuSkaits;
                xlSheet.Cells[i, 5] = apmekletoSezuSkaits;
                xlSheet.Cells[i, 6] = neapmekletoSezuSkaits;
                xlSheet.Cells[i, 7] = procenti;
                xlSheet.Cells[i, 8] = videjaisPasvaldiba;
                xlSheet.Cells[i, 9] = iemesli;

                i++;
            }

            xlWb.SaveAs(@"C:\Work_misc\Protokoli\Parskats\Parskats.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
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
