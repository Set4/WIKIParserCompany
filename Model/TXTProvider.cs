using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{


    static class TXTProvider
    {
        /// <summary>
        /// Чтение .txt файла
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <returns>список строк</returns>
        public static List<string> ReadTextFile(string path)
        {
            Parser parse = new Parser();
            List<string> lines = new List<string>();
            try
            {
                using (StreamReader file = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        line = parse.ReplaceNameCompani(line);
                        if (!String.IsNullOrWhiteSpace(line.Trim()))
                            lines.Add(line);
                    }
                }
                return lines;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(ReadTextFile-метод) :  {0}", ex.Message);

                return null;
            }
        }

     

        /// <summary>
        /// Запись в .txt файла
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="text">данные для записи</param>
        public static void WriteTextFile(string path, string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                {
                    if(!String.IsNullOrWhiteSpace(text))
                    sw.WriteLine(text);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error(WriteTextFile-метод) :  {0}", ex.Message);
            }

        }
    }
}
