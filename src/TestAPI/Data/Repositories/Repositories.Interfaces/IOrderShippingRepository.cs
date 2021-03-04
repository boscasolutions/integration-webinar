using System.Linq;
using System.Threading.Tasks;
using TestApi.Models;

namespace TestApi.Data.Repositories.Repositories.Interfaces
{
    public interface IOrderShippingRepository
    {
        Task<bool> Create(OrderShipping orderShipping);

        Task<bool> Update(OrderShipping orderShipping);

        Task<OrderShipping> GetById(string OrderId);

        Task<IOrderedQueryable<OrderShipping>> GetAll();

        Task<bool> Delete(string orderId);
    }
}