using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosService.DAL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly PromotionDAL _promotionDal;

        public PromotionsController(PromotionDAL promotionDal)
        {
            _promotionDal = promotionDal;
        }

        [HttpGet]
        public async Task<ActionResult<List<PromotionDTO>>> GetAll([FromQuery] bool? isActive = null)
        {
            var list = await _promotionDal.GetAllAsync(isActive);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PromotionDTO>> GetById(int id)
        {
            var promotion = await _promotionDal.GetByIdAsync(id);
            if (promotion == null) return NotFound();
            return Ok(promotion);
        }

        [HttpPost]
        public async Task<ActionResult<PromotionDTO>> Create([FromBody] CreatePromotionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest();

            var created = await _promotionDal.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.PromotionId }, created);
        }
    }
}

