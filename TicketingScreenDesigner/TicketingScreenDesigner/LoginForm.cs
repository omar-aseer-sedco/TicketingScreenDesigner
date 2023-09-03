using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public partial class LoginForm : Form {
		private readonly SqlConnection connection;

		public LoginForm() {
			InitializeComponent();
			LogsHelper.InitializeLogsDirectory();

			try {
				connection = DBUtils.CreateConnection();
			}
			catch (Exception) {
				Environment.Exit(1);
			}
		}

		private bool CheckIfExists(string bankName) {
			bool exists = false;

			try {
				string query = $"SELECT * FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				try {
					connection.Open();
					exists = command.ExecuteReader().HasRows;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Bank Name");
				}
				finally {
					connection.Close();
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return exists;
		}

		private bool AddBank(string bankName) {
			bool success = false;

			try {
				string query = $"INSERT INTO {BanksConstants.TABLE_NAME} VALUES (@bankName);";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				try {
					connection.Open();
					success = command.ExecuteNonQuery() == 1;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Bank Name");
				}
				finally {
					connection.Close();
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private void LogInButton_Click(object sender, EventArgs e) {
			try {
				string bankName = bankNameTextBox.Text.Trim();

				if (string.IsNullOrEmpty(bankName)) {
					MessageBox.Show("Please enter the name of your bank.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!CheckIfExists(bankName)) {
					var confirmationResult = MessageBox.Show("Your bank is not in the database. Add a new bank?", "Add a new bank", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

					if (confirmationResult == DialogResult.No)
						return;

					if (!AddBank(bankName))
						return;
				}

				var bankForm = new BankForm(connection, bankName);
				bankForm.Show();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}
