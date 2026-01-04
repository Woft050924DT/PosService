using System;
using System.Collections.Generic;
using PosService.DAL;
using PosService.DTO;

namespace BLL
{
    public class bll_Products
    {
        private readonly ProductDAL dal;

        public bll_Products(ProductDAL productDal)
        {
            dal = productDal;
        }

        public List<ProductDTO> GetAllProducts(
            bool? isActive = null,
            int? categoryId = null,
            int? supplierId = null,
            string? q = null)
        {
            return dal.GetAllAsync(isActive, categoryId, supplierId, q).GetAwaiter().GetResult();
        }

        public ProductDTO? GetProductById(int productId)
        {
            if (productId <= 0)
                throw new Exception("ProductId không hợp lệ");

            return dal.GetByIdAsync(productId).GetAwaiter().GetResult();
        }

        public bool CreateProduct(ProductDTO product)
        {
            if (product == null)
                throw new Exception("Dữ liệu sản phẩm không được để trống");

            if (string.IsNullOrEmpty(product.ProductCode))
                throw new Exception("ProductCode không được để trống");

            if (string.IsNullOrEmpty(product.ProductName))
                throw new Exception("ProductName không được để trống");

            var created = dal.CreateAsync(product).GetAwaiter().GetResult();
            return created != null && created.ProductId > 0;
        }

        public bool UpdateProduct(ProductDTO product)
        {
            if (product == null)
                throw new Exception("Dữ liệu sản phẩm không được để trống");

            if (product.ProductId <= 0)
                throw new Exception("ProductId không hợp lệ");

            if (string.IsNullOrEmpty(product.ProductCode))
                throw new Exception("ProductCode không được để trống");

            if (string.IsNullOrEmpty(product.ProductName))
                throw new Exception("ProductName không được để trống");

            var updated = dal.UpdateAsync(product.ProductId, product).GetAwaiter().GetResult();
            return updated != null;
        }

        public bool DeleteProduct(int productId)
        {
            if (productId <= 0)
                throw new Exception("ProductId không hợp lệ");

            return dal.DeleteAsync(productId).GetAwaiter().GetResult();
        }
    }
}

