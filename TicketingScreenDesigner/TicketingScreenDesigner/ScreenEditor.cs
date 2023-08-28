using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketingScreenDesigner
{
	public partial class ScreenEditor : Form
	{
		private const string titleText = "Screen Editor";
		private readonly string bankName;
		private readonly string screenId;
		private readonly SqlConnection connection;
		private readonly List<TicketingButton> pendingButtons;
		private readonly List<string> deleteList;
		private readonly BankForm callingForm;
		private readonly bool newScreen;

		private ScreenEditor()
		{
			InitializeComponent();
			bankName = "";
			screenId = "";
			connection = dbUtils.CreateConnection();
			pendingButtons = new List<TicketingButton>();
			deleteList = new List<string>();
		}

		public ScreenEditor(BankForm callingForm, string bankName) : this()
		{
			this.callingForm = callingForm;
			this.bankName = bankName;
			Text = titleText + " - New Screen";
			newScreen = true;
		}

		public ScreenEditor(BankForm callingForm, string bankName, string screenId, string screenTitle, bool isActive) : this(callingForm, bankName)
		{
			this.screenId = screenId;
			screenIdTextBox.Text = screenId;
			screenTitleTextBox.Text = screenTitle;
			activeCheckBox.Checked = isActive;
			Text = titleText + " - " + screenId;
			newScreen = false;
			UpdateListView();
		}

		private void UpdateListView()
		{
			buttonsListView.Items.Clear();

			string query = $"SELECT {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN} " +
				$"FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenId);

			try
			{
				connection.Open();

				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					AddToListView(reader[ButtonsConstants.BUTTON_ID].ToString() ?? "Error", reader[ButtonsConstants.NAME_EN].ToString() ?? "Error", reader[ButtonsConstants.TYPE].ToString() ?? "Error",
						reader[ButtonsConstants.SERVICE].ToString(), reader[ButtonsConstants.MESSAGE_EN].ToString());
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

			foreach (var button in pendingButtons)
			{
				AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
			}
		}

		private void AddToListView(string buttonId, string nameEn, string type, string? service, string? messageEn)
		{
			if (deleteList.Contains(buttonId))
			{
				return;
			}

			ListViewItem row = new()
			{
				Name = ButtonsConstants.BUTTON_ID,
				Text = buttonId
			};

			ListViewItem.ListViewSubItem buttonNameEn = new()
			{
				Name = ButtonsConstants.NAME_EN,
				Text = nameEn
			};
			row.SubItems.Add(buttonNameEn);

			ListViewItem.ListViewSubItem buttonType = new()
			{
				Name = ButtonsConstants.TYPE,
				Text = type
			};
			row.SubItems.Add(buttonType);

			ListViewItem.ListViewSubItem buttonService = new()
			{
				Name = ButtonsConstants.SERVICE,
				Text = service ?? string.Empty
			};
			row.SubItems.Add(buttonService);

			ListViewItem.ListViewSubItem buttonMessageEn = new()
			{
				Name = ButtonsConstants.MESSAGE_EN,
				Text = messageEn ?? string.Empty
			};
			row.SubItems.Add(buttonMessageEn);

			buttonsListView.Items.Add(row);
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			Random random = new();

			var button = new TicketingButton(bankName, screenIdTextBox.Text, $"Button{random.Next(0, 65536)}", "Issue Ticket", "Some Button", "كبسة", "Some Service", null, null);
			pendingButtons.Add(button);
			AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
		}

		private bool AddScreen()
		{
			bool success = false;

			string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME}, {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE}) VALUES (@bankName, @screenId, @isActive, @screenTitle);";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenIdTextBox.Text);
			command.Parameters.AddWithValue("@isActive", 0);
			command.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text != string.Empty ? screenTitleTextBox.Text : DBNull.Value);

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

		private bool AddNewScreen()
		{
			bool addScreenSuccess = AddScreen();
			bool activationSuccess = addScreenSuccess && UpdateScreenActivation();
			bool addButtonsSuccess = addScreenSuccess && AddPendingButtons();

			return addScreenSuccess & activationSuccess & addButtonsSuccess;
		}

		private bool UpdateScreenInformation()
		{
			bool success = false;

			string updateScreenQuery = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_ID} = @newScreenId, {ScreensConstants.SCREEN_TITLE} = @screenTitle " +
				$"WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
			var updateScreenCommand = new SqlCommand(updateScreenQuery, connection);
			updateScreenCommand.Parameters.AddWithValue("@newScreenId", screenIdTextBox.Text);
			updateScreenCommand.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text);
			updateScreenCommand.Parameters.AddWithValue("@screenId", screenId);
			updateScreenCommand.Parameters.AddWithValue("@bankName", bankName);

			try
			{
				connection.Open();
				success = updateScreenCommand.ExecuteNonQuery() == 1;
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - UpdateCurrentScreen");
			}
			finally
			{
				connection.Close();
			}

			return success;
		}

		private bool UpdateScreenActivation()
		{
			bool success;

			if (activeCheckBox.Checked)
			{
				success = callingForm.ActivateScreen(screenIdTextBox.Text);
			}
			else
			{
				success = callingForm.DeactivateScreen(screenIdTextBox.Text);
			}

			return success;
		}

		private SqlCommand? CreateAddButtonsCommand()
		{
			if (pendingButtons.Count == 0)
				return null;

			var query = new StringBuilder($"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.TYPE}, {ButtonsConstants.NAME_EN}, " +
				$"{ButtonsConstants.NAME_AR}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES ");
			var command = new SqlCommand();

			int i = 0;
			foreach (var b in pendingButtons)
			{
				query.Append('(');
				query.Append("@bankName").Append(i).Append(',');
				query.Append("@screenId").Append(i).Append(',');
				query.Append("@buttonId").Append(i).Append(',');
				query.Append("@type").Append(i).Append(',');
				query.Append("@nameEn").Append(i).Append(',');
				query.Append("@nameAr").Append(i).Append(',');
				query.Append("@service").Append(i).Append(',');
				query.Append("@messageEn").Append(i).Append(',');
				query.Append("@messageAr").Append(i);
				query.Append("),");

				command.Parameters.Add("@bankName" + i, SqlDbType.VarChar).Value = b.BankName;
				command.Parameters.Add("@screenId" + i, SqlDbType.VarChar).Value = b.ScreenId;
				command.Parameters.Add("@buttonId" + i, SqlDbType.VarChar).Value = b.ButtonId;
				command.Parameters.Add("@type" + i, SqlDbType.VarChar).Value = b.Type;
				command.Parameters.Add("@nameEn" + i, SqlDbType.VarChar).Value = b.NameEn;
				command.Parameters.Add("@nameAr" + i, SqlDbType.NVarChar).Value = b.NameAr;
				command.Parameters.Add("@service" + i, SqlDbType.VarChar).Value = b.Service;
				if (b.MessageEn is not null)
				{
					command.Parameters.Add("@messageEn" + i, SqlDbType.VarChar).Value = b.MessageEn;
				}
				else
				{
					command.Parameters.Add("@messageEn" + i, SqlDbType.VarChar).Value = DBNull.Value;
				}
				if (b.MessageAr is not null)
				{
					command.Parameters.Add("@messageAr" + i, SqlDbType.NVarChar).Value = b.MessageAr;
				}
				else
				{
					command.Parameters.Add("@messageAr" + i, SqlDbType.VarChar).Value = DBNull.Value;
				}

				++i;
			}

			query.Length--; // remove last

			command.Connection = connection;
			command.CommandText = query.ToString();

			return command;
		}

		private bool AddPendingButtons()
		{
			bool success = true;
			var addButtonsCommand = CreateAddButtonsCommand();

			if (addButtonsCommand is not null)
			{
				success = false;

				try
				{
					connection.Open();
					success = addButtonsCommand.ExecuteNonQuery() == pendingButtons.Count;
				}
				catch (Exception ex) // INCOMPLETE
				{
					MessageBox.Show(ex.Message, "Something went wrong XD - AddPendingButtons");
					MessageBox.Show(addButtonsCommand.CommandText, "Something went wrong XD - AddPendingButtons");
				}
				finally
				{
					connection.Close();
				}
			}

			if (success)
				pendingButtons.Clear();

			return success;
		}

		private bool UpdateCurrentScreen()
		{
			bool informationUpdateSuccess = UpdateScreenInformation();
			bool activationUpdateSuccess = UpdateScreenActivation();
			bool addButtonsSuccess = AddPendingButtons();

			return informationUpdateSuccess & activationUpdateSuccess & addButtonsSuccess;
		}

		private SqlCommand? CreateDeleteButtonsCommand()
		{
			if (deleteList.Count == 0)
				return null;

			var query = new StringBuilder($"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (");
			var command = new SqlCommand();
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenId);

			int i = 0;
			foreach (var b in deleteList)
			{
				query.Append("@buttonId").Append(i).Append(',');
				command.Parameters.AddWithValue("@buttonId" + i, b);
				++i;
			}

			query.Length--; // remove last
			query.Append(");");

			command.Connection = connection;
			command.CommandText = query.ToString();

			return command;
		}

		private bool ExecutePendingDeletes()
		{
			bool success = true;
			var command = CreateDeleteButtonsCommand();

			if (command is not null)
			{
				success = false;

				try
				{
					connection.Open();
					int tmp = command.ExecuteNonQuery();
					success = tmp == deleteList.Count;
				}
				catch (Exception ex) // INCOMPLETE
				{
					MessageBox.Show(ex.Message, "Something went wrong XD - AddPendingButtons");
				}
				finally
				{
					connection.Close();
				}
			}

			if (success)
				deleteList.Clear();

			return success;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			bool success;

			if (newScreen)
			{
				success = AddNewScreen();
			}
			else
			{
				success = UpdateCurrentScreen();
			}

			success &= ExecutePendingDeletes();

			if (success)
			{
				callingForm.UpdateListView();
				Close();
			}
			else
			{
				MessageBox.Show("Something went wrong", "broblem XD", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			if (pendingButtons.Count > 0)
			{
				var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No)
				{
					return;
				}
			}

			pendingButtons.Clear();
			Close();
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			int selectedCount = buttonsListView.SelectedItems.Count;

			if (selectedCount == 0)
			{
				MessageBox.Show("Select buttons to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected button(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmationResult == DialogResult.No)
			{
				return;
			}

			foreach (ListViewItem button in buttonsListView.SelectedItems)
			{
				bool inPendingList = false;
				foreach (var pendingButton in pendingButtons)
				{
					if (pendingButton.ButtonId == button.Text)
					{
						pendingButtons.Remove(pendingButton);
						inPendingList = true;
						break;
					}
				}

				if (!inPendingList)
				{
					deleteList.Add(button.Text);
				}
			}

			UpdateListView();
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedCount = buttonsListView.SelectedItems.Count;

			if (selectedCount == 0)
			{
				editButton.Enabled = false;
				deleteButton.Enabled = false;
			}
			else if (selectedCount == 1)
			{
				editButton.Enabled = true;
				deleteButton.Enabled = true;
			}
			else
			{
				editButton.Enabled = false;
				deleteButton.Enabled = true;
			}
		}
	}
}
