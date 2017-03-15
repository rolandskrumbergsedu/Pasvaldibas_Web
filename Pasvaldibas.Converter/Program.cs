using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Converter
{
    class Program
    {
        private const int ApmeklejumaNrColNr = 1;
        private const int DeputaVardaColNr = 2;
        private const int DateColNr = 3;
        private const int ApmeklejumaColNr = 4;
        private const int NeapmeklesanasIemeslaColNr = 5;

        static readonly ApplicationDbContext db = new ApplicationDbContext();


        static void Main(string[] args)
        {
            var files = Directory.GetFiles($"C:\\Work_misc\\Protokoli\\Gatavie");

            foreach (var file in files)
            {
                var split = file.Split('\\');
                var fileName = split[split.Length - 1].Replace(".xlsx",string.Empty).Replace(".xls", string.Empty);

                var muniName = fileName;
                var muniCode = NameToCode(fileName);

                try
                {
                    HandleMunicipality(file, muniName, muniCode);
                }
                catch (Exception ex)
                {
                    File.AppendAllText("C:\\Work_misc\\Protokoli\\ErrorLogs.txt", $"Exception occured. {muniName} {muniCode} - {ex.Message}" + Environment.NewLine);
                }
            }

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private static void HandleMunicipality(string resultFile, string name, string code)
        {

            var xlApp = new Microsoft.Office.Interop.Excel.Application();
            var xlWorkbook = xlApp.Workbooks.Open(resultFile);
            var xlWorksheet = xlWorkbook.Sheets[1];
            var xlRange = xlWorksheet.UsedRange;

            var rowCount = xlRange.Rows.Count;

            // TO DO: Izveido jaunu pasvaldibu un iegūst atsauci uz to
            CreateMunicipality(name, code);

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
                    deputats = CreateDeputy(deputatsName, name);
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
                    try
                    {
                        double d = double.Parse(xlRange.Cells[i, DateColNr].Value2.ToString());
                        date = DateTime.FromOADate(d);
                    }
                    catch (Exception) // Not a date format,check if string
                    {
                        var posibleDate = (string)xlRange.Cells[i, DateColNr].Value2.ToString();
                        var split = posibleDate.Split('.');
                        if (split.Length == 3)
                        {
                            date = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));
                        }
                        else
                        {
                            throw;
                        }

                    }
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

                if (i % 30 == 0)
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
            File.AppendAllText("C:\\Work_misc\\Protokoli\\Logs.txt", $"Municipality added {pasvaldiba.CodeNr} {pasvaldiba.Code} {pasvaldiba.Name}" + Environment.NewLine);
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
            File.AppendAllText("C:\\Work_misc\\Protokoli\\Logs.txt", $"Deputy added {deputats.Name}" + Environment.NewLine);

            return db.Deputati.FirstOrDefault(x => x.Name == name && x.Pasvaldiba.Code == municipality.ToUpper());
        }

        private static void SaveAttendance(List<Apmeklejums> apmeklejumi)
        {
            db.Apmeklejumi.AddRange(apmeklejumi);
            db.SaveChanges();

            foreach (var apmeklejums in apmeklejumi)
            {
                File.AppendAllText("C:\\Work_misc\\Protokoli\\Logs.txt", $"Added: {apmeklejums.Datums} {apmeklejums.Deputats.Name} {apmeklejums.Apmekleja} {apmeklejums.NeapmeklesanasIemesls}" + Environment.NewLine);
                Console.WriteLine($"Added: {apmeklejums.Datums} {apmeklejums.Deputats.Name} {apmeklejums.Apmekleja} {apmeklejums.NeapmeklesanasIemesls}");
            }
        }

        private static string NameToCode(string name)
        {
            return name.ToUpper()
                .Replace('Ē', 'E')
                .Replace('Ū', 'U')
                .Replace('Ī', 'I')
                .Replace('Ā', 'A')
                .Replace('Š', 'S')
                .Replace('Ķ', 'K')
                .Replace('Ļ', 'L')
                .Replace('Ž', 'Z')
                .Replace('Č', 'C')
                .Replace('Ņ', 'N')
                .Replace(' ', '_');
        }
    }
}
