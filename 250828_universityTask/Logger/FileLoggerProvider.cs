using _250828_universityTask.Cache;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using System.IO;
using System.Text.Json;

namespace _250828_universityTask.Logger
{
    public class FileLoggerProvider
    {
        private readonly string _filePath = @"C:\Users\stv\source\repos\250828_universityTask\250828_universityTask\Logger\logfile.txt";
        private readonly ILogger<FileLoggerProvider> _logger;
        private string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public FileLoggerProvider(ILogger<FileLoggerProvider> logger)
        {
            _logger = logger;
            Load();
        }

        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath);
            }
        }

        public void SaveBehaviourLogging(string message, LoggerTopics topic)
        {
            string appendText = "[ " + topic + " ] " + time + " " + message + Environment.NewLine;

            _logger.LogInformation(message);
            File.AppendAllText(_filePath, appendText);
        }

        public void SaveExceptionLogging(string message)
        {
            string appendText = "[ Exception ] " + time + " " + message + Environment.NewLine;
            File.AppendAllText(_filePath, appendText);
        }
    }
}
