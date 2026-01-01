using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace HandleLogChangeDB
{
    public class MarkInvoiceDirty
    {
        private DTO_I_DBHelper dbHelper;
        public MarkInvoiceDirty(DTO_I_DBHelper _dbHelper)
        {
            dbHelper = _dbHelper;
        }
        public void edit_doanhThu(DateTime date)
        {
            string msgError = "";
            object result=dbHelper.ExecuteScalarSProcedureWithTransaction(out msgError, "edit_doanhThu", "@date", date);
            if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
            {
                throw new Exception(Convert.ToString(result) + msgError);
            }
            
        }
    }
}
