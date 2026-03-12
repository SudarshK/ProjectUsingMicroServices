
using ServiceMesh.Services.OrderAPI.Models.DTO;

namespace ServiceMesh.OrderAPI.Models.Dto
{
    public class StripeRequestDto
    {
        public string? StripeSessionURL { get; set; }
        public string? StripeSessionId { get; set; }
        public string ApprovedUrl { get; set; } 
        public string CancelUrl { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
