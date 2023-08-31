namespace TicketingScreenDesigner {
	public class PreviewForm : Form {
		private const int TOP_MARGIN = 20;
		private const int BOTTOM_MARGIN = 60;
		private const int SIDE_MARGIN = 60;
		private const int TEXT_SPACING = 30;
		private const int BUTTON_HEIGHT = 50;
		private const int BUTTON_WIDTH = 120;
		private const int BUTTON_SPACING_VERTICAL = 30;
		private const int BUTTON_SPACING_HORIZONTAL = 50;
		private const int VERTICAL_CORRECTION = 30;
		private const int HORIZONTAL_CORRECTION = 16;

		private readonly List<TicketingButton> ticketingButtons;
		private readonly string titleText;
		private readonly int buttonCount;

		private List<Button> formButtons;
		private Label titleLabel;
		private int rows;
		private int columns;

		public PreviewForm(string screenTitle, List<TicketingButton> ticketingButtons) {
			titleText = screenTitle;
			this.ticketingButtons = ticketingButtons;
			buttonCount = ticketingButtons.Count;
			InitializeComponent();
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

				SetGridDimensions();
				SetFormSize();
				MinimumSize = Size;

				Padding = new Padding(SIDE_MARGIN, TOP_MARGIN, SIDE_MARGIN, BOTTOM_MARGIN);
				titleLabel.Location = new Point((Size.Width - titleLabel.Width) / 2, TOP_MARGIN);

				int i = 0;
				foreach (var ticketingButton in ticketingButtons) {
					formButtons.Add(new Button {
						Name = ticketingButton.ButtonId,
						Text = ticketingButton.NameEn,
						Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
						Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
						Location = GetButtonLocation(i),
						Anchor = AnchorStyles.None,
						AutoEllipsis = true,
					});

					++i;
				}

				Controls.AddRange(formButtons.ToArray<Control>());
				Controls.Add(titleLabel);

				ResumeLayout();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private int GetNextSquareRoot(int x) {
			return (int) Math.Ceiling(Math.Sqrt(x));
		}

		private void SetGridDimensions() {
			rows = GetNextSquareRoot(buttonCount);
			columns = (int) Math.Ceiling((double) buttonCount / rows);
		}

		private void SetFormSize() {
			int height = TOP_MARGIN + titleLabel.Height + TEXT_SPACING + (BUTTON_HEIGHT * rows) + (BUTTON_SPACING_VERTICAL * (rows - 1)) + BOTTOM_MARGIN + VERTICAL_CORRECTION;
			int width = (SIDE_MARGIN * 2) + Math.Max(titleLabel.Width, (BUTTON_WIDTH * columns) + (BUTTON_SPACING_HORIZONTAL * (columns - 1))) + HORIZONTAL_CORRECTION;

			Size = new Size(width, height);
		}

		private Point GetButtonLocation(int index) {
			int row = index / columns, column = index % columns;

			int x = SIDE_MARGIN + (column * (BUTTON_WIDTH + BUTTON_SPACING_HORIZONTAL));
			int y = TOP_MARGIN + titleLabel.Height + TEXT_SPACING + (row * (BUTTON_HEIGHT + BUTTON_SPACING_VERTICAL));

			return new Point(x, y);
		}
	}
}
