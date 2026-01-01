using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HandleLogChangeDB
{
    public class NotifyAdmin
    {
        private DTO_I_DBHelper dbHelper;
        public NotifyAdmin(DTO_I_DBHelper _dbHelper)
        {
            dbHelper = _dbHelper;
        }
        public void addNotification()
        {
            string msgError = "";
            object result = dbHelper.ExecuteScalarSProcedureWithTransaction(out msgError, "addNotification", "@receiverID", 3);
            if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
            {
                throw new Exception(Convert.ToString(result) + msgError);
            }
        }
    }
}
