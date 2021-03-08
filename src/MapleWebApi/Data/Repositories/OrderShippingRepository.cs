using System;
using System.Linq;
using System.Threading.Tasks;
using MapleWebApi.Data.Repositories.Repositories.Interfaces;
using MapleWebApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapleWebApi.Data.Repositories
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
            bool success = false;

            _databaseContext.OrderShippings.Add(orderShipping);

            int numberOfItemsCreated = await _databaseContext.SaveChangesAsync();

            if (numberOfItemsCreated == 1)
            {
                success = true;
            }

            return success;
        }

        public async Task<bool> Update(OrderShipping orderShipping)
        {
            bool success = false;

            OrderShipping existingOrderShipping = await GetById(orderShipping.OrderId);

            if (existingOrderShipping != null)
            {
                existingOrderShipping.State = orderShipping.State;
                existingOrderShipping.LastUpdatedDateTimeUtc = orderShipping.LastUpdatedDateTimeUtc;

                _databaseContext.OrderShippings.Attach(existingOrderShipping);

                int numberOfItemsUpdated = await _databaseContext.SaveChangesAsync();

                if (numberOfItemsUpdated == 1)
                {
                    success = true;
                }
            }

            return success;
        }

        public async Task<OrderShipping> GetById(string orderId)
        {
            OrderShipping result = _databaseContext.OrderShippings
                                .Where(x => x.OrderId == orderId)
                                .FirstOrDefault();

            return result;
        }

        public async Task<IOrderedQueryable<OrderShipping>> GetAll()
        {
            IOrderedQueryable<OrderShipping> result = _databaseContext.OrderShippings
                .OrderByDescending(x => x.CreatedAt);

            return result;
        }

        public async Task<bool> Delete(string ordertId)
        {
            bool success = false;

            OrderShipping existingOrderShipping = await GetById(ordertId);

            if (existingOrderShipping != null)
            {
                _databaseContext.OrderShippings.Remove(existingOrderShipping);

                int numberOfItemsDeleted = await _databaseContext.SaveChangesAsync();

                if (numberOfItemsDeleted == 1)
                {
                    success = true;
                }
            }

            return success;
        }
    }
}