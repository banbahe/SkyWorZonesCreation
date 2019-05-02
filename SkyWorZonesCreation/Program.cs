using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkyWorZonesCreation.Controllers;
using SkyWorZonesCreation.Models;
using Newtonsoft.Json;

namespace SkyWorZonesCreation
{
    public class Program
    {
        public static string sPath { get; set; }
        public static List<string> list { get; set; } = new List<string>();
        public static int rowOK { get; set; }
        public static int rowBAD { get; set; }

        private static IWorkZone ctrlworkZone { get; set; }

        static void Main(string[] args)
        {
            #region Code Create WorkZones

            ctrlworkZone = new WorkZoneController();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Leyendo Ubicación (Folder) del archivo");
            Console.WriteLine(" Por ejemplo C:\\Users\\inmotion\\Documents\\z");
            Console.ResetColor();
            sPath = ConfigurationManager.AppSettings["pathfiles"].ToString();
            // sPath = @"C:\Users\inmotion\Documents\z\workzoneTDD\MX\";
            if (Directory.Exists(sPath))
                Console.WriteLine("Leyendo archivos CSV");
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Ubicación no valida {0}", sPath));
                Thread.Sleep(1800);
                Console.ResetColor();
                throw new Exception(" x=> { x.id = 'error' }");
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Logger("----------------------------------------------------------------------------------");
            Logger("Inicio");
            Logger(DateTime.Now.ToString());
            Console.WriteLine("Leyendo archivos CSV");
            // sPath = @"C:\Users\inmotion\Documents\bitbucket\ofsc\skymx\doc\workzones";
            ReadCSV(sPath);

            List<WorkZone> listworkZone = new List<WorkZone>();
            foreach (var item in Program.list)
            {
                string[] result = item.Split(';');
                WorkZone objWorkZone = new WorkZone();
                objWorkZone.workZoneName = result[0];
                objWorkZone.workZoneLabel = result[1];
                objWorkZone.travelArea = result[2];
                objWorkZone.status = result[3];
                objWorkZone.keylabel.Add(result[4]);
                listworkZone.Add(objWorkZone);
            }

            Console.WriteLine("TOTAL DE REGISTROS " + listworkZone.Count());
            Logger(" TOTAL DE REGISTROS " + listworkZone.Count());


            Console.WriteLine("En proceso " + listworkZone.Count() + " registros .");

            if (listworkZone.Count <= 500)
            {
                foreach (var item in listworkZone)
                    WorkZoneMain(item);
            }
            else
            {

                double microprocess = listworkZone.Count / 5;
                int microprocess0 = (int)microprocess;
                int microprocess1 = microprocess0 * 2;
                int microprocess2 = microprocess0 * 3;

                var tmp0 = listworkZone.Take(microprocess0).ToList();
                var tmp1 = listworkZone.Skip(microprocess0).Take(microprocess0).ToList();
                var tmp2 = listworkZone.Skip(microprocess1).Take(microprocess0).ToList();
                var tmp3 = listworkZone.Skip(microprocess2).Take(microprocess0).ToList();

                var tmpStromg = listworkZone.Skip(microprocess2).ToList();

                Thread thread0 = new Thread(() => WorkZoneQueue(tmp0));
                Thread thread1 = new Thread(() => WorkZoneQueue(tmp1));
                Thread thread2 = new Thread(() => WorkZoneQueue(tmp2));
                Thread thread3 = new Thread(() => WorkZoneQueue(tmp3));
                Thread threadStrong = new Thread(() => WorkZoneQueue(tmpStromg));

                threadStrong.Start();
                thread0.Start();
                thread1.Start();
                thread2.Start();
                thread3.Start();

                threadStrong.Join();
                thread0.Join();
                thread1.Join();
                thread2.Join();
                thread3.Join();
            }


            Console.WriteLine("TOTAL DE REGISTROS OK " + rowOK);
            Console.WriteLine("TOTAL DE REGISTROS MALOS " + rowBAD);
            stopwatch.Stop();
            Console.WriteLine("Ha terminado");
            Console.WriteLine("Se tardo en Milisegundos " + stopwatch.Elapsed.TotalMilliseconds);
            Console.WriteLine("Se tardo en Segundos " + stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine("Se tardo en Minutos " + stopwatch.Elapsed.TotalMinutes);
            Logger("Milisegundos " + stopwatch.Elapsed.TotalMilliseconds.ToString());
            Logger("Segundos " + stopwatch.Elapsed.TotalSeconds.ToString());
            Logger("Minutos " + stopwatch.Elapsed.TotalMinutes.ToString());
            Logger(DateTime.Now.ToString());
            Logger(" End ");
            Logger("----------------------------------------------------------------------------------");
            Console.ReadLine();
            Console.ReadKey();
            #endregion

        }

        private static void ReadCSV(string path)
        {
            Console.Clear();
            path = Path.GetFullPath(path);
            var files = Directory.GetFiles(path, "*.csv");

            foreach (var item in files)
            {
                try
                {
                    CSVController objCSVController = new CSVController();
                    objCSVController.source = @item;

                    Task<List<string>> task = objCSVController.LinesFile();
                    task.Wait();
                    var result = task.Result;
                    Program.list.AddRange(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error  al leer el archivo {0} : Exepción :{1}", item, ex.Message));
                }
            }
            Console.Clear();
        }
        private static void WorkZoneQueue(List<WorkZone> listworkZone)
        {
            int limitTemp = 0;

            foreach (var item in listworkZone)
            {
                limitTemp++;
                WorkZoneMain(item);

                if (limitTemp == 1000)
                {
                    Thread.Sleep(1000);
                    limitTemp = 0;
                }
            }

            // return string.Concat(listworkZone.Count, ",", good, ",", bad);
        }

        private static void WorkZoneMain(WorkZone workZone)
        {
            ResponseOFSC responseOFSC = new ResponseOFSC();
            Console.WriteLine("workZone :" + workZone.workZoneLabel + "travelArea: " + workZone.travelArea);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.None;

            Logger(string.Format("workzone {0}", workZone.workZoneLabel));

            if (!ctrlworkZone.Exist(workZone).flag)
                responseOFSC = ctrlworkZone.Create(workZone);
            else
                responseOFSC = ctrlworkZone.Set(workZone);


            if (responseOFSC.flag)
            {
                rowOK = rowOK += 1;
                Logger(JsonConvert.SerializeObject(workZone, settings), 4);
            }
            else
            {
                rowBAD = rowBAD += 1;
                Logger(JsonConvert.SerializeObject(workZone, settings), 5);
            }
        }

        /// <summary>
        /// 1 log_ok
        /// <br></br> 2 log_not
        /// <br></br> 3 log_json
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="opcion"></param>
        public static void Logger(String lines, int opcion = 0)
        {
            try
            {
                string temppath = string.Empty;
                switch (opcion)
                {
                    case 1:
                        temppath = @sPath + "\\log_ok.txt";
                        break;
                    case 2:
                        temppath = @sPath + "\\log_not.txt";
                        break;
                    case 3:
                        temppath = @sPath + "\\log_json.txt";
                        break;
                    case 4:
                        temppath = @sPath + "\\log_workzones_ok.txt";
                        break;
                    case 5:
                        temppath = @sPath + "\\log_workzones_no_creadas.txt";
                        break;
                    default:
                        temppath = @sPath + "\\log.txt";
                        break;
                }

                if (!string.IsNullOrEmpty(lines))
                {
                    System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
                    string s_unicode = lines;
                    byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(s_unicode);
                    string s_unicode2 = System.Text.Encoding.UTF8.GetString(utf8Bytes);

                    System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true);
                    file.WriteLine(s_unicode2);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(800);
                Logger(lines, opcion);
            }
        }
    }
}