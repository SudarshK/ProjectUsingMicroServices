using Microsoft.EntityFrameworkCore;
using ServiceMesh.Services.EmailAPI.Data;
using ServiceMesh.Services.EmailAPI.Model;
using ServiceMesh.Services.EmailAPI.Model.DTO;
using System.Text;

namespace ServiceMesh.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine($"Cart Email Requested");
            message.AppendLine($"Cart Total" + cartDto.CartHeader.CartTotal);
            message.Append($"<br/>");
            message.Append($"<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append($"{item.Product.Name} x {item.Count} ");
                message.Append("</li>");
            }
            message.Append("</ul>");
            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardMessage rewardsDto)
        {
            string message = "New Order Placed <br/> Order ID : "+ rewardsDto.OrderId;
            await LogAndEmail(message, "sk@gmail.com");
        }
        
        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registeration Successful. <br/> Email : " + email;
            await LogAndEmail(message, "sk@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string mail)
        {
            try
            {
                EmailLogger emailLog = new EmailLogger()
                {
                    Email = mail,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var db = new AppDbContext(_dbOptions);
                await db.EmailLoggers.AddAsync(emailLog);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                return false;
            }
        }
    }
}
