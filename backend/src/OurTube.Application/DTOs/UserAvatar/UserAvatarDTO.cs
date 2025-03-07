using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.UserAvatar
{
    public class UserAvatarDTO
    {
        public string FileName { get; set; }
        public string FileDirInStorage { get; set; }
        public string BucketName { get; set; }
    }
}
