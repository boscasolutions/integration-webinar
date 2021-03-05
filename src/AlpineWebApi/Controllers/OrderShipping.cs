using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlpineWebApi.Data.Services.Interfaces;
using AlpineWebApi.Models;

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
            var result = await _orderShippingService.GetById(orderId).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderShipping model)
        {
            var tracking = Guid.NewGuid().ToString();
            model.TrackingNumber = tracking;

            var result = await _orderShippingService.Create(model).ConfigureAwait(false);

            return CreatedAtAction(
                nameof(GetByOrderById),
                new { id = result.OrderId }, result);
        }
    }
}