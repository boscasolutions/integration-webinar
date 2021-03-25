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

        [HttpPost]
        [Route("GetByOrderById")]
        public async Task<ActionResult> GetByOrderById([FromBody] OrderShipping model)
        {
            OrderShipping result = await _orderShippingService.GetById(model.OrderId).ConfigureAwait(false);

            if (result.State == "Not Found" || string.IsNullOrEmpty(result.OrderId))
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderShipping model)
        {
            if (Program.responseSet == 200)
            {
                // Try to check if the item exist (idempotent)
                OrderShipping result = await _orderShippingService.GetById(model.OrderId).ConfigureAwait(false);
                if (result.OrderId == model.OrderId)
                    return Ok(result);

                model.TrackingNumber = Guid.NewGuid().ToString();

                result = await _orderShippingService.Create(model)
                    .ConfigureAwait(false);

                return Ok(result);
            }

            if (Program.responseSet == 500)
            {
                model.TrackingNumber = Guid.NewGuid().ToString();

                await _orderShippingService.Create(model)
                    .ConfigureAwait(false);

                return Problem();
            }
            else
            {
                return new StatusCodeResult(Program.responseSet);
            }
        }
    }
}