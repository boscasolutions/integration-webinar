using System.Linq;
using System.Threading.Tasks;
using TestApi.Models;

namespace TestApi.Data.Services.Interfaces
{
    public interface IOrderShippingService
    {
        Task<OrderShipping> Create(OrderShipping orderShipping);

        Task<OrderShipping> Update(OrderShipping orderShipping);

        Task<OrderShipping> GetById(string orderShippingId);

        Task<IOrderedQueryable<OrderShipping>> GetAll();

        Task<bool> Delete(string orderShippingId);
    }
}