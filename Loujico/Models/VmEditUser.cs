using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace Loujico.Models
{
    public class VmEditUser
    {

        [StringLength(32, MinimumLength = 1, ErrorMessage = "معرّف المستخدم يجب أن يكون بين 1 و 32 حرفاً")]
        public string userid { get; set; }


        [Required(ErrorMessage = "اسم المستخدم مطلوب ولا يمكن أن يكون فارغاً")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم المستخدم يجب أن يكون بين 2 و 100 حرف")]
        [RegularExpression(@"^[a-zA-Z0-9_\.\-@]+$",
            ErrorMessage = "اسم المستخدم يمكن أن يحتوي فقط على أحرف إنجليزية، أرقام، النقاط، الواصلات، وعلامة @")]
        public string username { get; set; }


        [Required(ErrorMessage = "البريد الإلكتروني مطلوب ولا يمكن أن يكون فارغاً")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [StringLength(256, ErrorMessage = "البريد الإلكتروني يجب أن لا يتجاوز 256 حرفاً")]
        public string Email { get; set; }

        [Required(ErrorMessage = "يجب تحديد دور للمستخدم")]
        [StringLength(50, ErrorMessage = "اسم الدور يجب أن لا يتجاوز 50 حرفاً")]
        [DisplayName("الدور المحدد")]
        public string selectedRole { get; set; }

        /// <summary>
        /// كلمة المرور الجديدة للمستخدم (اختياري)
        /// في حالة التعديل، إذا تم إدخال كلمة مرور جديدة سيتم تحديثها
        /// يجب أن تستوفي متطلبات القوة إذا تم إدخالها
        /// </summary>
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون بين 6 و 100 حرف")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
            ErrorMessage = "كلمة المرور يجب أن تحتوي على حرف كبير على الأقل، حرف صغير على الأقل، رقم على الأقل، ورمز خاص واحد على الأقل")]
        public string password { get; set; }

        public List<SelectListItem>? roles { get; set; }
    }
}
