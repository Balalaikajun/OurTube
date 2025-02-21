using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs
{
    public class VideoPreviewDTO
    {
        public string FileName { get; set; }
        public string FileDirInStorage { get; set; }
        public string BucketName { get; set; }
    }
}
