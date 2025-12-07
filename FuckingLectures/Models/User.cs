using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuckingLectures.Models.Entities
{
    public class User : IdentityUser<Guid>
    {



        public string FullName { get; set; }



        public UserState State { get; set; } = UserState.InActive;
        public string? FcmToken { get; set; }

        public bool isOTPActivated { get; set; } = false;



    }


    public enum UserState
    {
        Active = 0,
        InActive = 1,
        Banned = 2,
        Suspended = 3,

    }
    

}
