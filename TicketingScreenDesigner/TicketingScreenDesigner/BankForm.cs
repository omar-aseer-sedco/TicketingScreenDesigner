#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using BusinessLogicLayer;

namespace TicketingScreenDesigner {
	public partial class BankForm : Form {
		private const string TITLE_TEXT = "Ticketing Screen Designer";

		private readonly string bankName;
		private readonly BankController bankController;

		private List<TicketingScreen> screens;

		private BankForm() {
			InitializeComponent();
			bankName = string.Empty;
			screens = new List<TicketingScreen>();
		}

		public BankForm(string bankName) : this() {
			bankController = new BankController(bankName);
			this.bankName = bankName;
			Text = TITLE_TEXT + " - " + this.bankName;
			UpdateTitleLabel();
			UpdateListView();
		}

		public BankForm(string bankName, List<TicketingScreen> screens) : this(bankName) {
			this.screens = screens;
		}

		private void UpdateTitleLabel() {
			titleLabel.Text = TITLE_TEXT + " - " + bankName;
			int sizeDifference = ClientSize.Width - titleLabel.Width;
			titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
		}

		private void Add() {
			var screenEditor = new ScreenEditor(this, bankController);
			screenEditor.ShowDialog();
		}

		private void addScreenButton_Click(object sender, EventArgs e) {
			Add();
		}

		private void SyncScreens() {
			screens = bankController.GetScreens();
		}

		public void UpdateListView() {
			try {
				screensListView.Items.Clear();

				SyncScreens();

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
			}
		}

		private void Edit() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = (int) screensListView.SelectedItems[0].Tag;

				if (!bankController.CheckIfScreenExists(screenId)) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				bool isActive = screensListView.SelectedItems[0].SubItems[ScreensConstants.IS_ACTIVE].Text == "Yes";
				var screen = new TicketingScreen(bankController.BankName, screenId, screenTitle, isActive);

				var screenEditor = new ScreenEditor(this, bankController, screen);
				screenEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void editScreenButton_Click(object sender, EventArgs e) {
			Edit();
		}

		private List<int> GetSelectedScreenIds() {
			var selectedScreenIds = new List<int>();
			foreach (ListViewItem row in screensListView.SelectedItems)
				selectedScreenIds.Add((int) row.Tag);

			return selectedScreenIds;
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

				bankController.DeleteScreens(GetSelectedScreenIds());

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void deleteScreenButton_Click(object sender, EventArgs e) {
			Delete();
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
			}
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateFormButtonActivation();
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

				bankController.ActivateScreen(selectedScreenId);
				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void setActiveButton_Click(object sender, EventArgs e) {
			SetActive();
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
			}
		}

		private void previewButton_Click(object sender, EventArgs e) {
			Preview();
		}

		private void PreviewScreenById(int screenId, string screenTitle) {
			try {
				if (!bankController.CheckIfScreenExists(screenId)) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var previewForm = new PreviewForm(screenTitle, bankController.GetButtons(screenId));
				previewForm.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void refreshButton_Click(object sender, EventArgs e) {
			UpdateListView();
		}

		private void HandleKeyEvent(KeyEventArgs e) {
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

		private void BankForm_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void screensListView_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e) {
			string shortcuts = "E: Edit\nD/Del/Backspace: Delete\nA: Add\nS: Set Active\nP: Preview\nR: Refresh";

			MessageBox.Show(shortcuts, "Keyboard shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Question);
		}
	}
}
