using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkyWorZonesCreation.Controllers;
using SkyWorZonesCreation.Models;

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

            Console.WriteLine(" Ingrese Ubicación (Folder) del archivo");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Por ejemplo C:\\Users\\inmotion\\Documents\\z");
            Console.ResetColor();
            sPath = Console.ReadLine();
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

            if (listworkZone.Count <= 1500)
            {
                foreach (var item in listworkZone)
                    WorkZoneMain(item);
            }
            else
            {

                var tmp0 = listworkZone.Take(1500).ToList();
                var tmp1 = listworkZone.Skip(1500).Take(1500).ToList();
                var tmp2 = listworkZone.Skip(3000).Take(1500).ToList();
                var tmp3 = listworkZone.Skip(4500).Take(1500).ToList();
                var tmp4 = listworkZone.Skip(6000).Take(1500).ToList();
                var tmpStromg = listworkZone.Skip(7500).ToList();


                Thread thread0 = new Thread(() => WorkZoneQueue(tmp0));
                Thread thread1 = new Thread(() => WorkZoneQueue(tmp1));
                Thread thread2 = new Thread(() => WorkZoneQueue(tmp2));
                Thread thread3 = new Thread(() => WorkZoneQueue(tmp3));
                Thread thread4 = new Thread(() => WorkZoneQueue(tmp4));
                Thread threadStrong = new Thread(() => WorkZoneQueue(tmpStromg));

                thread0.Start();
                thread1.Start();
                thread2.Start();
                thread3.Start();
                thread4.Start();
                threadStrong.Start();

                thread0.Join();
                thread1.Join();
                thread2.Join();
                thread3.Join();
                thread4.Join();
                threadStrong.Join();

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

        private static bool WorkZoneMain(WorkZone workZone)
        {
            Console.WriteLine("workZoneLabel:" + workZone.workZoneLabel + "   " + workZone.keylabel.FirstOrDefault() + "   status:" + workZone.status + "    workZoneName:" + workZone.workZoneName);
            bool flag = false;
            Logger(string.Format("workzone {0}|{1}|{2}|{3}|{4}|", workZone.workZoneLabel, workZone.status, workZone.travelArea, workZone.workZoneName, workZone.keylabel.FirstOrDefault()));

            var checkExist = ctrlworkZone.Exist(workZone);
            // if (checkExist.flag)
            //flag = ctrlworkZone.Set(workZone).flag;
            //else
            //  flag = ctrlworkZone.Create(workZone).flag;
            if (!checkExist.flag)
            {
                flag = ctrlworkZone.Create(workZone).flag;

                if (flag)
                {
                    rowOK = rowOK += 1;
                    Logger(string.Format("workzone {0}|{1}|{2}|{3}|{4}|", workZone.workZoneLabel, workZone.status, workZone.travelArea, workZone.workZoneName, workZone.keylabel.FirstOrDefault()), 1);
                }
                else
                {
                    rowBAD = rowBAD += 1;
                    Logger(string.Format("workzone {0}|{1}|{2}|{3}|{4}|", workZone.workZoneLabel, workZone.status, workZone.travelArea, workZone.workZoneName, workZone.keylabel.FirstOrDefault()), 2);
                }
            }
            return flag;
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
                    default:
                        temppath = @sPath + "\\log.txt";
                        break;
                }
                System.IO.StreamWriter file = new System.IO.StreamWriter(temppath, true);
                file.WriteLine(lines);
                file.Close();
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