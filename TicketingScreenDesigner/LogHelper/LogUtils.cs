using System.Text.Json;

namespace LogUtils {
	public class LogEvent {
		public LogEvent(string message, DateTime timeStamp, EventSeverity severity) {
			Message = message;
			TimeStamp = timeStamp;
			Severity = GetSeverity(severity);
			Source = null;
			StackTrace = null;
		}

		public LogEvent(string message, DateTime timeStamp, EventSeverity severity, string? source, string? stackTrace) : this(message, timeStamp, severity) {
			Source = source;
			StackTrace = stackTrace;
		}

		private string GetSeverity(EventSeverity severity) {
			switch (severity) {
				case EventSeverity.Error:
					return "Error";
				case EventSeverity.Warning:
					return "Warning";
				case EventSeverity.Info:
					return "Info";
				default:
					return "Undefined";
			}
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
		private static readonly string logsFilePath = Path.Join(logsDirectoryPath, "logs.txt");

		public static bool IsInitialized() {
			return Directory.Exists(logsDirectoryPath);
		}

		public static void InitializeLogsDirectory() {
			try {
				Directory.CreateDirectory(logsDirectoryPath);
			}
			catch (Exception ex) {
				Console.Error.WriteLine(ex.Message);
			}
		}

		public static void Log(LogEvent logEvent) {
			try {
				if (!IsInitialized()) {
					InitializeLogsDirectory();
				}

				var options = new JsonSerializerOptions() {
					WriteIndented = true,
				};

				using (var fileWriter = File.AppendText(logsFilePath)) {
					fileWriter.WriteLine(JsonSerializer.Serialize(logEvent, typeof(LogEvent), options));
				}
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
