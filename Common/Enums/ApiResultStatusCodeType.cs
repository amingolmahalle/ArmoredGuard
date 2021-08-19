using System.ComponentModel.DataAnnotations;

namespace Common.Enums
{
    public enum ApiResultStatusCodeType
    {
        [Display(Name = "The operation was successful")]
        Success = 0,

        [Display(Name = "an error occurred on the server")]
        ServerError = 1,

        [Display(Name = "submitted parameters are not valid")]
        BadRequest = 2,

        [Display(Name = "not found")] NotFound = 3,

        [Display(Name = "list is empty")] ListEmpty = 4,

        [Display(Name = "an error occurred processing")]
        LogicError = 5,

        [Display(Name = "unauthorized")] UnAuthorized = 6,

        [Display(Name = "no content")] NoContent = 7
    }
}