using ExceptionUtils;
using System.Text.Json;

namespace LogUtils {
	public class LogEvent {
		public LogEvent(string message, DateTime timeStamp, EventSeverity severity) {
			Message = message;
			TimeStamp = timeStamp;
			Severity = Enum.GetName(severity) ?? "Unknown";
			Source = null;
			StackTrace = null;
		}

		public LogEvent(string message, DateTime timeStamp, EventSeverity severity, string? source, string? stackTrace) : this(message, timeStamp, severity) {
			Source = source;
			StackTrace = stackTrace;
		}

		public string Message { get; private set; }
		public DateTime TimeStamp { get; private set; }
		public string Severity { get; private set; }
		public string? Source { get; private set; }
		public string? StackTrace { get; private set; }
	}

	public enum EventSeverity {
		Error,
		Warning,
		Info,
	}

	public static class LogsHelper {
		private static readonly string logsDirectoryPath = Path.Join(Directory.GetCurrentDirectory(), "logs");
		private static readonly string logsFilePath = Path.Join(logsDirectoryPath, "logs.json");

		public static bool IsInitialized() {
			try {
				return Directory.Exists(logsDirectoryPath) && File.Exists(logsFilePath);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		public static void InitializeLogsFile() {
			try {
				if (!File.Exists(logsFilePath)) {
					if (!Directory.Exists(logsDirectoryPath))
						Directory.CreateDirectory(logsDirectoryPath);

					using var fileWriter = File.AppendText(logsFilePath);
					fileWriter.WriteLine("[]");
				}
			}
			catch (Exception ex) {
				Console.Error.WriteLine(ex.Message);
			}
		}

		public static void Log(LogEvent logEvent) {
			try {
				InitializeLogsFile();

				var options = new JsonSerializerOptions() {
					WriteIndented = true,
				};

				bool isFirstLog = true;
				using (var fileStream = new FileStream(logsFilePath, FileMode.Open, FileAccess.ReadWrite)) {
					fileStream.Seek(1, SeekOrigin.Begin);
					if (fileStream.ReadByte() == '{')
						isFirstLog = false;

					fileStream.Seek(-3, SeekOrigin.End);
					if (fileStream.ReadByte() == ']')
						fileStream.SetLength(fileStream.Length - 3);
				}

				using var fileWriter = File.AppendText(logsFilePath);
				fileWriter.Write((isFirstLog ? "" : ",\n") + JsonSerializer.Serialize(logEvent, typeof(LogEvent), options));
				fileWriter.WriteLine("]");
			}
			catch (Exception ex) {
				Console.Error.WriteLine(ex.Message);
			}
		}

		public static void Log(string message, DateTime timestamp, EventSeverity severity) {
			Log(new LogEvent(message, timestamp, severity));
		}
	}
}
