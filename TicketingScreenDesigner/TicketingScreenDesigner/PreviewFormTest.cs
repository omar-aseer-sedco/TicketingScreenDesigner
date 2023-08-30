namespace TicketingScreenDesigner {
	public class PreviewFormTest : Form {
		private const int TOP_MARGIN = 20;
		private const int BOTTOM_MARGIN = 60;
		private const int SIDE_MARGIN = 75;
		private const int TEXT_SPACING = 30;
		private const int BUTTON_HEIGHT = 23;
		private const int BUTTON_WIDTH = 75;
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

		public PreviewFormTest(string screenTitle, List<TicketingButton> ticketingButtons) {
			titleText = screenTitle;
			formButtons = new List<Button>();
			titleLabel = new Label();
			this.ticketingButtons = ticketingButtons;
			buttonCount = ticketingButtons.Count;
			InitializeComponent();
		}

		private void InitializeComponent() {
			SuspendLayout();

			SetGridDimensions();
			SetFormSize();

			titleLabel.AutoSize = true;
			titleLabel.Text = titleText;
			titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
			titleLabel.TextAlign = ContentAlignment.MiddleCenter;
			titleLabel.Location = new Point((Size.Width - titleLabel.Width) / 2, TOP_MARGIN);

			int i = 0;
			foreach (var ticketingButton in ticketingButtons) {
				formButtons.Add(new Button {
					Name = ticketingButton.ButtonId,
					Text = ticketingButton.NameEn,
					Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
					Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
					Location = GetButtonLocation(i),
				});

				++i;
			}

			Controls.AddRange(formButtons.ToArray<Control>());
			Controls.Add(titleLabel);

			ResumeLayout();
		}

		private int GetNextSquareRoot(int x) {
			return (int) Math.Ceiling(Math.Sqrt(x));
		}

		private void SetGridDimensions() {
			rows = GetNextSquareRoot(buttonCount);
			columns = (int) Math.Ceiling((double) buttonCount / rows) + (buttonCount % rows != 0 ? 1 : 0);

			MessageBox.Show($"rows = {rows}, columns = {columns}");
		}

		private void SetFormSize() {
			int height = TOP_MARGIN + titleLabel.Height + TEXT_SPACING + (BUTTON_HEIGHT * rows) + (BUTTON_SPACING_VERTICAL * (rows - 1)) + BOTTOM_MARGIN + VERTICAL_CORRECTION;
			int width = (SIDE_MARGIN * 2) + Math.Max(titleLabel.Width, (BUTTON_WIDTH * columns) + (BUTTON_SPACING_HORIZONTAL * (columns - 1))) + HORIZONTAL_CORRECTION;

			Size = new Size(width, height);
		}

		private Point GetButtonLocation(int index) {
			int row = index / rows, column = index % rows;

			int x = SIDE_MARGIN + (column * (BUTTON_WIDTH + BUTTON_SPACING_HORIZONTAL));
			int y = TOP_MARGIN + titleLabel.Height + TEXT_SPACING + (row * (BUTTON_HEIGHT + BUTTON_SPACING_VERTICAL));

			return new Point(x, y);
		}
	}
}
