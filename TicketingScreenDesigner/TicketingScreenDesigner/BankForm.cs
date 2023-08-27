using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TicketingScreenDesigner
{
	public partial class BankForm : Form
	{
		private const string titleText = "Ticketing Screen Designer";
		private readonly string bankName;
		private readonly SqlConnection connection;

		private BankForm()
		{
			InitializeComponent();
			bankName = "";
			connection = dbUtils.CreateConnection();
		}

		public BankForm(string bankName) : this()
		{
			this.bankName = bankName;
			Text = titleText + " - " + this.bankName;
			UpdateTitleLabel();
			UpdateListView();
		}

		private void UpdateTitleLabel()
		{
			titleLabel.Text = titleText + " - " + this.bankName;
			int sizeDifference = ClientSize.Width - titleLabel.Width;
			titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
		}

		private void addScreenButton_Click(object sender, EventArgs e)
		{
			Random random = new();
			AddScreen($"Screen{random.Next(0, 65536)}");

			UpdateListView();
		}

		private bool AddScreen(string screenName)
		{
			string query = "INSERT INTO Screens VALUES (@bankName, @screenName, @isActive, @ScreenTitle);";
			SqlCommand command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenName", screenName);
			command.Parameters.AddWithValue("@isActive", "0");
			command.Parameters.AddWithValue("@ScreenTitle", DBNull.Value);

			bool success = false;

			try
			{
				connection.Open();
				success = command.ExecuteNonQuery() == 1;
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - AddScreen");
			}
			finally
			{
				connection.Close();
			}

			return success;
		}

		private void UpdateListView()
		{
			screensListView.Items.Clear();

			string query = "SELECT screen_id, is_active, screen_title FROM Screens WHERE bank_name = @bankName;";
			SqlCommand command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			try
			{
				connection.Open();

				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					ListViewItem row = new()
					{
						Text = reader["screen_id"].ToString(),
						Name = "screen_id"
					};

					ListViewItem.ListViewSubItem isActive = new()
					{
						Name = "is_active",
						Text = (bool) reader["is_active"] ? "Yes" : "No"
					};
					row.SubItems.Add(isActive);

					ListViewItem.ListViewSubItem screenTitle = new()
					{
						Name = "screen_title",
						Text = reader["screen_title"].ToString()
					};
					row.SubItems.Add(screenTitle);

					screensListView.Items.Add(row);
				}

				reader.Close();
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - GetScreens");
			}
			finally
			{
				connection.Close();
			}
		}

		private void editScreenButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("This doesn't do anything either.", "Wow.");
		}

		private bool DeleteSelected()
		{
			int selectedCount = screensListView.SelectedItems.Count;

			var query = new StringBuilder("DELETE FROM Screens WHERE bank_name = @bankName AND screen_id IN (");
			SqlCommand command = new SqlCommand();
			command.Parameters.AddWithValue("@bankName", bankName);

			int i = 0;
			foreach (ListViewItem row in screensListView.SelectedItems)
			{
				query.Append("@P").Append(i).Append(',');
				command.Parameters.Add("@P" + i, SqlDbType.VarChar).Value = row.SubItems["screen_id"].Text;
				++i;
			}

			query.Length--; // remove last ,
			query.Append(");");

			command.Connection = connection;
			command.CommandText = query.ToString();

			bool success = false;

			try
			{
				connection.Open();
				success = command.ExecuteNonQuery() == selectedCount;
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - DeleteSelected");
			}
			finally
			{
				connection.Close();
			}

			return success;
		}

		private void deleteScreenButton_Click(object sender, EventArgs e)
		{
			var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected screen(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmationResult == DialogResult.No)
			{
				return;
			}

			if (DeleteSelected())
			{
				MessageBox.Show("Screens deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				MessageBox.Show("Screen deletion failed.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			UpdateListView();
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedCount = screensListView.SelectedIndices.Count;
			if (selectedCount == 0)
			{
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = false;
				setActiveButton.Enabled = false;
			}
			else if (selectedCount == 1)
			{
				editScreenButton.Enabled = true;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = true;
			}
			else
			{
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = false;
			}
		}
	}
}
