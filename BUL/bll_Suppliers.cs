using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class bll_Suppliers
    {
        private readonly dal_Suppliers dal;
        public bll_Suppliers(dal_Suppliers _dal)
        {
            dal = _dal;
        }
        public List<DTO.dto_Suppliers> GetAllSuppliers()
        {
            return dal.GetAllSuppliers();
        }
        public bool AddSupplier(DTO.dto_Suppliers supplier)
        {
            if (string.IsNullOrEmpty(supplier.SupplierName))
                throw new Exception("SupplierName không được để trống");
            return dal.Add(supplier);
        }
        public bool UpdateSupplier(DTO.dto_Suppliers supplier)
        {
            if (supplier.SupplierId <= 0)
                throw new Exception("SupplierId không hợp lệ");
            if (string.IsNullOrEmpty(supplier.SupplierName))
                throw new Exception("SupplierName không được để trống");
            return dal.Update(supplier);
        }
        public bool DeleteSupplier(int supplierId)
        {
            if (supplierId <= 0)
                return false;
            return dal.Delete(supplierId);
        }
    }
}
