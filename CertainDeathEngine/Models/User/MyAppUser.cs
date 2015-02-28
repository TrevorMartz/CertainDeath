using Microsoft.AspNet.Facebook;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    [Table("FBUsers")]
    public class MyAppUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }


        //[JsonProperty("picture")] // This renames the property to picture.
        //[FacebookFieldModifier("type(large)")] // This sets the picture size to large.
        //public FacebookConnection<FacebookPicture> ProfilePicture { get; set; }

        //[FacebookFieldModifier("limit(8)")] // This sets the size of the friend list to 8, remove it to get all friends.
        //public FacebookGroupConnection<MyAppUserFriend> Friends { get; set; }

        //[FacebookFieldModifier("limit(16)")] // This sets the size of the photo list to 16, remove it to get all photos.
        //public FacebookGroupConnection<FacebookPhoto> Photos { get; set; }
    }
}
