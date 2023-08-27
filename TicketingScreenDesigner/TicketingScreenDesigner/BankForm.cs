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
			titleLabel.Text = titleText + " - " + bankName;
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
			string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} VALUES (@bankName, @screenName, @isActive, @ScreenTitle);";
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

			string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
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
						Name = ScreensConstants.SCREEN_ID,
						Text = reader[ScreensConstants.SCREEN_ID].ToString()
					};

					ListViewItem.ListViewSubItem isActive = new()
					{
						Name = ScreensConstants.IS_ACTIVE,
						Text = (bool) reader[ScreensConstants.IS_ACTIVE] ? "Yes" : "No"
					};
					row.SubItems.Add(isActive);

					ListViewItem.ListViewSubItem screenTitle = new()
					{
						Name = ScreensConstants.SCREEN_TITLE,
						Text = reader[ScreensConstants.SCREEN_TITLE].ToString()
					};
					row.SubItems.Add(screenTitle);

					screensListView.Items.Add(row);
				}

				reader.Close();
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - UpdateListView");
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

			var query = new StringBuilder($"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} IN (");
			SqlCommand command = new SqlCommand();
			command.Parameters.AddWithValue("@bankName", bankName);

			int i = 0;
			foreach (ListViewItem row in screensListView.SelectedItems)
			{
				query.Append("@P").Append(i).Append(',');
				command.Parameters.Add("@P" + i, SqlDbType.VarChar).Value = row.SubItems[ScreensConstants.SCREEN_ID].Text;
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
