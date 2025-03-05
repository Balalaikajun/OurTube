using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Views
{
    public class ViewPostDTO
    {
        public int VideoId { get; set; }
        public long EndTime { get; set; } = 0;
    }
}
