using System;
using Common.Enums;

namespace Services.Dtos;

public class CreateUserDto
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string OtpCode { get; set; }

    public string FullName { get; set; }

    public RegisterType RegisterType { get; set; }

    public int RoleId { get; set; }

    public DateTime? BirthDate { get; set; }

    public GenderType Gender { get; set; }
}