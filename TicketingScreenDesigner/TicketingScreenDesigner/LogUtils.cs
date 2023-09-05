using System.Runtime.Serialization;
using System.Text.Json;

namespace TicketingScreenDesigner {
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
					return string.Empty;
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

		public static void InitializeLogsDirectory() {
			try {
				Directory.CreateDirectory(logsDirectoryPath);
			}
			catch {
				MessageBox.Show("Failed to create logs directory.");
			}
		}

		public static void Log(LogEvent logEvent) {
			try {
				var options = new JsonSerializerOptions() {
					WriteIndented = true,
				};

				using (var fileWriter = File.AppendText(logsFilePath)) {
					fileWriter.WriteLine(JsonSerializer.Serialize(logEvent, typeof(LogEvent), options));
				}
			}
			catch (IOException ex) {
				MessageBox.Show($"Error accessing log file. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (SerializationException) {
				MessageBox.Show("Error serializing JSON object.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
