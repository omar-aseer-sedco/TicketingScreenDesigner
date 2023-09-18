#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using ExceptionUtils;
using DataAccessLayer.DataClasses;

namespace TicketingScreenDesigner {
	public class PreviewForm : Form {
		private const int TOP_MARGIN = 20;
		private const int BOTTOM_MARGIN = 60;
		private const int SIDE_MARGIN = 60;

		private const int TITLE_TEXT_SPACING = 30;
		private const int VERTICAL_CORRECTION = 30;
		private const int HORIZONTAL_CORRECTION = 16;

		private const int BUTTON_HEIGHT = 50;
		private const int BUTTON_WIDTH = 120;
		private const int BUTTON_SPACING_VERTICAL = 30;
		private const int BUTTON_SPACING_HORIZONTAL = 50;

		private const int LANGUAGE_BUTTON_WIDTH = 40;
		private const int LANGUAGE_BUTTON_HEIGHT = 24;
		private const int LANGUAGE_BUTTON_POSITION_X = 5;
		private const int LANGUAGE_BUTTON_POSITION_Y = 5;

		private readonly List<TicketingButton> ticketingButtons;
		private readonly string titleText;
		private readonly int buttonCount;

		private List<Button> formButtons;
		private Label titleLabel;
		private Button languageButton;
		private int rows;
		private int columns;
		private string language;

		public PreviewForm(string screenTitle, List<TicketingButton> ticketingButtons) {
			try {
				titleText = screenTitle;
				this.ticketingButtons = ticketingButtons;
				StartPosition = FormStartPosition.CenterParent;
				buttonCount = ticketingButtons.Count;
				language = "en";
				InitializeComponent();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void InitializeComponent() {
			try {
				SuspendLayout();

				formButtons = new List<Button>();

				titleLabel = new Label {
					Text = titleText,
					Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point),
					TextAlign = ContentAlignment.MiddleCenter,
					Anchor = AnchorStyles.Top,
					Dock = DockStyle.Top,
					AutoEllipsis = true,
				};

				languageButton = new Button {
					Text = language.ToUpper(),
					Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
					TextAlign = ContentAlignment.MiddleCenter,
					Size = new Size(LANGUAGE_BUTTON_WIDTH, LANGUAGE_BUTTON_HEIGHT),
					Location = new Point(LANGUAGE_BUTTON_POSITION_X, LANGUAGE_BUTTON_POSITION_Y),
					Anchor = AnchorStyles.Top | AnchorStyles.Left,
				};
				languageButton.Click += SwitchLanguage;

				SetButtonGridDimensions();
				SetFormSize();
				MinimumSize = Size;

				Padding = new Padding(SIDE_MARGIN, TOP_MARGIN, SIDE_MARGIN, BOTTOM_MARGIN);
				titleLabel.Location = new Point((Size.Width - titleLabel.Width) / 2, TOP_MARGIN);

				int i = 0;
				foreach (var ticketingButton in ticketingButtons) {
					var button = new Button {
						Name = ticketingButton.ButtonId.ToString(),
						Text = ticketingButton.NameEn,
						Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
						Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
						Location = GetButtonLocation(i),
						Anchor = AnchorStyles.None,
						AutoEllipsis = true,
					};
					button.Click += Button_Click;

					formButtons.Add(button);

					++i;
				}

				Controls.AddRange(formButtons.ToArray<Control>());
				Controls.Add(titleLabel);
				Controls.Add(languageButton);

				ResumeLayout();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private static int GetNextPerfectSquareRoot(int x) {
			try {
				return (int) Math.Ceiling(Math.Sqrt(x));
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return default;
			}
		}

		private void SetButtonGridDimensions() {
			try {
				rows = GetNextPerfectSquareRoot(buttonCount);
				columns = (int) Math.Ceiling((double) buttonCount / rows);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetFormSize() {
			try {
				int height = TOP_MARGIN + titleLabel.Height + TITLE_TEXT_SPACING + (BUTTON_HEIGHT * rows) + (BUTTON_SPACING_VERTICAL * (rows - 1)) + BOTTOM_MARGIN + VERTICAL_CORRECTION;
				int width = (SIDE_MARGIN * 2) + Math.Max(titleLabel.Width, (BUTTON_WIDTH * columns) + (BUTTON_SPACING_HORIZONTAL * (columns - 1))) + HORIZONTAL_CORRECTION;

				Size = new Size(width, height);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private Point GetButtonLocation(int index) {
			try {
				int row = index / columns, column = index % columns;

				int x = SIDE_MARGIN + (column * (BUTTON_WIDTH + BUTTON_SPACING_HORIZONTAL));
				int y = TOP_MARGIN + titleLabel.Height + TITLE_TEXT_SPACING + (row * (BUTTON_HEIGHT + BUTTON_SPACING_VERTICAL));

				return new Point(x, y);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return default;
			}
		}

		private void Button_Click(object? sender, EventArgs e) {
			try {
				if (sender is null)
					return;

				Button button = (Button) sender;
				string message = string.Empty;

				foreach (var ticketingButton in ticketingButtons) {
					if (int.Parse(button.Name) == ticketingButton.ButtonId) {
						if (ticketingButton is IssueTicketButton issueTicketButton) {
							message = issueTicketButton.Service;
						}
						else if (ticketingButton is ShowMessageButton showMessageButton) {
							string messageEn = showMessageButton.MessageEn;
							string messageAr = showMessageButton.MessageAr;

							message = $"{messageEn}.\n{messageAr}.";
						}

						break;
					}
				}

				MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SwitchLanguage(object? sender, EventArgs e) {
			try {
				if (language == "en") {
					for (int i = 0; i < formButtons.Count; ++i) {
						formButtons[i].RightToLeft = RightToLeft.Yes;
						formButtons[i].Text = ticketingButtons[i].NameAr;
					}

					language = "ar";
				}
				else {
					for (int i = 0; i < formButtons.Count; ++i) {
						formButtons[i].RightToLeft = RightToLeft.No;
						formButtons[i].Text = ticketingButtons[i].NameEn;
					}

					language = "en";
				}

				languageButton.Text = language.ToUpper();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show("An unexpected error has occurred. Check the logs for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
