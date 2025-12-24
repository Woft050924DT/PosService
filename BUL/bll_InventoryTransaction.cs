using DAL;
using DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace BLL
{
    public class bll_InventoryTransaction
    {
        
        private readonly dal_InventoryTransaction _dal;

        public bll_InventoryTransaction(dal_InventoryTransaction dal)
        {
            _dal = dal;
        }
        public List<dto_InventoryTransaction> GetAll()
        {
            return _dal.GetAll();
        }
        public bool AddTransaction(dto_InventoryTransaction transaction)
        {
            if (transaction.ProductId == null || transaction.ProductId <= 0)
                throw new Exception("ProductId không hợp lệ");
            if (transaction.Quantity == 0)
                throw new Exception("Quantity phải khác 0");

            return _dal.Add(transaction);
        }
        public bool UpdateTransaction(dto_InventoryTransaction transaction)
        {
            if (transaction.TransactionId <= 0)
                throw new Exception("TransactionId không hợp lệ");
            return _dal.Update(transaction);
        }
        public bool DeleteTransaction(int TransactionId) { 
            if(TransactionId  <= 0)
            {
                return false;
            }
            return _dal.Delete(TransactionId);
        }
    }
}
