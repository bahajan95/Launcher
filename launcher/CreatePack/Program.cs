using System;
using System.IO;
using Constants;
using WowSuite.Utils;
using WowSuite.Utils.Console;
using WowSuite.Utils.Patching;

namespace WowSuite.CreatePack
{
    internal class Program
    {
        const string COPYRIGHT = "CreatePack (c) SyntaxWEB 2015";

        private static void Main(string[] args)
        {
            Console.Title = COPYRIGHT;

            string langFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Wow.FileName.LANG_FILE_NAME);
            string pidFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Wow.FileName.PID_FILE_NAME);

            if (!File.Exists(langFile)) //Если файл не существует, вывод сообщения, завершение работы
            {
                ConsoleHelper.WriteErrorLine("File \"{0}\" not found!", Wow.FileName.LANG_FILE_NAME);
                Console.ReadKey();
                return; //завершение работы метода (в данном случае программы)
            }

            string[] files = Directory.GetFiles(
                AppDomain.CurrentDomain.BaseDirectory, "*.mpq", SearchOption.AllDirectories);
            if (files.Length == 0) //Если файлы не найдены, вывод сообщения, завершение работы
            {
                ConsoleHelper.WriteErrorLine("Put all *.mpq files next to the program.");
                Console.ReadKey();
                return; //завершение работы метода (в данном случае программы)
            }

            StreamReader reader = null;
            string message;
            try
            {
                reader = new StreamReader(pidFile);
                string pidHash = reader.ReadLine();
                string version = reader.ReadLine();
                message = string.Format("Enter a next version update (Current version: {0}): ", version);
                Console.Write(message);
                reader.Close();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteErrorLine("When reading a file \"{0}\" an error has occurred:", Wow.FileName.PID_FILE_NAME);
                ConsoleHelper.WriteErrorLine(ex.Message);
                Console.ReadKey();
                return; //завершение работы метода (в данном случае программы)
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }

            uint newVersion;
            while (!uint.TryParse(Console.ReadLine(), out newVersion))
            {
                ConsoleHelper.WriteErrorLine("Error! Enter a number a next version update.");
                Console.Write(message);
            }
            Console.WriteLine();

            int processedFilesCounter = 0;
            string localeFolderName = FileHelper.Txt.ReadFirstLine(langFile);

            string patchFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Wow.FileName.PATCH_FILE_NAME);
            using (StreamWriter sw = File.CreateText(patchFile))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    UpdateFile updFile = UpdateFile.FromFile(files[i], localeFolderName);
                    string formattedLine = updFile.ToString();

                    Console.WriteLine("Load => {0} {1}", updFile.FolderName, updFile.FileName);
                    Console.WriteLine("Complete => {0}", formattedLine);
                    sw.WriteLine(formattedLine);

                    //Console.WriteLine("1---------2---------3---------4---------5---------6---------7---------8---------"); //80 символов
                    ConsoleHelper.WriteSeparatorLine('-', ConsoleColor.DarkCyan);
                    processedFilesCounter++;
                }
            }

            string newVersionHash = HashHelper.GetMD5Hash(newVersion.ToString());
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(pidFile);
                writer.WriteLine(newVersionHash);
                writer.WriteLine(newVersion);
                writer.Close();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("When writing a file \"{0}\" an error has occurred:", Wow.FileName.PID_FILE_NAME);
                ConsoleHelper.WriteErrorLine(ex.Message);
                Console.ReadKey();
                return; //завершение работы метода (в данном случае программы)
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }

            Console.WriteLine("{0} created! Processed *.mpq files: {1}", Wow.FileName.PATCH_FILE_NAME, processedFilesCounter);
            Console.WriteLine();
            Console.WriteLine("{0}, Skype: darksapfir1,  vk.com/6stprod", COPYRIGHT);
            Console.ReadKey();
        }
    }
}