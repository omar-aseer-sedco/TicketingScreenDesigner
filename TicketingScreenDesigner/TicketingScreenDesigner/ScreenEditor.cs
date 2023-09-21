#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using BusinessLogicLayer;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using LogUtils;
using ExceptionUtils;
using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		private const string TITLE_TEXT = "Screen Editor";

		private readonly bool isNewScreen;
		private readonly BankForm callingForm;
		private readonly string bankName;
		private readonly ScreenController screenController;
		private readonly ButtonChangesListener buttonChangesListener;
		private List<TicketingButton> buttons;
		private bool alreadyAdded;

		private enum StatusLabelStates {
			UPDATING,
			UP_TO_DATE,
			ERROR
		}

		private ScreenEditor() {
			try {
				InitializeComponent();
				buttons = new List<TicketingButton>();
				alreadyAdded = false;
				HandleCreated += ScreenEditor_HandleCreated;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(BankForm callingForm, string bankName) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out bool success, bankName);
				if (!success) {
					LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("screeneditor Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Cursor.Current = Cursors.Default;
					Close();
				}

				isNewScreen = true;
				Text = TITLE_TEXT + " - New Screen";
				this.callingForm = callingForm;
				this.bankName = bankName;

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(BankForm callingForm, string bankName, TicketingScreen screen) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out bool success, bankName, screen.ScreenId, screen.ScreenTitle);
				if (!success) {
					LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Cursor.Current = Cursors.Default;
					Close();
				}

				isNewScreen = false;
				Text = TITLE_TEXT + " - " + screen.ScreenTitle;
				this.callingForm = callingForm;
				this.bankName = bankName;
				FillInfo(screen);

				buttonChangesListener = new ButtonChangesListener(this.bankName, screenController.ScreenId);
				buttonChangesListener.SubscribeToDelegate(RefreshOnChange);
				buttonChangesListener.Start();

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void RefreshOnChange(SqlNotificationInfo info) {
			if (info == SqlNotificationInfo.Insert || info == SqlNotificationInfo.Update || info == SqlNotificationInfo.Delete) {
				await UpdateButtonsListViewAsync();
			}
		}

		private async void ScreenEditor_HandleCreated(object? sender, EventArgs e) {
			await UpdateButtonsListViewAsync();
		}

		private void FillInfo(TicketingScreen screen) {
			try {
				screenTitleTextBox.Text = screen.ScreenTitle;
				activeCheckBox.Checked = screen.IsActive;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public async Task UpdateButtonsListViewAsync() {
			try {
				Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UPDATING)));

				List<TicketingButton>? screenButtons = await screenController.GetAllButtonsAsync();

				if (screenButtons is null) {
					LogsHelper.Log("Failed to sync buttons.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to sync with database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				buttons = screenButtons;

				Invoke(new MethodInvoker(() => AddButtonsToListView(buttons)));

				Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UP_TO_DATE)));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.ERROR)));
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddButtonsToListView(List<TicketingButton> buttonsToAdd) {
			buttonsListView.Items.Clear();

			foreach (var button in buttonsToAdd) {
				AddButtonToListView(button);
			}
		}

		private void UpdateStatusLabel(StatusLabelStates state) {
			switch (state) {
				case StatusLabelStates.UPDATING:
					statusLabel.Text = "Updating...";
					break;
				case StatusLabelStates.UP_TO_DATE:
					statusLabel.Text = $"Last update was {DateTime.Now.ToShortTimeString()}";
					break;
				case StatusLabelStates.ERROR:
					statusLabel.Text = "An error has occurred.";
					break;
			}
		}

		private void AddButtonToListView(TicketingButton button) {
			try {
				ListViewItem row = new() {
					Name = ButtonsConstants.NAME_EN,
					Text = button.NameEn
				};

				ListViewItem.ListViewSubItem buttonType = new() {
					Name = ButtonsConstants.TYPE,
					Text = button.Type == ButtonsConstants.Types.ISSUE_TICKET ? "Issue Ticket" : "Show Message"
				};
				row.SubItems.Add(buttonType);

				ListViewItem.ListViewSubItem buttonService = new() {
					Name = ButtonsConstants.SERVICE,
					Text = button is IssueTicketButton issueTicketButton ? issueTicketButton.Service : string.Empty
				};
				row.SubItems.Add(buttonService);

				ListViewItem.ListViewSubItem buttonMessageEn = new() {
					Name = ButtonsConstants.MESSAGE_EN,
					Text = button is ShowMessageButton showMessageButton ? showMessageButton.MessageEn : string.Empty
				};
				row.SubItems.Add(buttonMessageEn);

				row.Tag = button;

				buttonsListView.Items.Add(row);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddButton() {
			try {
				var buttonEditor = new ButtonEditor(this, screenController);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			try {
				AddButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddNewScreen() {
			try {
				int? screenId = BankController.AddScreen(new TicketingScreen() {
					BankName = bankName,
					ScreenTitle = screenTitleTextBox.Text,
					IsActive = activeCheckBox.Checked,
				});

				if (screenId is null) {
					LogsHelper.Log("Failed to add screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to add screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				screenController.ScreenId = (int) screenId;
				alreadyAdded = true;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateCurrentScreen() {
			try {
				bool success = BankController.UpdateScreen(bankName, screenController.ScreenId, new TicketingScreen() {
					BankName = bankName,
					ScreenTitle = screenTitleTextBox.Text,
					IsActive = activeCheckBox.Checked,
				});

				if (!success) {
					LogsHelper.Log("Failed to update screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to update screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void saveButton_Click(object sender, EventArgs e) {
			try {
				TrimInput();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in the screen title and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (isNewScreen) {
					if (!alreadyAdded) {
						AddNewScreen();
					}
				}
				else {
					bool? screenExists = BankController.CheckIfScreenExists(bankName, screenController.ScreenId);

					if (screenExists is null) {
						LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if ((bool) screenExists) {
						UpdateCurrentScreen();
					}
					else {
						MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						await callingForm.UpdateScreensListViewAsync();
						Close();
						return;
					}
				}

				if (!screenController.CommitPendingChanges(out List<int>? failedIds)) {
					if (failedIds is not null) {
						string fails = string.Empty;
						foreach (int id in failedIds) {
							fails += id;
						}

						foreach (int id in failedIds) {
							foreach (ListViewItem item in buttonsListView.Items) {
								if (((TicketingButton) item.Tag).ButtonId == id) {
									item.BackColor = Color.PaleVioletRed;
									break;
								}
							}
						}
					}

					LogsHelper.Log("Failed to commit changes.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to commit changes. Please check for errors.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				await callingForm.UpdateScreensListViewAsync();

				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsDataChanged() {
			TrimInput();
			return screenController.GetPendingChangeCount() != 0 || (isNewScreen && screenTitleTextBox.Text != string.Empty) || (!isNewScreen && screenTitleTextBox.Text != screenController.ScreenTitle);
		}

		private async void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				if (isNewScreen && alreadyAdded) {
					BankController.DeleteScreens(bankName, new List<int>() { screenController.ScreenId });
				}

				screenController.CancelPendingChanges();
				await callingForm.UpdateScreensListViewAsync();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<int>? GetSelectedButtonIds() {
			try {
				var selectedButtonIds = new List<int>();
				foreach (ListViewItem button in buttonsListView.SelectedItems)
					selectedButtonIds.Add(((TicketingButton) button.Tag).ButtonId);

				return selectedButtonIds;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return null;
		}

		private async void DeleteButton() {
			try {
				int selectedCount = buttonsListView.SelectedItems.Count;

				if (selectedCount == 0) {
					MessageBox.Show("Select buttons to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected button(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No) {
					return;
				}

				screenController.DeleteButtonsCancellable(GetSelectedButtonIds() ?? new List<int>());

				await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e) {
			try {
				DeleteButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateFormButtonActivation() {
			try {
				int selectedCount = buttonsListView.SelectedItems.Count;

				if (selectedCount == 0) {
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
				else if (selectedCount == 1) {
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}
				else {
					editButton.Enabled = false;
					deleteButton.Enabled = true;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void CheckIfScreenExists() {
			try {
				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenController.ScreenId);
				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!isNewScreen && !(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					screenController.CancelPendingChanges();
					Close();
					return;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void EditButton() {
			try {
				if (buttonsListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one button to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				TicketingButton button = (TicketingButton) buttonsListView.SelectedItems[0].Tag;

				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenController.ScreenId);
				bool? buttonExists = screenController.CheckIfButtonExists(button.ButtonId);

				if (screenExists is null || buttonExists is null) {
					LogsHelper.Log("Failed to access button.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await callingForm.UpdateScreensListViewAsync();
					Close();
					return;
				}

				if (!(bool) buttonExists) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await UpdateButtonsListViewAsync();
					return;
				}

				var buttonEditor = new ButtonEditor(this, screenController, button);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			try {
				EditButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsInformationComplete() {
			try {
				TrimInput();
				return buttonsListView.Items.Count > 0 && screenTitleTextBox.Text != string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return false;
		}

		private async void refreshButton_Click(object sender, EventArgs e) {
			try {
				await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ToggleIsScreenActive() {
			try {
				activeCheckBox.Checked = !activeCheckBox.Checked;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void HandleKeyEvent(KeyEventArgs e) {
			try {
				switch (e.KeyCode) {
					case Keys.E:
						EditButton();
						break;
					case Keys.Delete:
					case Keys.Back:
					case Keys.D:
						DeleteButton();
						break;
					case Keys.A:
						AddButton();
						break;
					case Keys.R:
						await UpdateButtonsListViewAsync();
						break;
					case Keys.S:
						ToggleIsScreenActive();
						break;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ScreenEditor_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void TrimInput() {
			try {
				screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ScreenEditor_FormClosed(object sender, FormClosedEventArgs e) {
			buttonChangesListener.Stop();
		}
	}
}
