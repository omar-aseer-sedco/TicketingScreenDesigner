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

namespace TicketingScreenDesigner {
	public partial class ButtonEditor : Form {
		private const int issueTicketIndex = 0;
		private const int showMessageIndex = 1;
		private const string titleText = "Button Editor";
		private const int defaultPanelPositionY = 131;
		private const int minimumHeightIssueTicket = 233;
		private const int minimumHeightShowMessage = 260;
		private readonly TicketingButton button;
		private readonly SqlConnection connection;
		private readonly ScreenEditor callingForm;
		private readonly bool isNewButton;

		private ButtonEditor() {
			InitializeComponent();
			connection = DBUtils.CreateConnection();
			button = new TicketingButton();
			callingForm = new ScreenEditor(new BankForm(button.BankName), button.BankName);
		}

		private ButtonEditor(ScreenEditor callingForm) : this() {
			this.callingForm = callingForm;
		}

		public ButtonEditor(ScreenEditor callingForm, string bankName, string screenId) : this(callingForm) {
			button.BankName = bankName;
			button.ScreenId = screenId;
			Text = titleText + " - New Button";
			isNewButton = true;
		}

		public ButtonEditor(ScreenEditor callingForm, string bankName, string screenId, string buttonId) : this(callingForm) {
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

		public ButtonEditor(ScreenEditor callingForm, TicketingButton button) : this(callingForm) {
			this.button = button;
			FillTextBoxes(button);
		}

		private TicketingButton? GetButtonById(string bankName, string screenId, string buttonId) {
			TicketingButton? ret = callingForm.GetPendingButtonById(buttonId);

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
			catch (Exception ex) { // INCOMPLETE
				MessageBox.Show(ex.Message, "Something went wrong XD - GetButtonById");
			}
			finally {
				connection.Close();
			}

			return ret;
		}

		private void FillTextBoxes(TicketingButton button) {
			buttonIdTextBox.Text = button.ButtonId;
			nameEnTextBox.Text = button.NameEn;
			nameArTextBox.Text = button.NameAr;
			typeComboBox.SelectedIndex = button.Type == ButtonsConstants.Types.ISSUE_TICKET ? issueTicketIndex : showMessageIndex;
			ShowTypeSpecificFields();
			messageEnTextBox.Text = button.MessageEn ?? string.Empty;
			messageArTextBox.Text = button.MessageAr ?? string.Empty;
			serviceTextBox.Text = button.Service ?? string.Empty;
		}

		private void ShowTypeSpecificFields() {
			if (typeComboBox.SelectedIndex == -1) {
				issueTicketPanel.Visible = issueTicketPanel.Enabled = false;
				showMessagePanel.Visible = showMessagePanel.Enabled = false;
			}
			else if (typeComboBox.SelectedIndex == issueTicketIndex) {
				issueTicketPanel.Visible = issueTicketPanel.Enabled = true;
				showMessagePanel.Visible = showMessagePanel.Enabled = false;

				issueTicketPanel.Location = new Point(issueTicketPanel.Location.X, defaultPanelPositionY);

				if (Size.Height < minimumHeightIssueTicket) {
					Size = new Size(Size.Width, minimumHeightIssueTicket);
				}
			}
			else if (typeComboBox.SelectedIndex == showMessageIndex) {
				issueTicketPanel.Visible = issueTicketPanel.Enabled = false;
				showMessagePanel.Visible = showMessagePanel.Enabled = true;

				showMessagePanel.Location = new Point(showMessagePanel.Location.X, defaultPanelPositionY);

				if (Size.Height < minimumHeightShowMessage) {
					Size = new Size(Size.Width, minimumHeightShowMessage);
				}
			}
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			ShowTypeSpecificFields();
		}

		private bool IsInformationComplete() {
			TrimInput();

			return buttonIdTextBox.Text != string.Empty && nameEnTextBox.Text != string.Empty && nameArTextBox.Text != string.Empty && typeComboBox.SelectedIndex != -1 &&
				((typeComboBox.SelectedIndex == issueTicketIndex && serviceTextBox.Text != string.Empty) || (typeComboBox.SelectedIndex == showMessageIndex && messageEnTextBox.Text != string.Empty && messageArTextBox.Text != string.Empty));
		}

		private void saveButton_Click(object sender, EventArgs e) {
			TrimInput();

			if (!IsInformationComplete()) {
				MessageBox.Show("Please fill in all the fields before saving the button.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string bankName = button.BankName;
			string screenId = button.ScreenId;
			string buttonId = buttonIdTextBox.Text;
			string type = typeComboBox.SelectedIndex == issueTicketIndex ? ButtonsConstants.Types.ISSUE_TICKET : ButtonsConstants.Types.SHOW_MESSAGE;
			string nameEn = nameEnTextBox.Text;
			string nameAr = nameEnTextBox.Text;
			string? service = serviceTextBox.Text == string.Empty ? null : serviceTextBox.Text;
			string? messageEn = messageEnTextBox.Text == string.Empty ? null : messageEnTextBox.Text;
			string? messageAr = messageArTextBox.Text == string.Empty ? null : messageArTextBox.Text;
			var newButton = new TicketingButton(bankName, screenId, buttonId, type, nameEn, nameAr, service, messageEn, messageAr);

			if (isNewButton) {
				callingForm.AddButton(newButton);
			}
			else {
				callingForm.UpdateButton(button.ButtonId, newButton);
			}

			callingForm.UpdateListView();
			Close();
		}

		private bool IsDataChanged() {
			TrimInput();

			return (isNewButton && (buttonIdTextBox.Text != string.Empty || nameEnTextBox.Text != string.Empty || nameArTextBox.Text != string.Empty || typeComboBox.SelectedIndex != -1)) ||
				(!isNewButton && (buttonIdTextBox.Text != button.ButtonId || nameEnTextBox.Text != button.NameEn || nameArTextBox.Text != button.NameAr ||
				typeComboBox.SelectedIndex != (button.Type == ButtonsConstants.Types.ISSUE_TICKET ? issueTicketIndex : showMessageIndex) || (typeComboBox.SelectedIndex == issueTicketIndex && serviceTextBox.Text != button.Service) ||
				(typeComboBox.SelectedIndex == showMessageIndex && (messageEnTextBox.Text != button.MessageEn || messageArTextBox.Text != button.MessageAr))));
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			if (IsDataChanged()) {
				var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No) {
					return;
				}
			}

			Close();
		}

		private void TrimInput() {
			buttonIdTextBox.Text = buttonIdTextBox.Text.Trim();
		}
	}
}