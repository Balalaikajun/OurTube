using OurTube.Application.DTOs.Video;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Views
{
    public class ViewGetDTO
    {
        public VideoMinGetDTO Video { get; set; }
        public DateTime DateTime { get; set; }
    }
}
