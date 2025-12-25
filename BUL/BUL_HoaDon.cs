using DAL.Models;
using DTO;
using Microsoft.Extensions.Options;

namespace BUL_HoaDon
{
    public class BUL_HoaDon
    {
        private DAL_HoaDon DAL_hoaDon;

        public BUL_HoaDon(DAL_HoaDon _DAL_hoaDon)
        {
            DAL_hoaDon = _DAL_hoaDon;
        }

        public void Create(PurchaseOrder model, int supplierId, int userId)
        {
            DAL_hoaDon.create(model, supplierId, userId);
        }
    }
}
