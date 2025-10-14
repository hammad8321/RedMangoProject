using MangoFusion_API.Data;
using MangoFusion_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MangoFusion_API.Controllers
{
    public class OrderController : Controller


    {
        private readonly ApplicationDbContext _db;
        private readonly ApiResponse _response;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetOrders(string userId = "")
        {
            IEnumerable<OrderHeader> orderHeadersList = _db.OrderHeader.Include(x => x.OrderDetails)
                .ThenInclude(x => x.MenuItem).OrderByDescending(x => x.OrderHeaderId);

            if (!string.IsNullOrEmpty(userId))
            {
                orderHeadersList = orderHeadersList.Where(x => x.ApplicationUserId == userId);
            }



            _response.Result = orderHeadersList;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }


        [HttpGet("{orderId: int}")]
        public ActionResult<ApiResponse> GetOrders(int orderId)
        {
            if(orderId==0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage.Add("Invalid order ID");
                return BadRequest(_response);
            }
            OrderHeader? orderHeader = _db.OrderHeader.Include(x => x.OrderDetails)
                    .ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.OrderHeaderId == orderId)
                    ;
            if(orderHeader==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessage.Add("Order not found");
                return NotFound(_response);
            }
          



            _response.Result = orderHeader;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }

    }
}
