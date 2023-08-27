using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TicketingScreenDesigner
{
	public partial class BankForm : Form
	{
		private readonly string bankName;

		private BankForm()
		{
			InitializeComponent();
		}

		public BankForm(string bankName) : this()
		{
			this.bankName = bankName;
			Text = "Ticketing Screen Designer - " + bankName;
		}

		private void addScreenButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("This doesn't do anything yet.", "Wow.");
		}
	}
}
