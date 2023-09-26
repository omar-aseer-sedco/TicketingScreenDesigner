using DataAccessLayer.Constants;
using ExceptionUtils;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer.Listeners {
	public class SqlButtonListener : SqlListener {
		private readonly string bankName;
		private readonly int screenId;

		public SqlButtonListener(string bankName, int screenId) : base() {
			try {
				this.bankName = bankName;
				this.screenId = screenId;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				this.bankName = string.Empty;
				this.screenId = -1;
			}
		}

		protected override SqlCommand CreateCommand() {
			try {
				CommandText = $"SELECT {ButtonsConstants.NAME_EN}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN} FROM dbo.{ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId;";

				SqlCommand command = new SqlCommand(CommandText, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return new SqlCommand();
			}
		}
	}
}
