using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public partial class LoginForm : Form {
		private readonly SqlConnection connection;

		public LoginForm() {
			InitializeComponent();

			connection = Utils.CreateConnection();
		}

		private bool CheckIfExists(string bankName) {
			bool exists = false;

			string query = $"SELECT * FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			try {
				connection.Open();
				exists = command.ExecuteReader().HasRows;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screenId");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return exists;
		}

		private bool AddBank(string bankName) {
			string query = $"INSERT INTO {BanksConstants.TABLE_NAME} VALUES (@bankName);";
			SqlCommand command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			bool success = false;

			try {
				connection.Open();
				success = command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screenId");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return success;
		}

		private void LogInButton_Click(object sender, EventArgs e) {
			string bankName = bankNameTextBox.Text.Trim();

			if (string.IsNullOrEmpty(bankName)) {
				MessageBox.Show("Please enter the name of your bank.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!CheckIfExists(bankName)) {
				if (!AddBank(bankName)) { // INCOMPLETE
					MessageBox.Show($"Something went wrong.");
				}
			}

			BankForm bankForm = new BankForm(bankName);
			bankForm.Show();
		}
	}
}
