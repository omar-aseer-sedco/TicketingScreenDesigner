using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private readonly BankForm callingForm;

		private ScreenEditor()
		{
			InitializeComponent();
			bankName = "";
			screenId = "";
			connection = dbUtils.CreateConnection();
			pendingButtons = new List<TicketingButton>();
		}

		public ScreenEditor(BankForm callingForm, string bankName) : this()
		{
			this.callingForm = callingForm;
			this.bankName = bankName;
			Text = titleText + " - New Screen";
		}

		public ScreenEditor(BankForm callingForm, string bankName, string screenId, string screenTitle, bool isActive) : this(callingForm, bankName)
		{
			this.screenId = screenId;
			screenIdTextBox.Text = screenId;
			screenTitleTextBox.Text = screenTitle;
			activeCheckBox.Checked = isActive;
			Text = titleText + " - " + screenId;
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
					AddToListView(reader[ButtonsConstants.BUTTON_ID].ToString(), reader[ButtonsConstants.NAME_EN].ToString(), reader[ButtonsConstants.TYPE].ToString(),
						reader[ButtonsConstants.SERVICE].ToString(), reader[ButtonsConstants.MESSAGE_EN].ToString());
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

			foreach (var button in pendingButtons)
			{
				AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
			}
		}

		private void AddToListView(string buttonId, string nameEn, string type, string? service, string? messageEn)
		{
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

			var button = new TicketingButton("TestBank", "TestScreen", $"Button{random.Next(0, 65536)}", "Issue Ticket", "Some Button", "كبسة", "Some Service", null, null);
			pendingButtons.Add(button);
			AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_ID} = @newScreenId, {ScreensConstants.SCREEN_TITLE} = @screenTitle " +
				$"WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@newScreenId", screenIdTextBox.Text);
			command.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text);
			command.Parameters.AddWithValue("@screenId", screenId);
			command.Parameters.AddWithValue("@bankName", bankName);

			try
			{
				connection.Open();
				command.ExecuteNonQuery();
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - GetScreens");
			}
			finally
			{
				connection.Close();
			}

			if (activeCheckBox.Checked)
			{
				callingForm.ActivateScreen(screenIdTextBox.Text);
			}
			else
			{
				callingForm.DeactivateScreen(screenIdTextBox.Text);
			}

			callingForm.UpdateListView();
			Close();
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
	}
}
