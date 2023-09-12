#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using BusinessLogicLayer;
using LogUtils;
using ExceptionUtils;

namespace TicketingScreenDesigner {
	public partial class BankForm : Form {
		private const string TITLE_TEXT = "Ticketing Screen Designer";

		private readonly string bankName;
		private readonly BankController bankController;

		private List<TicketingScreen> screens;

		private BankForm() {
			try {
				InitializeComponent();
				bankName = string.Empty;
				screens = new List<TicketingScreen>();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public BankForm(string bankName) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				bankController = new BankController(out bool success, bankName);
				if (!success) {
					LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("bankform Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Cursor.Current = Cursors.Default;
					Close();
				}

				this.bankName = bankName;
				Text = TITLE_TEXT + " - " + this.bankName;
				UpdateTitleLabel();
				UpdateListView();

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public BankForm(string bankName, List<TicketingScreen> screens) : this(bankName) {
			try {
				this.screens = screens;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateTitleLabel() {
			try {
				titleLabel.Text = TITLE_TEXT + " - " + bankName;
				int sizeDifference = ClientSize.Width - titleLabel.Width;
				titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Add() {
			try {
				var screenEditor = new ScreenEditor(this, bankController);
				if (!screenEditor.IsDisposed)
					screenEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void addScreenButton_Click(object sender, EventArgs e) {
			try {
				Add();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void UpdateListView() {
			try {
				List<TicketingScreen>? bankScreens = bankController.GetScreens();

				if (bankScreens is null) {
					LogsHelper.Log("Failed to sync screens.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to sync with database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				screens = bankScreens;

				screensListView.Items.Clear();

				foreach (var screen in screens) {
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

				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Edit() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = (int) screensListView.SelectedItems[0].Tag;

				bool? screenExists = bankController.CheckIfScreenExists(screenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				bool isActive = screensListView.SelectedItems[0].SubItems[ScreensConstants.IS_ACTIVE].Text == "Yes";
				var screen = new TicketingScreen(bankController.BankName, screenId, screenTitle, isActive);

				var screenEditor = new ScreenEditor(this, bankController, screen);
				if (!screenEditor.IsDisposed)
					screenEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void editScreenButton_Click(object sender, EventArgs e) {
			try {
				Edit();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}

		private void Delete() {
			try {
				if (screensListView.SelectedItems.Count == 0) {
					MessageBox.Show("Select a screen to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected screen(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (confirmationResult == DialogResult.No)
					return;

				if (!bankController.DeleteScreens(GetSelectedScreenIds() ?? new List<int>())) {
					LogsHelper.Log("Failed to delete screens.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to delete screen(s).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void deleteScreenButton_Click(object sender, EventArgs e) {
			try {
				Delete();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetActive() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to activate.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int selectedScreenId = (int) screensListView.SelectedItems[0].Tag;
				int? currentlyActiveScreenId = bankController.GetActiveScreenId();

				if (currentlyActiveScreenId != null) {
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

				if (!bankController.ActivateScreen(selectedScreenId)) {
					LogsHelper.Log("Failed to activate screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to activate screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void setActiveButton_Click(object sender, EventArgs e) {
			try {
				SetActive();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Preview() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to preview.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = (int) screensListView.SelectedItems[0].Tag;
				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				PreviewScreenById(screenId, screenTitle);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void previewButton_Click(object sender, EventArgs e) {
			try {
				Preview();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void PreviewScreenById(int screenId, string screenTitle) {
			try {
				bool? screenExists = bankController.CheckIfScreenExists(screenId);

				if (screenExists is null) {
					LogsHelper.Log("Failed to access screen.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access screen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				List<TicketingButton>? buttons = bankController.GetButtons(screenId);

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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void refreshButton_Click(object sender, EventArgs e) {
			try {
				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void HandleKeyEvent(KeyEventArgs e) {
			try {
				switch (e.KeyCode) {
					case Keys.Enter:
					case Keys.E:
						Edit();
						break;
					case Keys.Delete:
					case Keys.Back:
					case Keys.D:
						Delete();
						break;
					case Keys.A:
						Add();
						break;
					case Keys.S:
						SetActive();
						break;
					case Keys.P:
						Preview();
						break;
					case Keys.R:
						UpdateListView();
						break;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BankForm_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void screensListView_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				string shortcuts = "E: Edit\nD/Del/Backspace: Delete\nA: Add\nS: Set Active\nP: Preview\nR: Refresh";

				MessageBox.Show(shortcuts, "Keyboard shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Question);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
