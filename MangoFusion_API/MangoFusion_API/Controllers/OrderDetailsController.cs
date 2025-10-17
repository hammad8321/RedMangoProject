using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MangoFusion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : Controller


    {
        private readonly ApplicationDbContext _db;
        private readonly ApiResponse _response;
        public OrderDetailsController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }



        [HttpPut("{orderDetailsId:int}")]
        public ActionResult<ApiResponse> UpdateOrder(int orderDetailsId, [FromBody] OrderDetailsUpdateDTO orderDetailsUpdateDTO)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (orderDetailsId != orderDetailsUpdateDTO.OrderDetailId)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Order ID mismatch");
                        return BadRequest(_response);
                    }

                    OrderDetail? orderDetailsFromDb = _db.OrderDetail.FirstOrDefault(x => x.OrderDetailId == orderDetailsId);
                    if (orderDetailsUpdateDTO == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Order not found");
                        return NotFound(_response);
                    }
                    orderDetailsFromDb.Rating = orderDetailsUpdateDTO.Rating;
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_response);



                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);

                }



            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }


        }

    }
}
