#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using LogUtils;
using ExceptionUtils;
using System.Data.SqlClient;
using BusinessLogicLayer.Listeners;
using BusinessLogicLayer.Controllers;

namespace TicketingScreenDesigner {
	public partial class BankForm : Form {
		private const string TITLE_TEXT = "Ticketing Screen Designer";

		private readonly string bankName;
		private readonly ScreenChangesListener screenChangesListener;

		private List<TicketingScreen> screens;

		private enum StatusLabelStates {
			UPDATING,
			UP_TO_DATE,
			ERROR
		}

		private BankForm() {
			try {
				InitializeComponent();
				screens = new List<TicketingScreen>();
				HandleCreated += BankForm_HandleCreated;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public BankForm(string bankName) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				var status = BankController.Initialize();

				switch (status) {
					case InitializationStatus.FAILED_TO_CONNECT:
						LogsHelper.Log("Error establishing database connection - BankForm.", DateTime.Now, EventSeverity.Error);
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
						LogsHelper.Log("Error establishing database connection - BankForm.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to establish database connection due to an unexpected error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Cursor.Current = Cursors.Default;
						Close();
						break;
				}

				this.bankName = bankName;
				Text = TITLE_TEXT + " - " + this.bankName;
				UpdateTitleLabel();

				screenChangesListener = new ScreenChangesListener(this.bankName);
				screenChangesListener.SubscribeToDelegate(RefreshOnChange);
				screenChangesListener.Start();

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public BankForm(string bankName, List<TicketingScreen> screens) : this(bankName) {
			try {
				this.screens = screens;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void RefreshOnChange(SqlNotificationInfo info) {
			if (info == SqlNotificationInfo.Insert || info == SqlNotificationInfo.Update || info == SqlNotificationInfo.Delete) {
				await UpdateScreensListViewAsync();
			}
		}

		private async void BankForm_HandleCreated(object? sender, EventArgs e) {
			await UpdateScreensListViewAsync();
		}

		private void UpdateTitleLabel() {
			try {
				titleLabel.Text = TITLE_TEXT + " - " + bankName;
				int sizeDifference = ClientSize.Width - titleLabel.Width;
				titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task AddScreen() {
			try {
				var screenEditor = new ScreenEditor(bankName);
				if (!screenEditor.IsDisposed)
					screenEditor.ShowDialog();
				await UpdateScreensListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void addScreenButton_Click(object sender, EventArgs e) {
			try {
				await AddScreen();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public async Task UpdateScreensListViewAsync() {
			try {
				if (!IsHandleCreated)
					return;

				Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UPDATING)));

				List<TicketingScreen>? bankScreens = await BankController.GetScreensAsync(bankName);

				if (bankScreens is null) {
					LogsHelper.Log("Failed to sync screens.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to sync with database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				screens = bankScreens;
				Invoke(new MethodInvoker(() => AddScreensToListView(screens)));

				Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.UP_TO_DATE)));

				Invoke(UpdateFormButtonActivation);
			}
			catch (ObjectDisposedException ex) {
				LogsHelper.Log(ex.Message, DateTime.Now, EventSeverity.Warning);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				if (IsHandleCreated)
					Invoke(new MethodInvoker(() => UpdateStatusLabel(StatusLabelStates.ERROR)));
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

		private void AddScreensToListView(List<TicketingScreen> screensToAdd) {
			screensListView.Items.Clear();

			foreach (var screen in screensToAdd) {
				ListViewItem row = new() {
					Name = ScreensConstants.SCREEN_TITLE,
					Text = screen.ScreenTitle
				};

				ListViewItem.ListViewSubItem isActive = new() {
					Name = ScreensConstants.IS_ACTIVE,
					Text = screen.IsActive ? "Yes" : "No"
				};
				row.SubItems.Add(isActive);

				row.Tag = screen.ScreenId;
				screensListView.Items.Add(row);
			}
		}

		private async Task EditScreen() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = (int) screensListView.SelectedItems[0].Tag;

				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await UpdateScreensListViewAsync();
					return;
				}

				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				bool isActive = screensListView.SelectedItems[0].SubItems[ScreensConstants.IS_ACTIVE].Text == "Yes";
				var screen = new TicketingScreen(bankName, screenId, screenTitle, isActive);

				var screenEditor = new ScreenEditor(bankName, screen);
				if (!screenEditor.IsDisposed)
					screenEditor.ShowDialog();
				await UpdateScreensListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void editScreenButton_Click(object sender, EventArgs e) {
			try {
				await EditScreen();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<int>? GetSelectedScreenIds() {
			try {
				var selectedScreenIds = new List<int>();
				foreach (ListViewItem row in screensListView.SelectedItems)
					selectedScreenIds.Add((int) row.Tag);

				return selectedScreenIds;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}

		private async Task DeleteScreen() {
			try {
				int selectedCount = screensListView.SelectedItems.Count;
				if (selectedCount == 0) {
					MessageBox.Show("Select a screen to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				string message;
				if (selectedCount == 1) {
					message = "Are you sure you want to delete the selected screen? This action cannot be undone.";
				}
				else {
					message = "Are you sure you want to delete the selected screens? This action cannot be undone.";
				}
				var confirmationResult = MessageBox.Show(message, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (confirmationResult == DialogResult.No)
					return;

				if (!BankController.DeleteScreens(bankName, GetSelectedScreenIds() ?? new List<int>())) {
					LogsHelper.Log("Failed to delete screens.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to delete screen(s).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				await UpdateScreensListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void deleteScreenButton_Click(object sender, EventArgs e) {
			try {
				await DeleteScreen();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateFormButtonActivation() {
			try {
				int selectedCount = screensListView.SelectedIndices.Count;
				if (selectedCount == 0) {
					editScreenButton.Enabled = false;
					deleteScreenButton.Enabled = false;
					setActiveButton.Enabled = false;
					previewButton.Enabled = false;
				}
				else if (selectedCount == 1) {
					editScreenButton.Enabled = true;
					deleteScreenButton.Enabled = true;
					setActiveButton.Enabled = true;
					previewButton.Enabled = true;
				}
				else {
					editScreenButton.Enabled = false;
					deleteScreenButton.Enabled = true;
					setActiveButton.Enabled = false;
					previewButton.Enabled = false;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task SetScreenToActive() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to activate.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int selectedScreenId = (int) screensListView.SelectedItems[0].Tag;
				int? currentlyActiveScreenId = BankController.GetActiveScreenId(bankName);

				if (currentlyActiveScreenId is not null && currentlyActiveScreenId != -1) {
					if (currentlyActiveScreenId == selectedScreenId) {
						MessageBox.Show("The selected screen is already active.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}
					else {
						var confirmationResult = MessageBox.Show("Setting this screen to active will deactivate the currently active screen. Do you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

						if (confirmationResult == DialogResult.No) {
							return;
						}
					}
				}

				if (!BankController.ActivateScreen(bankName, selectedScreenId)) {
					LogsHelper.Log("Failed to activate screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to activate screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				await UpdateScreensListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void setActiveButton_Click(object sender, EventArgs e) {
			try {
				await SetScreenToActive();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task PreviewScreen() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to preview.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = (int) screensListView.SelectedItems[0].Tag;
				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				await PreviewScreenById(screenId, screenTitle);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void previewButton_Click(object sender, EventArgs e) {
			try {
				await PreviewScreen();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task PreviewScreenById(int screenId, string screenTitle) {
			try {
				bool? screenExists = BankController.CheckIfScreenExists(bankName, screenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await UpdateScreensListViewAsync();
					return;
				}

				Cursor.Current = Cursors.WaitCursor;
				List<TicketingButton>? buttons = await BankController.GetButtons(bankName, screenId);
				Cursor.Current = Cursors.Default;

				if (buttons is null) {
					LogsHelper.Log("Failed to get buttons.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to get screen information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				var previewForm = new PreviewForm(screenTitle, buttons);
				previewForm.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void refreshButton_Click(object sender, EventArgs e) {
			try {
				await UpdateScreensListViewAsync();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void HandleKeyEvent(KeyEventArgs e) {
			try {
				switch (e.KeyCode) {
					case Keys.Enter:
					case Keys.E:
						await EditScreen();
						break;
					case Keys.Delete:
					case Keys.Back:
					case Keys.D:
						await DeleteScreen();
						break;
					case Keys.A:
						await AddScreen();
						break;
					case Keys.S:
						await SetScreenToActive();
						break;
					case Keys.P:
						await PreviewScreen();
						break;
					case Keys.R:
						await UpdateScreensListViewAsync();
						break;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BankForm_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void screensListView_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				string shortcuts = "E: Edit\nD/Del/Backspace: Delete\nA: Add\nS: Set Active\nP: Preview\nR: Refresh";

				MessageBox.Show(shortcuts, "Keyboard shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Question);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BankForm_FormClosed(object sender, FormClosedEventArgs e) {
			try {
				screenChangesListener.Stop();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				var changePasswordForm = new ChangePasswordForm(bankName);
				changePasswordForm.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void logOutToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
