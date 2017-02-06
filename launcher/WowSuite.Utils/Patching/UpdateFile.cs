using System;
using System.IO;
using System.Linq;

namespace WowSuite.Utils.Patching
{
    /// <summary>
    /// Представляет описание файла обновления.
    /// </summary>
    public class UpdateFile
    {        
        /// <summary>
        /// Создать экземпляр класса.
        /// </summary>
        /// <exception cref="ArgumentNullException">Генерируется, если переданная строка имеет значение null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Генерируется, если переданный размер файла в байтах меньше нуля</exception>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <param name="hash"></param>
        /// <param name="fileSize"></param>
        public UpdateFile(string fileName, string folderName, string hash, long fileSize)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (folderName == null)
                throw new ArgumentNullException("folderName");
            if (hash == null)
                throw new ArgumentNullException("hash");
            if (fileSize < 0L)
                throw new ArgumentOutOfRangeException("fileSize");

            FileName = fileName;
            FolderName = folderName;
            Hash = hash;
            FileSize = fileSize;
        }

        /// <summary>Название файла</summary>
        public string FileName { get; private set; }

        /// <summary>Название папки</summary>
        public string FolderName { get; private set; }

        /// <summary>Хэш код</summary>
        public string Hash { get; private set; }

        /// <summary>Размер файла в байтах</summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Получить информацию о файле апдейта из строки в специальном формате.
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="FormatException">Генерируется, если переданная строка имеет неверный формат</exception>
        /// <exception cref="ArgumentNullException">Генерируется, если переданная строка имеет значение null</exception>
        /// <exception cref="OverflowException">Генерируется, если неудаётся распарсить размер файла в байтах</exception>
        /// <returns></returns>
        public static UpdateFile FromString(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            if (line.StartsWith(" "))
                line = line.TrimStart();

            if (line.EndsWith(" "))
                line = line.TrimEnd();

            string[] segments = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments == null || segments.Length != 4)
            {
                throw new FormatException("The string has an invalid format");
            }

            long fileSize = long.Parse(segments[3]);
            return new UpdateFile(segments[1], segments[0], segments[2], fileSize);
        }

        /// <summary>
        /// Создать экземпляр класса распарсив файл.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="localeFolderName">Название папки локализации программы</param>
        /// <exception cref="FileNotFoundException">Генерируется, если файл не найден</exception>
        /// <exception cref="ArgumentNullException">Генерируется, если переданная строка имеет значение null</exception>
        /// <returns></returns>
        public static UpdateFile FromFile(string file, string localeFolderName)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (file == null)
                throw new ArgumentNullException("localeFolderName");

            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File: \"{0}\" not found", Path.GetFileName(file));
            }

            string rootFolderName = file.Split(Path.DirectorySeparatorChar).Last().Split('-')[1];
            string folderName = rootFolderName == localeFolderName ? localeFolderName : "Data";
            string[] filePathSegments = file.Split(Path.DirectorySeparatorChar);
            string fileName = filePathSegments[filePathSegments.Length - 1];

            var updateFile = new UpdateFile(fileName, folderName, HashHelper.GetMD5HashOfFile(file), fileInfo.Length);
            return updateFile;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", FolderName, FileName, Hash, FileSize);
        }
    }
}
