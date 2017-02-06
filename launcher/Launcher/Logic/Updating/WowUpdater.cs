using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Constants;
using WowLauncher.Utils;
using WowSuite.Utils;
using WowSuite.Utils.Patching;

namespace WowSuite.Launcher.Logic.Updating
{
    class WowUpdater
    {
        /// <summary>Корневая папка, в которой находится апдейтер</summary>
        private readonly string _rootDirectory;
        /// <summary>Адрес pid файла на сервере</summary>
        private readonly string _serverPidFile;

        private readonly string _localPidFile;

        /// <summary>Адрес файла c описанием файлов для обновления на сервере</summary>
        private readonly string _serverPatchListFile;

        private readonly string _serverFilesRoot;

        private readonly SynchronizationContext _syncContext;
        private readonly WebClientFactory _webClientFactory;
        private readonly WebClient _webClient;

        private UpdateState _currentState; //Текущее состояние апдейтера
        private string _tempFolder;

        private Pid _pidFromServer;

        /// <summary>
        /// Инициализирует экземпляр класса.
        /// </summary>
        /// <param name="rootDirectory">Корневая папка, в которой происходят обновления</param>
        /// <param name="serverPidFile">Адрес pid файла на сервере</param>
        /// <param name="localPidFile">Путь к локальному Pid файлу</param>
        /// <param name="serverPatchListFile">Адрес файла c описанием файлов для обновления на сервере</param>
        /// <param name="serverFilesRoot">Корневая папка на сервере, в которой лежат файлы для обновления</param>
        public WowUpdater(string rootDirectory, string serverPidFile, string localPidFile, string serverPatchListFile, string serverFilesRoot)
        {
            if (!Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException("rootDirectory");
            }

            _rootDirectory = rootDirectory;
            _serverPidFile = serverPidFile;
            _localPidFile = localPidFile;
            _serverPatchListFile = serverPatchListFile;
            _serverFilesRoot = serverFilesRoot;

            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _webClientFactory = new WebClientFactory();
            _webClient = _webClientFactory.Create();

            _currentState = UpdateState.None;
            TempFolder = Path.Combine(Path.GetTempPath(), Wow.FolderName.TEMP_FOLDER_NAME);
        }

        /// <summary>
        /// Путь к временной папке, в которую будут сохраняться файлы обновления.
        /// </summary>
        public string TempFolder
        {
            get { return _tempFolder; }
            set
            {
                if (_currentState != UpdateState.None)
                    throw new InvalidOperationException("Нельзя изменить временную папку в процессе работы апдейтера");

                if (value == null)
                    throw new ArgumentNullException("value");

                _tempFolder = value;
            }
        }

        /// <summary>Уведомляет об изменении состояния апдейтера</summary>
        public event EventHandler<UpdateStateEventArgs> UpdateStateChanged;

        /// <summary>Прогресс выполнения обновления изменился от 0 до 100%</summary>
        public event EventHandler<ProgressEventArgs> UpdateProgressChanged;

        protected void OnUpdateStateChanged(UpdateState newState)
        {
            UpdateState oldState = _currentState;
            _currentState = newState;
            var handler = UpdateStateChanged;
            if (handler != null)
            {
                //перенаправляем выполнение кода подписчиков в тот поток, в котором был создан экземпляр этого класса
                _syncContext.Post((unused) =>
                {
                    handler(this, new UpdateStateEventArgs(newState, oldState));
                }, null);
            }
        }

        protected void OnUpdateProgressChanged(int percentage)
        {
            var handler = UpdateProgressChanged;
            if (handler != null)
            {
                //перенаправляем выполнение кода подписчиков в тот поток, в котором был создан экземпляр этого класса
                _syncContext.Post((unused) =>
                {
                    handler(this, new ProgressEventArgs(percentage));
                }, null);
            }
        }

