using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace zwajapp.API.DTOS
{
    public class UserTransferRegisterDto
    {
        [Required(ErrorMessage ="اسم المستخدم مطلوب")]
        public string  username { get; set; }

        [MaxLength(8,ErrorMessage ="يجب ان لا تزيد كلمة المرور عن ثمانية احرف"),MinLength(4,ErrorMessage ="يجب ان لاتقل كلمة المرور عن اربعة احرف"),]
        public string password { get; set; }
    }
}
