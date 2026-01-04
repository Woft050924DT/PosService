using System.Collections.Generic;
using BLL;
using Microsoft.AspNetCore.Mvc;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly bll_Promotions _promotionBll;

        public PromotionsController(bll_Promotions promotionBll)
        {
            _promotionBll = promotionBll;
        }

        [HttpGet]
        public ActionResult<List<PromotionDTO>> GetAll([FromQuery] bool? isActive = null)
        {
            var list = _promotionBll.GetAllPromotions(isActive);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<PromotionDTO> GetById(int id)
        {
            var promotion = _promotionBll.GetPromotionById(id);
            if (promotion == null) return NotFound();
            return Ok(promotion);
        }

        [HttpPost]
        public ActionResult<bool> Create([FromBody] CreatePromotionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest();

            var ok = _promotionBll.CreatePromotion(dto);
            if (!ok) return BadRequest();
            return Ok(true);
        }
    }
}

