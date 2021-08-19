using System.ComponentModel.DataAnnotations;

namespace Web.Models.RequestModels.Role
{
    public class CreateRoleRequest
    {
        [Required] [StringLength(30)] public string RoleName { get; set; }

        [StringLength(50)] public string Description { get; set; }
    }
}