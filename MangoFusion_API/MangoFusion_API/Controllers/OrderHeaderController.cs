using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MangoFusion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHeaderController : Controller


    {
        private readonly ApplicationDbContext _db;
        private readonly ApiResponse _response;
        public OrderHeaderController(ApplicationDbContext db)
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


        [HttpGet("{orderId:int}")]
        public ActionResult<ApiResponse> GetOrders(int orderId)
        {
            if (orderId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage.Add("Invalid order ID");
                return BadRequest(_response);
            }
            OrderHeader? orderHeader = _db.OrderHeader.Include(x => x.OrderDetails)
                    .ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.OrderHeaderId == orderId)
                    ;
            if (orderHeader == null)
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
        [HttpPost]
        public ActionResult<ApiResponse> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderCreatewDTO)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    OrderHeader orderHeader = new()
                    {
                        PickUpName = orderHeaderCreatewDTO.PickUpName,
                        PickUpPhoneNumber = orderHeaderCreatewDTO.PickUpPhoneNumber,
                        PickUpEmail = orderHeaderCreatewDTO.PickUpEmail,
                        OrderDate = DateTime.Now,
                        ApplicationUserId = orderHeaderCreatewDTO.ApplicationUserId,
                        OrderTotal = orderHeaderCreatewDTO.OrderTotal,
                        OrderStatus = SD.Status_Confirmed,
                        TotalItems = orderHeaderCreatewDTO.TotalItems,


                        // OrderStatus = string.IsNullOrEmpty(orderHeaderCreateDTO.OrderStatus) ? "Pending" : orderHeaderCreateDTO.OrderStatus,
                        //  TotalItems = orderHeaderCreateDTO.TotalItems,

                    };

                    _db.OrderHeader.Add(orderHeader);
                    _db.SaveChanges(); // to get the orderHeaderId

                    foreach (var orderDetailDTO in orderHeaderCreatewDTO.OrderDetailsDTO)
                    {
                        OrderDetail orderDetails = new()
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            MenuItemId = orderDetailDTO.MenuItemId,
                            Quantity = orderDetailDTO.Quantity,
                            ItemName = orderDetailDTO.ItemName,
                            Price = orderDetailDTO.Price,
                        };
                        _db.OrderDetail.Add(orderDetails);
                    }

                    _db.SaveChanges();

                    _response.Result = orderHeader;
                    orderHeader.OrderDetails = []; // to avoid circular reference in JSON serialization
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtAction(nameof(GetOrders), new { orderId = orderHeader.OrderHeaderId }, _response);


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



        [HttpPut("{orderId:int}")]
        public ActionResult<ApiResponse> UpdateOrder(int orderId, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if(orderId != orderHeaderUpdateDTO.OrderHeaderId)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Order ID mismatch");
                        return BadRequest(_response);
                    }

                    OrderHeader? orderHeaderFromDb = _db.OrderHeader.FirstOrDefault(x => x.OrderHeaderId == orderId);
                    if (orderHeaderFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Order not found");
                        return NotFound(_response);
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpName))
                    {
                        orderHeaderFromDb.PickUpName = orderHeaderUpdateDTO.PickUpName;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpPhoneNumber))
                    {
                        orderHeaderFromDb.PickUpPhoneNumber = orderHeaderUpdateDTO.PickUpPhoneNumber;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpEmail))
                    {
                        orderHeaderFromDb.PickUpEmail = orderHeaderUpdateDTO.PickUpEmail;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.OrderStatus))
                    {
                        if (orderHeaderFromDb.OrderStatus.Equals(SD.Status_Confirmed, StringComparison.InvariantCultureIgnoreCase)
                            && orderHeaderUpdateDTO.OrderStatus.Equals(SD.Status_ReadyForPickUP, StringComparison.InvariantCultureIgnoreCase))
                        {
                            orderHeaderFromDb.OrderStatus = SD.Status_ReadyForPickUP;
                        }
                        if (orderHeaderFromDb.OrderStatus.Equals(SD.Status_ReadyForPickUP, StringComparison.InvariantCultureIgnoreCase)
                            && orderHeaderUpdateDTO.OrderStatus.Equals(SD.Status_Completed, StringComparison.InvariantCultureIgnoreCase))
                        {
                            orderHeaderFromDb.OrderStatus = SD.Status_Completed;
                        }
                        if (orderHeaderUpdateDTO.OrderStatus.Equals(SD.Status_Cancelled, StringComparison.InvariantCultureIgnoreCase))
                        {
                            orderHeaderFromDb.OrderStatus = SD.Status_Cancelled;
                        }

                    }
                    _db.SaveChanges();



                    _response.StatusCode = HttpStatusCode.NoContent
;
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
