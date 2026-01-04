using System;
using System.Collections.Generic;
using PosService.DAL;
using PosService.DTO;

namespace BLL
{
    public class bll_Promotions
    {
        private readonly PromotionDAL dal;

        public bll_Promotions(PromotionDAL promotionDal)
        {
            dal = promotionDal;
        }

        public List<PromotionDTO> GetAllPromotions(bool? isActive = null)
        {
            return dal.GetAllAsync(isActive).GetAwaiter().GetResult();
        }

        public PromotionDTO? GetPromotionById(int id)
        {
            if (id <= 0)
                throw new Exception("PromotionId không hợp lệ");

            return dal.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public bool CreatePromotion(CreatePromotionDTO dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu khuyến mãi không được để trống");

            if (string.IsNullOrEmpty(dto.PromotionCode))
                throw new Exception("PromotionCode không được để trống");

            if (string.IsNullOrEmpty(dto.PromotionName))
                throw new Exception("PromotionName không được để trống");

            var created = dal.CreateAsync(dto).GetAwaiter().GetResult();
            return created != null && created.PromotionId > 0;
        }
    }
}

