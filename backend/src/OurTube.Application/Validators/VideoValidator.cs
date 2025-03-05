using OurTube.Application.DTOs.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OurTube.Application.Validators
{
    public class VideoValidator
    {
        public void ValidateVideo(VideoUploadDTO video)
        {
            // Валидация данных
            if (!video.VideoFile.ContentType.StartsWith("video/"))
            {
                throw new FormatException($"Формат файла: {video.VideoFile.ContentType} - не поддерживается");
            }

            if (!video.PreviewFile.ContentType.StartsWith("image/"))
            {
                throw new FormatException($"Формат файла: {video.PreviewFile.ContentType} - не поддерживается");
            }

            if (video.VideoPostDTO.Title == "con")
            {
                throw new FormatException("Данное название не соответствует политике компании");
            }
        }
    }
}
