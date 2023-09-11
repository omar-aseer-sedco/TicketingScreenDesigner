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
			InitializeComponent();
			buttons = new List<TicketingButton>();
		}

		public ScreenEditor(BankForm callingForm, BankController bankController) : this() {
			isNewScreen = true;
			Text = TITLE_TEXT + " - New Screen";
			this.callingForm = callingForm;
			this.bankController = bankController;
			screenController = new ScreenController(bankController.BankName);
		}

		public ScreenEditor(BankForm callingForm, BankController bankController, TicketingScreen screen) : this(callingForm, bankController) {
			isNewScreen = false;
			Text = TITLE_TEXT + " - " + screen.ScreenTitle;
			screenController = new ScreenController(bankController.BankName, screen.ScreenId, screen.ScreenTitle);
			FillInfo(screen);
			UpdateListView();
		}

		private void FillInfo(TicketingScreen screen) {
			screenTitleTextBox.Text = screen.ScreenTitle;
			activeCheckBox.Checked = screen.IsActive;
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
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			Add();
		}

		private void AddNewScreen() {
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

		private void UpdateCurrentScreen() {
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

		private List<int> GetSelectedButtonIds() {
			var selectedButtonIds = new List<int>();
			foreach (ListViewItem button in buttonsListView.SelectedItems)
				selectedButtonIds.Add(((TicketingButton) button.Tag).ButtonId);

			return selectedButtonIds;
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

				screenController.DeleteButtonsCancellable(GetSelectedButtonIds());

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e) {
			Delete();
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
			UpdateFormButtonActivation();
		}

		public void CheckIfScreenExists() {
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

		private void Edit() {
			try {
				if (buttonsListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one button to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				TicketingButton button = (TicketingButton) buttonsListView.SelectedItems[0].Tag;

				bool? buttonExists = screenController.CheckIfButtonExists(button.ButtonId);

				if (buttonExists is null) {
					LogsHelper.Log("Failed to access button.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("Failed to access button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (!(bool) buttonExists) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var buttonEditor = new ButtonEditor(this, screenController, button);
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show($"Unhandled Error.\nType: {ex.GetType()}\nMessage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			Edit();
		}

		private bool IsInformationComplete() {
			TrimInput();
			return buttonsListView.Items.Count > 0 && screenTitleTextBox.Text != string.Empty;
		}

		private void refreshButton_Click(object sender, EventArgs e) {
			UpdateListView();
		}

		private void ToggleActive() {
			activeCheckBox.Checked = !activeCheckBox.Checked;
		}

		private void HandleKeyEvent(KeyEventArgs e) {
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

		private void ScreenEditor_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void buttonsListView_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void TrimInput() {
			screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();
		}
	}
}
