using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceMesh.Services.AuthAPI.Models.DTO;
using ServiceMesh.Services.AuthAPI.RabbitMQSender;
using ServiceMesh.Services.AuthAPI.Service.IService;

namespace ServiceMesh.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRabbitMQAuthMessageSender _messageBus;
        //private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public AuthApiController(IAuthService authService,IRabbitMQAuthMessageSender messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            _messageBus.SendMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if(loginResponse.User == null)
            {
                _response.IsSuccess = false ;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }
        
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody]RegistrationRequestDto model)
        {
            var assignRoleSuccess = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if(!assignRoleSuccess)
            {
                _response.IsSuccess = false ;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
