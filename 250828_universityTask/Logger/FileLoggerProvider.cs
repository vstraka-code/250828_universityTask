
namespace _250828_universityTask.Logger
{
    public class FileLoggerProvider
    {
        #region Properties
        private readonly string _filePath = @"C:\Users\stv\source\repos\250828_universityTask\250828_universityTask\Logger\logfile.txt";
        private string _time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        private readonly bool _disableFileIO;

        [Inject] private readonly ILogger<FileLoggerProvider> _logger;
        #endregion

        #region Constructor
        public FileLoggerProvider(ILogger<FileLoggerProvider> logger, bool disableFileIO = false)
        {
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_TEST") == "true")
            {
                disableFileIO = true;
            }

            _logger = logger;
            _disableFileIO = disableFileIO;

            if (!_disableFileIO)
            {
                Load();
            }
        }
        #endregion

        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath);
            }
        }

        #region SaveLogging
        public void SaveBehaviourLogging(string message, LoggerTopics topic)
        {
            if (_disableFileIO) return;

            string appendText = "[ " + topic + " ] " + _time + " " + message + Environment.NewLine;

            _logger.LogInformation(message);
            File.AppendAllText(_filePath, appendText);
        }

        public void SaveExceptionLogging(string message)
        {
            if (_disableFileIO) return;

            string appendText = "[ Exception ] " + _time + " " + message + Environment.NewLine;

            File.AppendAllText(_filePath, appendText);
        }
        #endregion
    }
}
