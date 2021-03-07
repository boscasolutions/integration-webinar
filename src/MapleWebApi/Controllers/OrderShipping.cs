using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MapleWebApi.Data.Services.Interfaces;
using MapleWebApi.Models;

namespace MapleWebApi.Controllers
{
    [ApiController]
    [Route("OrderShipping")]
    public class OrderShippingController : ControllerBase
    {
        private readonly IOrderShippingService _orderShippingService;
        public OrderShippingController(IOrderShippingService orderShippingService)
        {
            _orderShippingService = orderShippingService;
        }

        [HttpPost("{orderId}", Name = "GetByOrderById")]
        public async Task<ActionResult> GetByOrderById(string orderId)
        {
            if (Program.responseSet == 200)
            {
                var result = await _orderShippingService.GetById(orderId).ConfigureAwait(false);
                return Ok(result);
            }
            else
            {
                return new StatusCodeResult(Program.responseSet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderShipping model)
        {
            if (Program.responseSet == 200)
            {
                var tracking = Guid.NewGuid().ToString();
                model.TrackingNumber = tracking;
                
                var result = await _orderShippingService.Create(model).ConfigureAwait(false);
                
                return CreatedAtAction(
                    nameof(GetByOrderById),
                    new { id = result.OrderId }, result);
            }
            else
            {
                return new StatusCodeResult(Program.responseSet);
            }
        }
    }
}