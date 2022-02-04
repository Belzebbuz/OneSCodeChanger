using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSCodeChanger
{
    public class Example
    {
        public static string OneSLouncher = @"C:\Program Files\1cv8\8.3.14.1854\bin\1cv8.exe";
        public async Task MyMethod()
        {

            Console.WriteLine("Выгрузка файлов конфигурации");
            Console.WriteLine("Введите путь к базе данных /F модификатор для файловой базы /S для серверной \n Например /F \"D:\\Base\" или /S OPK01\\UT_local");
            Console.Write("Путь: ");
            string basePath = "/S OPK01\\UT_local";//Console.ReadLine();//"/F \"D:\\Влад\\Базы 1С\\СпецЭкз\""

            string tempPath = Path.GetTempPath() + "OneSTemp";
            CreateTempFolder(tempPath);
            string listOfFiles = Path.Combine(tempPath, "list.txt");


            Console.Write("Введите полное имя модуля для изменения: ");
            string moduleName = "ОбщийМодуль.СинхронизацияКлиентСервер";// Console.ReadLine(); // "ОбщийМодуль.СинхронизацияКлиентСервер ОбщийМодуль.СинхронизацияВызовСервера"
            using (StreamWriter streamWriter = new StreamWriter(listOfFiles))
            {
                await streamWriter.WriteLineAsync(moduleName);
            }

            OneS($"CONFIG {basePath} /DumpConfigToFiles \"{tempPath}\" -listFile \"{listOfFiles}\" -Format Plain");

            if (ContainsModule(tempPath, out string modulePath, out string xmlPath))
            {

                Cmd($"start {tempPath}");
                NotePad($"{modulePath}");
                //OneS($"CONFIG {basePath} /LoadConfigFromFiles \"{tempPath}\" -Files \"{xmlPath}\" -Format Plain");
                Directory.Delete(tempPath, true);
            }
            else
            {
                OneS($"CONFIG {basePath} /DumpConfigToFiles \"{tempPath}\" -listFile \"{listOfFiles}\" -Format Plain");
            }

            // "C:\Program Files\1cv8\8.3.17.1851\bin\1cv8.exe" CONFIG /F "D:\Влад\Базы 1С\СпецЭкз" /DumpConfigToFiles D:\Влад\Мусор\Модули -listFile "D:\Влад\Мусор\list.txt" -Format Plain
            //"C:\Program Files\1cv8\8.3.17.1851\bin\1cv8.exe" CONFIG /F "D:\Влад\Базы 1С\СпецЭкз" /LoadConfigFromFiles D:\Влад\Мусор\Модули -Files "D:\Влад\Мусор\Модули\CommonModule.СинхронизацияКлиентСервер.xml" -Format Plain

        }

        private static void CreateTempFolder(string tempPath)
        {
            Directory.CreateDirectory(tempPath);
            if (Directory.GetFiles(tempPath).Length > 0)
            {
                Directory.GetFiles(tempPath).ToList().ForEach(f => File.Delete(f));
            }

        }

        public static bool ContainsModule(string path, out string modulePath, out string xmlPath)
        {
            var files = Directory.GetFiles(path);
            for (int i = 0; i < 3; i++)
            {
                Task.Delay(2000).Wait();
                Console.WriteLine($"Попытка найти файлы №{i}");
                if (files.Where(file => file.Contains("Module.txt")).Count() > 0)
                {
                    modulePath = files.Where(file => file.Contains("Module.txt")).FirstOrDefault();
                    xmlPath = files.Where(file => file.Contains(".xml")).FirstOrDefault();
                    return true;
                }
            }
            modulePath = null;
            xmlPath = null;
            return false;
        }

        public void Cmd(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line}",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }
        public void OneS(string line)
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = OneSLouncher,
                Arguments = $"{ line}",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            proc.Start();
            proc.WaitForExit();

            Process.Start(new ProcessStartInfo
            {
                FileName = OneSLouncher,
                Arguments = $"{ line}",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }

        public void NotePad(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad",
                Arguments = $" {line}"
            }).WaitForExit();
        }
    }
}
