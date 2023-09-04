#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public partial class ButtonEditor : Form {
		private const int ISSUE_TICKET_INDEX = 0;
		private const int SHOW_MESSAGE_INDEX = 1;
		private const string TITLE_TEXT = "Button Editor";
		private const int DEFAULT_PANEL_POSITION_Y = 95;
		private const int MINIMUM_HEIGHT_ISSUE_TICKET = 206; // 233
		private const int MINIMUM_HEIGHT_SHOW_MESSAGE = 233; // 260
		private const int MINIMUM_WIDTH = 628;
		private readonly TicketingButton button;
		private readonly SqlConnection connection;
		private readonly ScreenEditor callingForm;
		private readonly bool isNewButton;

		private ButtonEditor() {
			InitializeComponent();
			button = new TicketingButton();
		}

		private ButtonEditor(SqlConnection connection, ScreenEditor callingForm) : this() {
			this.connection = connection;
			this.callingForm = callingForm;
		}

		public ButtonEditor(SqlConnection connection, ScreenEditor callingForm, string bankName) : this(connection, callingForm) {
			button.BankName = bankName;
			Text = TITLE_TEXT + " - New Button";
			isNewButton = true;
		}

		public ButtonEditor(SqlConnection connection, ScreenEditor callingForm, string bankName, int screenId) : this(connection, callingForm, bankName) {
			button.ScreenId = screenId;
		}

		public ButtonEditor(SqlConnection connection, ScreenEditor callingForm, string bankName, int screenId, int buttonId) : this(connection, callingForm) {
			TicketingButton? button = GetButtonById(bankName, screenId, buttonId);

			if (button is null) {
				MessageBox.Show("This button does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}
			else {
				this.button = button;
				FillTextBoxes(button);
				isNewButton = false;
			}
		}

		public ButtonEditor(SqlConnection connection, ScreenEditor callingForm, TicketingButton button) : this(connection, callingForm) {
			this.button = button;
			FillTextBoxes(button);
		}

		private TicketingButton? GetButtonById(string bankName, int screenId, int buttonId) {
			TicketingButton? ret = null;

			try {
				ret = callingForm.GetPendingButtonById(buttonId);

				string query = $"SELECT {ButtonsConstants.TYPE}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR} FROM " +
					$"{ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				try {
					connection.Open();

					var reader = command.ExecuteReader();

					if (reader.Read()) {
						string type = reader[ButtonsConstants.TYPE].ToString() ?? $"Error getting {ButtonsConstants.TYPE}";
						string nameEn = reader[ButtonsConstants.NAME_EN].ToString() ?? $"Error getting {ButtonsConstants.NAME_EN}";
						string nameAr = reader[ButtonsConstants.NAME_AR].ToString() ?? $"Error getting {ButtonsConstants.NAME_AR}";
						string? service = reader[ButtonsConstants.SERVICE].ToString();
						string? messageEn = reader[ButtonsConstants.MESSAGE_EN].ToString();
						string? messageAr = reader[ButtonsConstants.MESSAGE_AR].ToString();

						ret = new TicketingButton(bankName, screenId, buttonId, type, nameEn, nameAr, service, messageEn, messageAr);
					}
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "screenId");
				}
				finally {
					connection.Close();
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return ret;
		}

		private void FillTextBoxes(TicketingButton button) {
			try {
				nameEnTextBox.Text = button.NameEn;
				nameArTextBox.Text = button.NameAr;
				typeComboBox.SelectedIndex = button.Type == ButtonsConstants.Types.ISSUE_TICKET ? ISSUE_TICKET_INDEX : SHOW_MESSAGE_INDEX;
				ShowTypeSpecificFields();
				messageEnTextBox.Text = button.MessageEn ?? string.Empty;
				messageArTextBox.Text = button.MessageAr ?? string.Empty;
				serviceTextBox.Text = button.Service ?? string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void ShowTypeSpecificFields() {
			try {
				if (typeComboBox.SelectedIndex == -1) {
					issueTicketPanel.Visible = issueTicketPanel.Enabled = false;
					showMessagePanel.Visible = showMessagePanel.Enabled = false;
				}
				else if (typeComboBox.SelectedIndex == ISSUE_TICKET_INDEX) {
					MinimumSize = new Size(MINIMUM_WIDTH, MINIMUM_HEIGHT_ISSUE_TICKET);

					issueTicketPanel.Visible = issueTicketPanel.Enabled = true;
					showMessagePanel.Visible = showMessagePanel.Enabled = false;

					issueTicketPanel.Location = new Point(issueTicketPanel.Location.X, DEFAULT_PANEL_POSITION_Y);

					if (Size.Height < MINIMUM_HEIGHT_ISSUE_TICKET) {
						Size = new Size(Size.Width, MINIMUM_HEIGHT_ISSUE_TICKET);
					}
				}
				else if (typeComboBox.SelectedIndex == SHOW_MESSAGE_INDEX) {
					MinimumSize = new Size(MINIMUM_WIDTH, MINIMUM_HEIGHT_SHOW_MESSAGE);

					issueTicketPanel.Visible = issueTicketPanel.Enabled = false;
					showMessagePanel.Visible = showMessagePanel.Enabled = true;

					showMessagePanel.Location = new Point(showMessagePanel.Location.X, DEFAULT_PANEL_POSITION_Y);

					if (Size.Height < MINIMUM_HEIGHT_SHOW_MESSAGE) {
						Size = new Size(Size.Width, MINIMUM_HEIGHT_SHOW_MESSAGE);
					}
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			ShowTypeSpecificFields();
		}

		private bool IsInformationComplete() {
			try {
				return nameEnTextBox.Text != string.Empty && nameArTextBox.Text != string.Empty && typeComboBox.SelectedIndex != -1 && ((typeComboBox.SelectedIndex == ISSUE_TICKET_INDEX && serviceTextBox.Text != string.Empty) ||
					(typeComboBox.SelectedIndex == SHOW_MESSAGE_INDEX && messageEnTextBox.Text != string.Empty && messageArTextBox.Text != string.Empty));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		private void saveButton_Click(object sender, EventArgs e) {
			try {
				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in all the fields before saving the button.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				string bankName = button.BankName;
				int screenId = button.ScreenId;
				string type = typeComboBox.SelectedIndex == ISSUE_TICKET_INDEX ? ButtonsConstants.Types.ISSUE_TICKET : ButtonsConstants.Types.SHOW_MESSAGE;
				string nameEn = nameEnTextBox.Text;
				string nameAr = nameArTextBox.Text;
				string? service = serviceTextBox.Text == string.Empty ? null : serviceTextBox.Text;
				string? messageEn = messageEnTextBox.Text == string.Empty ? null : messageEnTextBox.Text;
				string? messageAr = messageArTextBox.Text == string.Empty ? null : messageArTextBox.Text;
				var newButton = new TicketingButton(bankName, screenId, button.ButtonId, type, nameEn, nameAr, service, messageEn, messageAr);

				if (isNewButton) {
					callingForm.AddButtonToPendingList(newButton);
				}
				else {
					callingForm.UpdateButton(button.ButtonId, newButton);
				}

				callingForm.UpdateListView();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private bool IsDataChanged() {
			try {
				return (isNewButton && (nameEnTextBox.Text != string.Empty || nameArTextBox.Text != string.Empty || typeComboBox.SelectedIndex != -1)) || (!isNewButton && (nameEnTextBox.Text != button.NameEn || nameArTextBox.Text != button.NameAr ||
					typeComboBox.SelectedIndex != (button.Type == ButtonsConstants.Types.ISSUE_TICKET ? ISSUE_TICKET_INDEX : SHOW_MESSAGE_INDEX) || (typeComboBox.SelectedIndex == ISSUE_TICKET_INDEX && serviceTextBox.Text != button.Service) ||
					(typeComboBox.SelectedIndex == SHOW_MESSAGE_INDEX && (messageEnTextBox.Text != button.MessageEn || messageArTextBox.Text != button.MessageAr))));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}