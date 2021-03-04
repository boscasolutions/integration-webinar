using System;
using System.Linq;
using System.Threading.Tasks;
using TestApi.Data.Repositories.Repositories.Interfaces;
using TestApi.Data.Services.Interfaces;
using TestApi.Models;

namespace TestApi.Data.Services
{
    public class OrderShippingService : IOrderShippingService
    {
        private readonly IOrderShippingRepository _repository;

        public OrderShippingService(IOrderShippingRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderShipping> Create(OrderShipping orderShipping)
        {
            orderShipping.LastUpdatedDateTimeUtc = DateTime.UtcNow;

            var success = await _repository.Create(orderShipping);

            if (success)
                return orderShipping;
            else
                return null;
        }

        public async Task<OrderShipping> Update(OrderShipping orderShipping)
        {
            orderShipping.LastUpdatedDateTimeUtc = DateTime.UtcNow;

            var success = await _repository.Update(orderShipping);

            if (success)
                return orderShipping;
            else
                return null;
        }

        public async Task<OrderShipping> GetById(string orderShippingId)
        {
            var result = await _repository.GetById(orderShippingId);

            return result;
        }

        public async Task<IOrderedQueryable<OrderShipping>> GetAll()
        {
            var result = await _repository.GetAll();

            return result;
        }

        public async Task<bool> Delete(string orderShippingId)
        {
            var success = await _repository.Delete(orderShippingId);

            return success;
        }
    }
}