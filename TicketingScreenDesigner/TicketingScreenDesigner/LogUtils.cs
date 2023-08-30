using System.Runtime.Serialization;
using System.Text.Json;

namespace TicketingScreenDesigner {
	public class LogEvent {
		public LogEvent(string message, DateTime timeStamp, EventSeverity severity) {
			Message = message;
			TimeStamp = timeStamp;
			Severity = GetSeverity(severity);
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
	}

	public enum EventSeverity {
		Error,
		Warning,
		Info,
	}

	public static class LogsHelper {
		private const string LOG_FILE_NAME = "logs.txt";
		private static readonly string logFilePath = Path.Join(Directory.GetCurrentDirectory(), "system logs", LOG_FILE_NAME);

		public static void Log(LogEvent logEvent) {
			WriteLog(logEvent);
		}

		private static void WriteLog(LogEvent logEvent) {
			try {
				var options = new FileStreamOptions() {
					Mode = FileMode.OpenOrCreate,
					Access = FileAccess.Write,
				};

				using (var fileWriter = File.AppendText(logFilePath)) {
					fileWriter.WriteLine(JsonSerializer.Serialize(logEvent));
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