        /// <summary>
        /// Проверить наличие обновлений на сервере.
        /// </summary>
        /// <returns></returns>
        public Task<bool> CheckUpdateAsync()
        {
            return Task.Factory.StartNew(() =>
             {
                 //Уведомляем подписчиков, что проверка наличия новой версии запущена.
                 OnUpdateStateChanged(UpdateState.Checking);

                 string localPidFile = _localPidFile;
                 string localPidHash = null;
                 if (File.Exists(localPidFile)) //если локальный файл с хэшем версии обновления найден
                 {
                     localPidHash = Pid.FromTextFile(localPidFile).Hash;
                 }

                 WebClient web = _webClientFactory.Create();
                 string pidText = web.DownloadString(_serverPidFile);
                 Pid downloadedPid = Pid.FromString(pidText);

                 if (localPidHash == null)
                 {
                     Pid newLocalPid = Pid.FromVersionNumber(-1);
                     localPidHash = newLocalPid.Hash;
                     using (StreamWriter writer = File.CreateText(localPidFile))
                     {
                         writer.Write(newLocalPid.ToString());
                     }
                 }

                 OnUpdateStateChanged(UpdateState.None);
                 if (downloadedPid.Hash != localPidHash)
                 {
                     _pidFromServer = downloadedPid;
                     return true;
                 }

                 //Уведомляем подписчиков, что обновление не требуется.
                 OnUpdateStateChanged(UpdateState.NotNeeded);
                 return false;
             });
        }
        public void Update()
        {
            if (_currentState != UpdateState.None)
                return;
            //Чтобы максимально быстро сменить состояние не дав выполнить проверку до того, как состояние изменится из другого потока
            _currentState = UpdateState.Started;

            //Запускаем выполнение кода в потоке из пула потоков.
            ThreadPool.QueueUserWorkItem(unused =>
            {
                //Уведомляем подписчиков, что обновление запущено.
                OnUpdateStateChanged(UpdateState.Started);

                //Скачиваем патч лист
                string patchList = _webClient.DownloadString(_serverPatchListFile);
                //Распарсиваем и получаем объект-описание патча
                var patch = new PatchInfo(patchList);

                //Получить путь до папки с данными обновляемого клиента (папка, в которой находится апдейтер/лаунчер и данные для обновления).
                string dataDirectory = Path.Combine(_rootDirectory, Wow.FolderName.Client.DATA_FOLDER_NAME);
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }
                //Получить все mpq файлы, находящиеся на локальном диске (включая все подпапки).
                string[] localMpqFiles = Directory.GetFiles(dataDirectory, "*.mpq", SearchOption.AllDirectories);

                //Отфильтровать оставив только те локальные файлы, имена которых совпадают с именами файлов в патче
                Dictionary<string, FileInfo> matchingFiles =
                    localMpqFiles.Where( //Отфильтровать, где
                    f => patch.UpdateFiles.Any( //локальный файл f совпадает
                        pf => pf.FileName == Path.GetFileName(f))) //с хоть одним файлом в патче по имени
                        .Select(mf => new FileInfo(mf)) //Каждый элемент коллекции проецировать в новый тип данных
                        .ToDictionary(info => info.Name); //отфильтрованную коллекцию конвертировать в словарь с ключом "имя файла"

                //последнее значение прогресса выполнения в процентах
                int lastPercentValue = 0;

                //Создаём временную папку, если не существует. Папка, в которую будем качать новые файлы обновления
                if (!Directory.Exists(TempFolder))
                    Directory.CreateDirectory(TempFolder);

                //Создаём сопоставимый список скачанных файлов с информацией об этом файле
                //ранее полученной с сервера (из патч листа)
                var downloaded = new List<Tuple<string, UpdateFile>>(patch.UpdateFiles.Length);

                //создаём переменную счётчик кол-ва скачанных байт всех файлов
                long downloadedBytesLength = 0L;
                //скачиваем все файлы
                for (int i = 0; i < patch.UpdateFiles.Length; i++)
                {
                    UpdateFile updateFile = patch.UpdateFiles[i];

                    string tempFile = Path.Combine(TempFolder, updateFile.FileName);

                    if (matchingFiles.ContainsKey(updateFile.FileName))
                    {
                        FileInfo file = matchingFiles[updateFile.FileName];
                        string localFileHash = HashHelper.GetMD5HashOfFile(file.FullName);
                        if (localFileHash == updateFile.Hash)
                        {
                            //то к сумме скачанных байтов добавляем его размер,
                            //чтобы далее правильно считался прогресс выполнения скачивания в процентах
                            downloadedBytesLength += updateFile.FileSize;
                            lastPercentValue = UpdateProgress(downloadedBytesLength, patch.PatchLength, lastPercentValue);
                            continue; //выходим из текущей итерации цикла и цикл переходит к следующей
                        }
                    }

                    long offset = 0L;
                    //Если файл существует (был скачан ранее) и скачан полностью (хэш совпадает с хэшем из патч листа)
                    if (File.Exists(tempFile))
                    {
                        string hash = HashHelper.GetMD5HashOfFile(tempFile);
                        if (hash == updateFile.Hash) //сверяем хэши
                        {
                            //то к сумме скачанных байтов добавляем его размер,
                            //чтобы далее правильно считался прогресс выполнения скачивания в процентах
                            downloadedBytesLength += updateFile.FileSize;
                            downloaded.Add(Tuple.Create(tempFile, updateFile));
                            lastPercentValue = UpdateProgress(downloadedBytesLength, patch.PatchLength, lastPercentValue);
                            continue; //выходим из текущей итерации цикла и цикл переходим к следующей
                        }
                        /*-------------   докачка-------------------------*/
                        else
                        {
                            var fileInfo = new FileInfo(tempFile);
                            offset = fileInfo.Length; //Сколько скачано байт. На сколько надо сдвинуть при скачивании.
                            downloadedBytesLength += fileInfo.Length; //теперь прогресс будет правильно считаться
                        }
                    }

                    Uri url = new Uri(UrlHelper.Combine(_serverFilesRoot, updateFile.FileName));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    if (offset > 0L) //L - значит, что литерал 0 типа long или Int64, что одно и тоже. Без L будет типа int.
                    {
                        request.AddRange(offset);
                    }
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream source = response.GetResponseStream();
                        if (source != null)
                        {
                            byte[] buffer = new byte[102400]; //102400 байт = 100 Килобайт
                            using (FileStream fs = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                fs.Position = offset;
                                while (true)
                                {
                                    //кол-во реально прочитанных байт (буфер может быть больше,
                                    //чем на последней итерации цикла "do while" реально прочитали)
                                    int readed = source.Read(buffer, 0, buffer.Length);
                                    if (readed == 0) break;

                                    fs.Write(buffer, 0, readed);
                                    downloadedBytesLength += readed;
                                    lastPercentValue = UpdateProgress(downloadedBytesLength, patch.PatchLength, lastPercentValue);
                                }
                            }

                            downloaded.Add(Tuple.Create(tempFile, updateFile));
                        }
                    }
                }

                foreach (Tuple<string, UpdateFile> tuple in downloaded) //<путь к временному файлу, соответствующее описание этому файлу>
                {
                    UpdateFile updateFile = tuple.Item2;
                    string tempFile = tuple.Item1;
                    string directory = string.Empty;
                    if (updateFile.FolderName == Wow.FolderName.Client.DATA_FOLDER_NAME)
                    {   // ../Data
                        directory = Path.Combine(_rootDirectory, Wow.FolderName.Client.DATA_FOLDER_NAME);
                    }
                    else if (updateFile.FolderName == Wow.FolderName.Client.LOCALE_FOLDER_NAME)
                    {   // ../Data/ruRU
                        directory = Path.Combine(_rootDirectory, Wow.FolderName.Client.DATA_FOLDER_NAME, Wow.FolderName.Client.LOCALE_FOLDER_NAME);
                    }
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string file = Path.Combine(directory, updateFile.FileName);
                    if (File.Exists(file))
                    {
                        Debug.WriteLine("Файл удалён:{0}{1}", Environment.NewLine, file);
                        File.Delete(file);
                    }
                    Debug.WriteLine("Файл перемещён{0}Из: {1}{0}в: {2}", Environment.NewLine, tempFile, file);
                    File.Move(tempFile, file);
                }

                using (StreamWriter writer = File.CreateText(_localPidFile))
                {
                    writer.Write(_pidFromServer.ToString());
                }

                OnUpdateStateChanged(UpdateState.Completed);
                OnUpdateStateChanged(UpdateState.None);
            });
        }

        /// <summary>
        /// Обновить прогресс выполнения, если прогресс выполнения изменился хотя 
        /// бы на один процент, уведомить подписчиков о изменении прогресса выполнения
        /// </summary>
        /// <param name="downloadedBytesLength">Кол-во скачанных байт</param>
        /// <param name="patchLength">Общий размер всех файлов в патче</param>
        /// <param name="lastPercentValue">Предыдущее прогресса выполнения в процентах</param>
        /// <returns>Вернёт новое значение прогресса выполнения в процентах</returns>
        private int UpdateProgress(long downloadedBytesLength, long patchLength, int lastPercentValue)
        {
            //Процент закачки = сумма скачанного / заранее известный объем всех файлов * 100
            int percentage = (int)(downloadedBytesLength / (double)patchLength * 100d);
            if (percentage < 0 || percentage > 100)
            {
                Debug.WriteLine("percentage = {0}", percentage);
            }
            if (lastPercentValue != percentage)
            {
                OnUpdateProgressChanged(percentage);
            }
            return percentage; //вернуть новое значение прогресса выполнения в процентах
        }
    }
}