using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Loujico.Models
{
    public class VmUserRoles
    {
        [StringLength(32, ErrorMessage = "معرّف المستخدم يجب أن لا يتجاوز 32 حرفاً")]
        public string userid { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم المستخدم يجب أن يكون بين 2 و 100 حرف")]
        [RegularExpression(@"^[a-zA-Z0-9_\.]+$",
            ErrorMessage = "اسم المستخدم يمكن أن يحتوي فقط على أحرف إنجليزية، أرقام، نقاط، وشرط سفلي")]
        public string username { get; set; }


        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [StringLength(256, ErrorMessage = "البريد الإلكتروني يجب أن لا يتجاوز 256 حرفاً")]
        public string Email { get; set; }


        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم الأول يجب أن لا يتجاوز 100 حرف")]
        [RegularExpression(@"^[\p{IsArabic} ]+$",
            ErrorMessage = "الاسم الأول يمكن أن يحتوي فقط على أحرف عربية ومسافات")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم الأخير يجب أن لا يتجاوز 100 حرف")]
        [RegularExpression(@"^[\p{IsArabic} ]+$",
            ErrorMessage = "الاسم الأخير يمكن أن يحتوي فقط على أحرف عربية ومسافات")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "يجب تحديد دور واحد على الأقل للمستخدم")]
        [MinLength(1, ErrorMessage = "يجب تحديد دور واحد على الأقل للمستخدم")]
        public IEnumerable<string> roles { get; set; }
    }
}