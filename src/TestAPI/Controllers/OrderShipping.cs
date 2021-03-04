using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApi.Data.Services.Interfaces;
using TestApi.Models;

namespace TestApi.Controllers
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

        [HttpGet("{orderId}", Name = "GetByOrderById")]
        public async Task<ActionResult> GetByOrderById(string orderId)
        {
            var result = await _orderShippingService.GetById(orderId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderShipping model)
        {
            var tracking = Guid.NewGuid().ToString();
            model.TrackingNumber = tracking;

            if (model.IsValid(out IEnumerable<string> errors))
            {
                var result = await _orderShippingService.Create(model);

                return CreatedAtAction(
                    nameof(GetByOrderById),
                    new { id = result.OrderId }, result);
            }
            else
            {
                return BadRequest(errors);
            }
        }
    }
}