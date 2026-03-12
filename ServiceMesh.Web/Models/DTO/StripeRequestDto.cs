using ServiceMesh.Services.Web.Models.DTO;

namespace ServiceMesh.Web.Models.DTO
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
