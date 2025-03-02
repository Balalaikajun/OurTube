using OurTube.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OurTube.Application.Services
{
    public class VideoValidationService
    {
        public void ValidateVideo(VideoUploadDTO video)
        {
            // Валидация данных
            if (video.VideoFile.ContentType.StartsWith("video/"))
            {
                throw new FormatException("Данный формат файла не поддерживается");
            }

            if (video.PreviewFile.ContentType.StartsWith("image/"))
            {
                throw new FormatException("Данный формат файла не поддерживается");
            }

            if (video.VideoPostDTO.Title == "con")
            {
                throw new FormatException("Данное название не соответствует политике компании");
            }
        }
    }
}
