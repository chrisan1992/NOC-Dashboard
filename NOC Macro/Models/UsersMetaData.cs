using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NOC_Macro.Models
{

    [MetadataType(typeof(UsersMetaData))]
    public partial class Users
    {

    }

    public class UsersMetaData
    {
        [Required(ErrorMessage ="Email is required.")]
        public String Email { get; set; }

        [Required(ErrorMessage ="Username is required")]
        public String UserName { get; set; }

        [Required(ErrorMessage ="The password is required")]
        public String Emailpass { get; set; }
    }

    
}