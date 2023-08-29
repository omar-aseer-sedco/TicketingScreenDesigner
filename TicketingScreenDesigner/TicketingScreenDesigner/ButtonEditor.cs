using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketingScreenDesigner
{
	public partial class ButtonEditor : Form
	{
		private const string titleText = "Button Editor";
		private const int defaultPanelPositionY = 131;
		private const int minimumHeightIssueTicket = 233;
		private const int minimumHeightShowMessage = 260;
		private readonly TicketingButton button;
		private readonly SqlConnection connection;
		private readonly ScreenEditor callingForm;
		private readonly bool newButton;

		private ButtonEditor()
		{
			InitializeComponent();
			connection = dbUtils.CreateConnection();
			button = new TicketingButton();
			callingForm = new ScreenEditor(new BankForm(button.BankName), button.BankName);
		}

		private ButtonEditor(ScreenEditor callingForm) : this()
		{
			this.callingForm = callingForm;
		}

		public ButtonEditor(ScreenEditor callingForm, string bankName, string screenId) : this(callingForm)
		{
			button.BankName = bankName;
			button.ScreenId = screenId;
			Text = titleText + " - New Button";
			newButton = true;
		}

		public ButtonEditor(ScreenEditor callingForm, string bankName, string screenId, string buttonId) : this(callingForm)
		{
			TicketingButton? button = GetButtonById(bankName, screenId, buttonId);

			if (button is null)
			{
				MessageBox.Show("This button does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}
			else
			{
				this.button = button;
				FillTextBoxes(button);
				newButton = false;
			}
		}

		public ButtonEditor(ScreenEditor callingForm, TicketingButton button) : this(callingForm)
		{
			this.button = button;
			FillTextBoxes(button);
		}

		private TicketingButton? GetButtonById(string bankName, string screenId, string buttonId)
		{
			TicketingButton? ret = null;

			string query = $"SELECT * FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenId);
			command.Parameters.AddWithValue("@buttonId", buttonId);

			try
			{
				connection.Open();

				var reader = command.ExecuteReader();

				if (reader.Read())
				{
					ret = new TicketingButton(reader[ButtonsConstants.BANK_NAME].ToString(), reader[ButtonsConstants.SCREEN_ID].ToString(), reader[ButtonsConstants.BUTTON_ID].ToString(), 
						reader[ButtonsConstants.TYPE].ToString(), reader[ButtonsConstants.NAME_EN].ToString(), reader[ButtonsConstants.NAME_AR].ToString(), 
						reader[ButtonsConstants.SERVICE].ToString(), reader[ButtonsConstants.MESSAGE_EN].ToString(), reader[ButtonsConstants.MESSAGE_AR].ToString());
				}
			}
			catch (Exception ex) // INCOMPLETE
			{
				MessageBox.Show(ex.Message, "Something went wrong XD - GetButtonById");
			}
			finally
			{
				connection.Close();
			}

			return ret;
		}

		private void FillTextBoxes(TicketingButton button)
		{
			nameEnTextBox.Text = button.NameEn;
			nameArTextBox.Text = button.NameAr;
			typeComboBox.SelectedIndex = button.Type == "Issue Ticket" ? 0 : 1;
			messageEnTextBox.Text = button.MessageEn ?? string.Empty;
			messageArTextBox.Text = button.MessageAr ?? string.Empty;
			serviceTextBox.Text = button.Service ?? string.Empty;
			ShowTypeSpecificFields();
		}

		private void ShowTypeSpecificFields()
		{
			if (typeComboBox.SelectedIndex == -1)
			{
				issueTicketPanel.Visible = false;
				showMessagePanel.Visible = false;
			}
			else if (typeComboBox.SelectedIndex == 0)
			{
				issueTicketPanel.Visible = true;
				showMessagePanel.Visible = false;

				issueTicketPanel.Location = new Point(issueTicketPanel.Location.X, defaultPanelPositionY);

				if (Size.Height < minimumHeightIssueTicket)
				{
					Size = new Size(Size.Width, minimumHeightIssueTicket);
				}
			}
			else if (typeComboBox.SelectedIndex == 1)
			{
				issueTicketPanel.Visible = false;
				showMessagePanel.Visible = true;

				showMessagePanel.Location = new Point(showMessagePanel.Location.X, defaultPanelPositionY);

				if (Size.Height < minimumHeightShowMessage)
				{
					Size = new Size(Size.Width, minimumHeightShowMessage);
				}
			}
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowTypeSpecificFields();
		}
	}
}
