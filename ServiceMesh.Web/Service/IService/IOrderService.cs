using ServiceMesh.Services.Web.Models.DTO;
using ServiceMesh.Web.Models;
using ServiceMesh.Web.Models.DTO;

namespace ServiceMesh.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?>CreateOrder(CartDto cartDto);
        Task<ResponseDto?>GetAllOrder(string? userId);
        Task<ResponseDto?>GetOrder(int orderId);
        Task<ResponseDto?>UpdateOrderStatus(int orderId, string newStatus);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
    }
}
