using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Filter;

namespace Web.Controller.Base
{
    [ApiResultFilter]
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BaseController : ControllerBase
    {
    }
}