﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlpineWebApi.Data.Repositories.Repositories.Interfaces;
using AlpineWebApi.Data.Services.Interfaces;
using AlpineWebApi.Models;

namespace AlpineWebApi.Data.Services
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

            bool success = await _repository.Create(orderShipping);

            if (success)
            {
                return orderShipping;
            }
            else
            {
                return null;
            }
        }

        public async Task<OrderShipping> GetById(string orderShippingId)
        {
            OrderShipping result = await _repository.GetById(orderShippingId);

            if (result == null)
            {
                return new OrderShipping() { State = "Not Found" };
            }

            return result;
        }

        public async Task<IOrderedQueryable<OrderShipping>> GetAll()
        {
            IOrderedQueryable<OrderShipping> result = await _repository.GetAll();

            if (result == null)
            {
                List<OrderShipping> notFound = new List<OrderShipping>
                {
                    new OrderShipping() { State = "Not Found" }
                };

                return notFound.AsQueryable().OrderBy(x => x.State);
            }

            return result;
        }

        public async Task<bool> Delete(string orderShippingId)
        {
            bool success = await _repository.Delete(orderShippingId);

            return success;
        }
    }
}