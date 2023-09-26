#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.DataClasses;
using DataAccessLayer.Constants;
using LogUtils;
using ExceptionUtils;
using BusinessLogicLayer.Controllers;

namespace TicketingScreenDesigner {
	public partial class ButtonEditor : Form {
		private const string TITLE_TEXT = "Button Editor";
		private const int DEFAULT_PANEL_POSITION_Y = 95;
		private const int MINIMUM_HEIGHT_ISSUE_TICKET = 206;
		private const int MINIMUM_HEIGHT_SHOW_MESSAGE = 233;

		private readonly ScreenEditor callingForm;
		private readonly ScreenController screenController;
		private readonly TicketingButton button;
		private readonly bool isNewButton;

		private enum TypeIndex {
			ISSUE_TICKET = 0,
			SHOW_MESSAGE = 1,
		}

		private ButtonEditor() {
			try {
				InitializeComponent();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ButtonEditor(ScreenEditor callingForm, ScreenController screenController) : this() {
			try {
				Text = TITLE_TEXT + " - New Button";
				isNewButton = true;
				this.callingForm = callingForm;
				this.screenController = screenController;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ButtonEditor(ScreenEditor callingForm, ScreenController screenController, TicketingButton button) : this(callingForm, screenController) {
			try {
				Text = TITLE_TEXT + " - " + button.NameEn;
				isNewButton = false;
				this.button = button;
				FillTextBoxes(button);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillTextBoxes(TicketingButton button) {
			try {
				nameEnTextBox.Text = button.NameEn;
				nameArTextBox.Text = button.NameAr;
				if (button is IssueTicketButton issueTicketButton) {
					typeComboBox.SelectedIndex = (int) TypeIndex.ISSUE_TICKET;
					serviceTextBox.Text = issueTicketButton.Service;
				}
				else if (button is ShowMessageButton showMessageButton) {
					typeComboBox.SelectedIndex = (int) TypeIndex.SHOW_MESSAGE;
					messageEnTextBox.Text = showMessageButton.MessageEn;
					messageArTextBox.Text = showMessageButton.MessageAr;
				}
				ShowTypeSpecificFields();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ShowTypeSpecificFields() {
			try {
				if (typeComboBox.SelectedIndex == -1) {
					issueTicketPanel.Visible = issueTicketPanel.Enabled = false;
					showMessagePanel.Visible = showMessagePanel.Enabled = false;
				}
				else if (typeComboBox.SelectedIndex == (int) TypeIndex.ISSUE_TICKET) {
					MinimumSize = new Size(MinimumSize.Width, MINIMUM_HEIGHT_ISSUE_TICKET);

					issueTicketPanel.Visible = issueTicketPanel.Enabled = true;
					showMessagePanel.Visible = showMessagePanel.Enabled = false;

					issueTicketPanel.Location = new Point(issueTicketPanel.Location.X, DEFAULT_PANEL_POSITION_Y);

					if (Size.Height < MINIMUM_HEIGHT_ISSUE_TICKET) {
						Size = new Size(Size.Width, MINIMUM_HEIGHT_ISSUE_TICKET);
					}
				}
				else if (typeComboBox.SelectedIndex == (int) TypeIndex.SHOW_MESSAGE) {
					MinimumSize = new Size(MinimumSize.Width, MINIMUM_HEIGHT_SHOW_MESSAGE);

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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				ShowTypeSpecificFields();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsInformationComplete() {
			try {
				return nameEnTextBox.Text != string.Empty && nameArTextBox.Text != string.Empty && typeComboBox.SelectedIndex != -1 && ((typeComboBox.SelectedIndex == (int) TypeIndex.ISSUE_TICKET && serviceTextBox.Text != string.Empty) ||
					(typeComboBox.SelectedIndex == (int) TypeIndex.SHOW_MESSAGE && messageEnTextBox.Text != string.Empty && messageArTextBox.Text != string.Empty));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		private async void saveButton_Click(object sender, EventArgs e) {
			try {
				TrimInput();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in all the fields before saving the button.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!isNewButton) {
					bool? buttonExists = screenController.CheckIfButtonExists(button.ButtonId);

					if (buttonExists is null) {
						LogsHelper.Log("Failed to access button.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to access button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if (!(bool) buttonExists) {
						MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						await callingForm.UpdateButtonsListViewAsync();
						Close();
						return;
					}
				}

				string bankName = screenController.BankName;
				int screenId = screenController.ScreenId;
				ButtonsConstants.Types type = typeComboBox.SelectedIndex == (int) TypeIndex.ISSUE_TICKET ? ButtonsConstants.Types.ISSUE_TICKET : ButtonsConstants.Types.SHOW_MESSAGE;
				string nameEn = nameEnTextBox.Text;
				string nameAr = nameArTextBox.Text;

				TicketingButton newButton;
				if (type == ButtonsConstants.Types.ISSUE_TICKET) {
					string service = serviceTextBox.Text;
					newButton = new IssueTicketButton() {
						BankName = bankName,
						ScreenId = screenId,
						Type = type,
						NameEn = nameEn,
						NameAr = nameAr,
						Service = service,
					};
				}
				else if (type == ButtonsConstants.Types.SHOW_MESSAGE) {
					string messageEn = messageEnTextBox.Text;
					string messageAr = messageArTextBox.Text;
					newButton = new ShowMessageButton() {
						BankName = bankName,
						ScreenId = screenId,
						Type = type,
						NameEn = nameEn,
						NameAr = nameAr,
						MessageEn = messageEn,
						MessageAr = messageAr,
					};
				}
				else {
					return;
				}

				if (isNewButton) {
					newButton.ButtonId = screenController.GetNextPendingIndex();
					screenController.AddButtonCancellable(newButton);
				}
				else {
					screenController.UpdateButtonCancellable(button.ButtonId, newButton);
				}

				callingForm.CheckIfScreenExists();
				await callingForm.UpdateButtonsListViewAsync();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsDataChanged() {
			try {
				return (isNewButton && (nameEnTextBox.Text != string.Empty || nameArTextBox.Text != string.Empty || typeComboBox.SelectedIndex != -1)) || (!isNewButton && (nameEnTextBox.Text != button.NameEn || nameArTextBox.Text != button.NameAr ||
					typeComboBox.SelectedIndex != (button is IssueTicketButton ? (int) TypeIndex.ISSUE_TICKET : (int) TypeIndex.SHOW_MESSAGE) || (button is IssueTicketButton issueTicketButton && serviceTextBox.Text != issueTicketButton.Service) ||
					(button is ShowMessageButton showMessageButton && (messageEnTextBox.Text != showMessageButton.MessageEn || messageArTextBox.Text != showMessageButton.MessageAr))));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		private void TrimInput() {
			try {
				nameEnTextBox.Text = nameEnTextBox.Text.Trim();
				nameArTextBox.Text = nameArTextBox.Text.Trim();
				messageEnTextBox.Text = messageEnTextBox.Text.Trim();
				messageArTextBox.Text = messageArTextBox.Text.Trim();
				serviceTextBox.Text = serviceTextBox.Text.Trim();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
