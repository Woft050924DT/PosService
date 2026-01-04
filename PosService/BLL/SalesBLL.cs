using System;
using System.Collections.Generic;
using PosService.DAL;
using PosService.DTO;

namespace BLL
{
    public class bll_Sales
    {
        private readonly SalesDAL dal;

        public bll_Sales(SalesDAL salesDal)
        {
            dal = salesDal;
        }

        public List<SalesInvoiceDTO> GetAllSales(int? customerId = null, DateTime? from = null, DateTime? to = null)
        {
            return dal.GetAllAsync(customerId, from, to).GetAwaiter().GetResult();
        }

        public SalesInvoiceDTO? GetSalesInvoiceById(int id)
        {
            if (id <= 0)
                throw new Exception("InvoiceId không hợp lệ");

            return dal.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public bool CreateSalesInvoice(CreateSalesInvoiceDTO dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu hóa đơn không được để trống");

            if (dto.Details == null || dto.Details.Count == 0)
                throw new Exception("Hóa đơn phải có ít nhất một chi tiết");

            var created = dal.CreateAsync(dto).GetAwaiter().GetResult();
            return created != null && created.InvoiceId > 0;
        }
    }
}

