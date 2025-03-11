using OurTube.Application.DTOs.Video;

namespace OurTube.Application.DTOs.Views
{
    public class ViewGetDTO
    {
        public VideoMinGetDTO Video { get; set; }
        public DateTime DateTime { get; set; }
    }
}
