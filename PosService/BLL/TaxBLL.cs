using System;
using System.Collections.Generic;
using PosService.DAL;
using PosService.DTO;

namespace BLL
{
    public class bll_Tax
    {
        private readonly TaxDAL dal;

        public bll_Tax(TaxDAL taxDal)
        {
            dal = taxDal;
        }

        public List<TaxDTO> GetAllTax(bool? isActive = null, string? q = null)
        {
            return dal.GetAllAsync(isActive, q).GetAwaiter().GetResult();
        }

        public TaxDTO? GetTaxById(int id)
        {
            if (id <= 0)
                throw new Exception("TaxId không hợp lệ");

            return dal.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public bool CreateTax(CreateTaxDTO dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu thuế không được để trống");

            if (string.IsNullOrEmpty(dto.TaxCode))
                throw new Exception("TaxCode không được để trống");

            if (string.IsNullOrEmpty(dto.TaxName))
                throw new Exception("TaxName không được để trống");

            var created = dal.CreateAsync(dto).GetAwaiter().GetResult();
            return created != null && created.TaxId > 0;
        }

        public bool UpdateTax(int id, UpdateTaxDTO dto)
        {
            if (id <= 0)
                throw new Exception("TaxId không hợp lệ");

            if (dto == null)
                throw new Exception("Dữ liệu thuế không được để trống");

            var updated = dal.UpdateAsync(id, dto).GetAwaiter().GetResult();
            return updated != null;
        }

        public bool DeleteTax(int id)
        {
            if (id <= 0)
                throw new Exception("TaxId không hợp lệ");

            return dal.DeleteAsync(id).GetAwaiter().GetResult();
        }
    }
}

