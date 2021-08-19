using System.ComponentModel.DataAnnotations;

namespace Common.Enums
{
    public enum GenderType
    {
        [Display(Name = "Male")] Male = 1,

        [Display(Name = "Female")] Female = 2
    }
}