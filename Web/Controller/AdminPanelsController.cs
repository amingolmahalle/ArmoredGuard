using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Exceptions;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Web.Controller.Base;

namespace Web.Controller
{
    public class AdminPanelsController : BaseController
    {
        private readonly IUserService _userService;

        private readonly IOAuthService _authService;

        public AdminPanelsController(IUserService userService, IOAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPut("inactivate/{id:int}")]
        public async Task<ApiResult.ApiResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    User user = await _userService.FindByIdAsync(id.ToString());

                    if (user == null)
                        throw new NotFoundException("user not found");

                    if (!user.IsActive)
                        throw new BadRequestException("User is disabled");

                    user.IsActive = false;

                    await _userService.UpdateSecurityStampAsync(user);

                    await _authService.DeleteAllUserRefreshCodesAsync(user.Id, cancellationToken);

                    transactionScope.Complete();

                    return Ok();
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    return null;
                }
            }
        }

        [HttpPut("activate/{id:int}")]
        public async Task<ApiResult.ApiResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
        {
            User user = await _userService.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            if (user.IsActive)
                throw new BadRequestException("user is active");

            user.IsActive = true;

            await _userService.UpdateSecurityStampAsync(user);

            return Ok();
        }
    }
}