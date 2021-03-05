using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AlpineWebApi.Data.Repositories.Repositories.Interfaces;
using AlpineWebApi.Models;

namespace AlpineWebApi.Data.Repositories
{
    public class OrderShippingRepository : IOrderShippingRepository
    {
        private readonly IServiceScope _scope;
        private readonly OrderShippingDatabaseContext _databaseContext;

        public OrderShippingRepository(IServiceProvider services)
        {
            _scope = services.CreateScope();

            _databaseContext = _scope.ServiceProvider.GetRequiredService<OrderShippingDatabaseContext>();
        }

        public async Task<bool> Create(OrderShipping orderShipping)
        {
            var success = false;

            _databaseContext.OrderShippings.Add(orderShipping);

            var numberOfItemsCreated = await _databaseContext.SaveChangesAsync();

            if (numberOfItemsCreated == 1)
                success = true;

            return success;
        }

        public async Task<bool> Update(OrderShipping orderShipping)
        {
            var success = false;

            var existingOrderShipping = await GetById(orderShipping.OrderId);

            if (existingOrderShipping != null)
            {
                existingOrderShipping.State = orderShipping.State;
                existingOrderShipping.LastUpdatedDateTimeUtc = orderShipping.LastUpdatedDateTimeUtc;

                _databaseContext.OrderShippings.Attach(existingOrderShipping);

                var numberOfItemsUpdated = await _databaseContext.SaveChangesAsync();

                if (numberOfItemsUpdated == 1)
                    success = true;
            }

            return success;
        }

        public async Task<OrderShipping> GetById(string orderId)
        {
            var result = _databaseContext.OrderShippings
                                .Where(x => x.OrderId == orderId)
                                .FirstOrDefault();

            return result;
        }

        public async Task<IOrderedQueryable<OrderShipping>> GetAll()
        {
            var result = _databaseContext.OrderShippings
                .OrderByDescending(x => x.CreatedAt);

            return result;
        }

        public async Task<bool> Delete(string ordertId)
        {
            var success = false;

            var existingOrderShipping = await GetById(ordertId);

            if (existingOrderShipping != null)
            {
                _databaseContext.OrderShippings.Remove(existingOrderShipping);

                var numberOfItemsDeleted = await _databaseContext.SaveChangesAsync();

                if (numberOfItemsDeleted == 1)
                    success = true;
            }

            return success;
        }
    }
}