#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using BusinessLogicLayer;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using LogUtils;
using ExceptionUtils;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		private const string TITLE_TEXT = "Screen Editor";

		private readonly bool isNewScreen;
		private readonly BankForm callingForm;
		private readonly BankController bankController;
		private readonly ScreenController screenController;
		private List<TicketingButton> buttons;

		private ScreenEditor() {
			try {
				InitializeComponent();
				buttons = new List<TicketingButton>();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(BankForm callingForm, BankController bankController) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out bool success, bankController.BankName);
				if (!success) {
					LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("screeneditor Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Cursor.Current = Cursors.Default;
					Close();
				}

				isNewScreen = true;
				Text = TITLE_TEXT + " - New Screen";
				this.callingForm = callingForm;
				this.bankController = bankController;

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public ScreenEditor(BankForm callingForm, BankController bankController, TicketingScreen screen) : this() {
			try {
				Cursor.Current = Cursors.WaitCursor;

				screenController = new ScreenController(out bool success, bankController.BankName, screen.ScreenId, screen.ScreenTitle);
				if (!success) {
					LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Cursor.Current = Cursors.Default;
					Close();
				}

				isNewScreen = false;
				Text = TITLE_TEXT + " - " + screen.ScreenTitle;
				this.callingForm = callingForm;
				this.bankController = bankController;
				FillInfo(screen);
				UpdateListView();

				Cursor.Current = Cursors.Default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillInfo(TicketingScreen screen) {
			try {
				screenTitleTextBox.Text = screen.ScreenTitle;
				activeCheckBox.Checked = screen.IsActive;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void UpdateListView() {
			try {
				List<TicketingButton>? screenButtons = screenController.GetAllButtons();

				if (screenButtons is null) {
					LogsHelper.Log("Failed to sync buttons.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to sync with database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				buttons = screenButtons;

				buttonsListView.Items.Clear();

				foreach (var button in buttons) {
					AddToListView(button);
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddToListView(TicketingButton button) {
			try {
				ListViewItem row = new() {
					Name = ButtonsConstants.NAME_EN,
					Text = button.NameEn,
				};

				ListViewItem.ListViewSubItem buttonType = new() {
					Name = ButtonsConstants.TYPE,
					Text = button.Type
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Add() {
			try {
				var buttonEditor = new ButtonEditor(this, screenController);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			try {
				Add();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddNewScreen() {
			try {
				int? screenId = bankController.AddScreen(new TicketingScreen() {
					BankName = bankController.BankName,
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateCurrentScreen() {
			try {
				bool success = bankController.UpdateScreen(screenController.ScreenId, new TicketingScreen() {
					BankName = bankController.BankName,
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void saveButton_Click(object sender, EventArgs e) {
			try {
				TrimInput();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in the screen title and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (isNewScreen) {
					AddNewScreen();
				}
				else {
					bool? screenExists = bankController.CheckIfScreenExists(screenController.ScreenId);

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
						callingForm.UpdateListView();
						Close();
						return;
					}
				}

				if (!screenController.CommitPendingChanges()) {
					LogsHelper.Log("Failed to commit changes.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to commit changes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				callingForm.UpdateListView();

				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsDataChanged() {
			TrimInput();
			return screenController.GetPendingChangeCount() != 0 || (isNewScreen && screenTitleTextBox.Text != string.Empty) || (!isNewScreen && screenTitleTextBox.Text != screenController.ScreenTitle);
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				screenController.CancelPendingChanges();
				callingForm.UpdateListView();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return null;
		}

		private void Delete() {
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

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e) {
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void CheckIfScreenExists() {
			try {
				bool? screenExists = bankController.CheckIfScreenExists(screenController.ScreenId);
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
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Edit() {
			try {
				if (buttonsListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one button to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				TicketingButton button = (TicketingButton) buttonsListView.SelectedItems[0].Tag;

				bool? screenExists = bankController.CheckIfScreenExists(screenController.ScreenId);
				bool? buttonExists = screenController.CheckIfButtonExists(button.ButtonId);

				if (screenExists is null || buttonExists is null) {
					LogsHelper.Log("Failed to access button.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) screenExists) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
					callingForm.UpdateListView();
					Close();
					return;
				}

				if (!(bool) buttonExists) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var buttonEditor = new ButtonEditor(this, screenController, button);
				if (!buttonEditor.IsDisposed)
					buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			try {
				Edit();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool IsInformationComplete() {
			try {
				TrimInput();
				return buttonsListView.Items.Count > 0 && screenTitleTextBox.Text != string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return false;
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

		private void ToggleActive() {
			try {
				activeCheckBox.Checked = !activeCheckBox.Checked;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void HandleKeyEvent(KeyEventArgs e) {
			try {
				switch (e.KeyCode) {
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
					case Keys.R:
						UpdateListView();
						break;
					case Keys.S:
						ToggleActive();
						break;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ScreenEditor_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonsListView_KeyDown(object sender, KeyEventArgs e) {
			try {
				HandleKeyEvent(e);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void TrimInput() {
			try {
				screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
