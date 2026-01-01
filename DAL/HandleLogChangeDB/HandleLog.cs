using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DTO;
using DAL;
namespace HandleLogChangeDB
{
    public class HandleLog
    {
        private GeneralDashboard generaledValueDashboard = new GeneralDashboard();
        private MarkInvoiceDirty markInvoiceDirty;
        private NotifyAdmin notifyAdmin;

        public HandleLog(MarkInvoiceDirty _markInvoiceDirty, NotifyAdmin _notifyAdmin) {
            markInvoiceDirty= _markInvoiceDirty;
            notifyAdmin= _notifyAdmin;
        }
        public void handleLog(DTO_InvoiceChangeLog log)
        {
            //client: 5p gui api tinh trackDailyGeneralDashboard 1 lan
            if (log.invoiceDate.Date != DateTime.Today)
            {
                 markInvoiceDirty.edit_doanhThu(log.invoiceDate);
                notifyAdmin.addNotification();//1 tu bang role lay ve receiverId
            }
        }
    }
}
