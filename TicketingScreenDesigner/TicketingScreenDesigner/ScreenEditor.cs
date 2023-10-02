#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using LogUtils;
using ExceptionUtils;
using System.Data.SqlClient;
using BusinessLogicLayer.Listeners;
using BusinessLogicLayer.Controllers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		public DialogResult Result { get; private set; }

		private const string TITLE_TEXT = "Screen Editor";

		private readonly string bankName;
		private readonly ScreenController screenController;
		private readonly ButtonChangesListener buttonChangesListener;
		private List<TicketingButton> buttons;

		private enum StatusLabelStates {
			UPDATING,
			UP_TO_DATE,
			ERROR
		}

		private ScreenEditor() {
			try {
				InitializeComponent();
				buttons = new List<TicketingButton>();
				HandleCreated += ScreenEditor_HandleCreated;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(string bankName) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out InitializationStatus status, bankName);

				switch (status) {
					case InitializationStatus.FAILED_TO_CONNECT:
						LogsHelper.Log("Error establishing database connection - ScreenEditor.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.FILE_CORRUPTED:
						LogsHelper.Log("Configuration file corrupted.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file is corrupted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.FILE_NOT_FOUND:
						LogsHelper.Log("Configration file not found", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.UNDEFINED_ERROR:
						LogsHelper.Log("Error establishing database connection - ScreenEditor.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to establish database connection due to an unexpected error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
				}

				Text = TITLE_TEXT + " - New Screen";
				this.bankName = bankName;

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(string bankName, TicketingScreen screen) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out InitializationStatus status, bankName, screen.ScreenId, screen.ScreenTitle);

				switch (status) {
					case InitializationStatus.FAILED_TO_CONNECT:
						LogsHelper.Log("Error establishing database connection - ScreenEditor.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.FILE_CORRUPTED:
						LogsHelper.Log("Configuration file corrupted.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file is corrupted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.FILE_NOT_FOUND:
						LogsHelper.Log("Configration file not found", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
					case InitializationStatus.UNDEFINED_ERROR:
						LogsHelper.Log("Error establishing database connection - ScreenEditor.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to establish database connection due to an unexpected error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
				}

				Text = TITLE_TEXT + " - " + screen.ScreenTitle;
				this.bankName = bankName;
				FillInfo(screen);

				buttonChangesListener = new ButtonChangesListener(this.bankName, screenController.ScreenId);
				buttonChangesListener.SubscribeToDelegate(RefreshOnChange);
				buttonChangesListener.Start();

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task UpdateButtonsListViewAsync() {
			try {
				if (!IsHandleCreated)
					return;

				BeginInvoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UPDATING)));

				List<TicketingButton>? screenButtons = await screenController.GetAllButtonsAsync();

				if (screenButtons is null) {
					LogsHelper.Log("Failed to sync buttons.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to sync with database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				buttons = screenButtons;

				BeginInvoke(new MethodInvoker(() => buttonsListView.Items.Clear()));

				//const int CHUNK_SIZE = 100;

				//for (int i = 0; i < buttons.Count; i += CHUNK_SIZE) {
				//	List<TicketingButton> chunk = buttons.GetRange(i, CHUNK_SIZE);

				//	await Task.Run(() => {
				//		BeginInvoke(new MethodInvoker(() => {
				//			foreach (var button in chunk) {
				//				ListViewItem row = new() {
				//					Name = ButtonsConstants.NAME_EN,
				//					Text = button.NameEn
				//				};

				//				ListViewItem.ListViewSubItem buttonType = new() {
				//					Name = ButtonsConstants.TYPE,
				//					Text = button.Type == ButtonsConstants.Types.ISSUE_TICKET ? "Issue Ticket" : "Show Message"
				//				};
				//				row.SubItems.Add(buttonType);

				//				ListViewItem.ListViewSubItem buttonService = new() {
				//					Name = ButtonsConstants.SERVICE,
				//					Text = button is IssueTicketButton issueTicketButton ? issueTicketButton.Service : string.Empty
				//				};
				//				row.SubItems.Add(buttonService);

				//				ListViewItem.ListViewSubItem buttonMessageEn = new() {
				//					Name = ButtonsConstants.MESSAGE_EN,
				//					Text = button is ShowMessageButton showMessageButton ? showMessageButton.MessageEn : string.Empty
				//				};
				//				row.SubItems.Add(buttonMessageEn);

				//				row.Tag = button;

				//				buttonsListView.Items.Add(row);
				//			}
				//		}));
				//	});
				//}

				List<ListViewItem> listViewItems = new();
				await Task.Run(() => {
					foreach (var button in buttons) {
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
						listViewItems.Add(row);
					}
				});
				BeginInvoke(new MethodInvoker(() => buttonsListView.Items.AddRange(listViewItems.ToArray())));

				BeginInvoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UP_TO_DATE)));
			}
			catch (ObjectDisposedException ex) {
				LogsHelper.Log(ex.Message, DateTime.Now, EventSeverity.Warning);
			}
			catch (InvalidOperationException ex) {
				LogsHelper.Log(ex.Message, DateTime.Now, EventSeverity.Warning);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				if (IsHandleCreated)
					BeginInvoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.ERROR)));
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private async void AddButton() {
			try {
				var buttonEditor = new ButtonEditor(screenController);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
				CheckIfScreenExists();
				if (buttonEditor.Result == DialogResult.OK)
					await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			try {
				AddButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void saveButton_Click(object sender, EventArgs e) {
			try {
				TrimInput();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in the screen title and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenController.ScreenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (screenController.ScreenId == -1) {
					if (!(bool) screenExists) {
						AddNewScreen();
					}
				}
				else {
					if ((bool) screenExists) {
						UpdateCurrentScreen();
					}
					else {
						MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

				Result = DialogResult.OK;
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsDataChanged() {
			TrimInput();
			return screenController.GetPendingChangeCount() != 0 || (screenController.ScreenId == -1 && screenTitleTextBox.Text != string.Empty) || (screenController.ScreenId != -1 && screenTitleTextBox.Text != screenController.ScreenTitle);
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenController.ScreenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (screenController.ScreenId == -1 && (bool) screenExists) {
					BankController.DeleteScreens(bankName, new List<int>() { screenController.ScreenId });
				}

				screenController.CancelPendingChanges();
				Result = DialogResult.Cancel;
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

				string message;
				if (selectedCount == 1) {
					message = "Are you sure you want to delete the selected button? This action cannot be undone.";
				}
				else {
					message = "Are you sure you want to delete the selected buttons? This action cannot be undone.";
				}

				var confirmationResult = MessageBox.Show(message, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No) {
					return;
				}

				screenController.DeleteButtonsCancellable(GetSelectedButtonIds() ?? new List<int>());

				await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e) {
			try {
				DeleteButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

				if (screenController.ScreenId != -1 && !(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					screenController.CancelPendingChanges();
					Close();
					return;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					Close();
					return;
				}

				if (!(bool) buttonExists) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await UpdateButtonsListViewAsync();
					return;
				}

				var buttonEditor = new ButtonEditor(screenController, button);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
				CheckIfScreenExists();
				if (buttonEditor.Result == DialogResult.OK)
					await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			try {
				EditButton();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsInformationComplete() {
			try {
				TrimInput();
				return buttonsListView.Items.Count > 0 && screenTitleTextBox.Text != string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return false;
		}

		private async void refreshButton_Click(object sender, EventArgs e) {
			try {
				await UpdateButtonsListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ToggleIsScreenActive() {
			try {
				activeCheckBox.Checked = !activeCheckBox.Checked;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ScreenEditor_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void TrimInput() {
			try {
				screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ScreenEditor_FormClosed(object sender, FormClosedEventArgs e) {
			buttonChangesListener?.Stop();
		}
	}
}
