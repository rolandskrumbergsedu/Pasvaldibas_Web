using System;
using System.Collections.Generic;
using System.Linq;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Converter
{
    class Program
    {
        private const string MuniName = "Alsunga";
        private const string MuniCode = "16";

        private const int ApmeklejumaNrColNr = 1;
        private const int DeputaVardaColNr = 3;
        private const int DateColNr = 4;
        private const int ApmeklejumaColNr = 5;
        private const int NeapmeklesanasIemeslaColNr = 6;

        static readonly ApplicationDbContext db = new ApplicationDbContext();


        static void Main(string[] args)
        {
            var resultFile = $"C:\\Work_misc\\Protokoli\\Rezultati\\{MuniName}.xlsx";

            var xlApp = new Microsoft.Office.Interop.Excel.Application();
            var xlWorkbook = xlApp.Workbooks.Open(resultFile);
            var xlWorksheet = xlWorkbook.Sheets[1];
            var xlRange = xlWorksheet.UsedRange;

            var rowCount = xlRange.Rows.Count;

            // TO DO: Izveido jaunu pasvaldibu un iegūst atsauci uz to
            CreateMunicipality(MuniName, MuniCode);

            var tmp = new List<Apmeklejums>();

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (var i = 2; i <= rowCount; i++)
            {
                var apmeklejums = new Apmeklejums();
                Deputats deputats = null;
                var date = DateTime.MinValue;
                var attended = false;
                var apmeklejumaNr = string.Empty;
                var neapmeklesanasIemesls = string.Empty;

                if (xlRange.Cells[i, DeputaVardaColNr] != null && xlRange.Cells[i, DeputaVardaColNr].Value2 != null)
                {
                    var deputatsName = xlRange.Cells[i, DeputaVardaColNr].Value2.ToString();
                    deputats = CreateDeputy(deputatsName, MuniName);
                }
                else
                {
                    continue;
                }


                if (xlRange.Cells[i, ApmeklejumaNrColNr] != null && xlRange.Cells[i, ApmeklejumaNrColNr].Value2 != null)
                {
                    apmeklejumaNr = xlRange.Cells[i, ApmeklejumaNrColNr].Value2.ToString();
                }

                if (xlRange.Cells[i, DateColNr] != null && xlRange.Cells[i, DateColNr].Value2 != null)
                {
                    double d = double.Parse(xlRange.Cells[i, DateColNr].Value2.ToString());
                    date = DateTime.FromOADate(d);
                }

                if (xlRange.Cells[i, ApmeklejumaColNr] != null && xlRange.Cells[i, ApmeklejumaColNr].Value2 != null)
                {
                    var attendedRaw = xlRange.Cells[i, ApmeklejumaColNr].Value2.ToString();

                    attended = attendedRaw == "1";
                }

                if (xlRange.Cells[i, NeapmeklesanasIemeslaColNr] != null && xlRange.Cells[i, NeapmeklesanasIemeslaColNr].Value2 != null)
                {
                    neapmeklesanasIemesls = xlRange.Cells[i, NeapmeklesanasIemeslaColNr].Value2.ToString();
                    neapmeklesanasIemesls = string.IsNullOrEmpty(neapmeklesanasIemesls) ||
                                            neapmeklesanasIemesls.Length < 2
                        ? string.Empty
                        : neapmeklesanasIemesls;
                }

                if (deputats != null && date.Year > 2012)
                {
                    apmeklejums.Deputats = deputats;
                    apmeklejums.Datums = date;
                    apmeklejums.Apmekleja = attended;
                    apmeklejums.ApmeklejumaNr = apmeklejumaNr;
                    apmeklejums.NeapmeklesanasIemesls = neapmeklesanasIemesls;
                }

                tmp.Add(apmeklejums);

                if (i%30 == 0)
                {
                    SaveAttendance(tmp);
                    tmp = new List<Apmeklejums>();
                }
                
            }

            SaveAttendance(tmp);

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //close and release
            xlWorkbook.Close();

            //quit and release
            xlApp.Quit();

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private static void CreateMunicipality(string municipality, string municipalityCode)
        {
            if (db.Pasvaldibas.FirstOrDefault(x => x.Code == municipality.ToUpper()) != null)
                return;

            var pasvaldiba = new Pasvaldiba
            {
                Code = municipality.ToUpper(),
                Name = municipality,
                CodeNr = municipalityCode
            };

            db.Pasvaldibas.Add(pasvaldiba);
            db.SaveChanges();
            Console.WriteLine($"Municipality added {pasvaldiba.CodeNr} {pasvaldiba.Code} {pasvaldiba.Name}");
            //return db.Pasvaldibas.FirstOrDefault(x => x.Code == municipality.ToUpper());
        }

        private static Deputats CreateDeputy(string name, string municipality)
        {
            var deputy = db.Deputati.FirstOrDefault(x => x.Name == name && x.Pasvaldiba.Code == municipality.ToUpper());

            if (deputy != null)
            {
                return deputy;
            }

            var muni = db.Pasvaldibas.FirstOrDefault(x => x.Code == municipality.ToUpper());
            var deputats = new Deputats
            {
                Pasvaldiba = muni,
                Name = name
            };

            db.Deputati.Add(deputats);
            db.SaveChanges();

            Console.WriteLine($"Deputy added {deputats.Name}");

            return db.Deputati.FirstOrDefault(x => x.Name == name && x.Pasvaldiba.Code == municipality.ToUpper());
        }

        private static void SaveAttendance(List<Apmeklejums> apmeklejumi)
        {
            db.Apmeklejumi.AddRange(apmeklejumi);
            db.SaveChanges();

            foreach (var apmeklejums in apmeklejumi)
            {
                Console.WriteLine($"Added: {apmeklejums.Datums} {apmeklejums.Deputats.Name} {apmeklejums.Apmekleja} {apmeklejums.NeapmeklesanasIemesls}");
            }
        }
    }
}
