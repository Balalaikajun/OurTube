namespace OurTube.Application.Replies.VideoPlaylist
{
    /// <summary>
    /// Модель плейлиста M3U8
    /// </summary>
    public class VideoPlaylist
    {
        /// <summary>
        /// Разрешение видео (например, 720, 1080).
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// Имя файла видео.
        /// </summary>
        public required string FileName { get; set; }

        /// <summary>
        /// Имя хранилища (bucket), где находится видеофайл.
        /// </summary>
        public required string Bucket { get; set; }
    }
}