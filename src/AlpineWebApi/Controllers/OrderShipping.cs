using System;
using System.Threading.Tasks;
using AlpineWebApi.Data.Services.Interfaces;
using AlpineWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlpineWebApi.Controllers
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
                OrderShipping result = await _orderShippingService.GetById(orderId).ConfigureAwait(false);
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
                string tracking = Guid.NewGuid().ToString();
                model.TrackingNumber = tracking;

                OrderShipping result = await _orderShippingService.Create(model).ConfigureAwait(false);

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