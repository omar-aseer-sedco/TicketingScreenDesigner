using System.Data.SqlClient;

namespace TicketingScreenDesigner
{
	public partial class LoginForm : Form
	{
		private readonly SqlConnection connection;

		public LoginForm()
		{
			InitializeComponent();

			connection = CreateConnection();
		}

		private static SqlConnection CreateConnection()
		{
			string connectionString = "server=(local);database=TSD;integrated security=sspi";
			return new SqlConnection(connectionString);
		}

		private bool CheckIfExists(string bankName)
		{
			bool exists = false;

			string queryString = $"SELECT * FROM Banks WHERE bank_name = @bankName";
			SqlCommand command = new SqlCommand(queryString, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			try
			{
				connection.Open();
				exists = command.ExecuteReader().HasRows;
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD");
			}
			finally
			{
				connection.Close();
			}

			return exists;
		}

		private bool CreateTable(string bankName)
		{
			string queryString = $"INSERT INTO Banks VALUES (@bankName)";
			SqlCommand command = new SqlCommand(queryString, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			bool success = false;

			try
			{
				connection.Open();
				success = command.ExecuteNonQuery() == 1;
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD");
			}
			finally
			{
				connection.Close();
			}

			return success;
		}

		private void LogInButton_Click(object sender, EventArgs e)
		{
			string bankName = bankNameTextBox.Text.Trim();

			if (string.IsNullOrEmpty(bankName))
			{
				MessageBox.Show("Please enter the name of your bank.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!CheckIfExists(bankName))
			{
				if (!CreateTable(bankName)) // INCOMPLETE
				{
					MessageBox.Show($"Something went wrong.");
				}
			}

			BankForm bankForm = new BankForm(bankName);
			bankForm.Show();
		}
	}
}