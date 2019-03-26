using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWorZonesCreation.Controllers
{
    public class CSVController
    {
        public string source { get; set; }

        public async Task<List<string>> LinesFile()
        {
            List<string> listResult = new List<string>();
            try
            {
                using (var reader = new StreamReader(this.source, Encoding.UTF8, true))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        //System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
                        //string s_unicode = line;
                        //byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(s_unicode);
                        //string s_unicode2 = System.Text.Encoding.UTF8.GetString(utf8Bytes);
                        listResult.Add(line);
                    }
                    return listResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error {0}, detalles: {1}", ex.Message, ex.InnerException.Message));
            }
            return null;
        }

        public string[] SplitBy(string text, char character)
        {
            string[] aResult;
            try
            {
                aResult = text.Split(character);
                return aResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error {0} {1}", ex.Message, ex.InnerException.Message));

            }
            return null;
        }

    }
}
