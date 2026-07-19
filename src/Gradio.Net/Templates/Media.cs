
namespace Gradio.Net
{
    public static class Media
    {
        private static readonly string MediaRoot = Path.Combine(AppContext.BaseDirectory, "media_assets");
        private static readonly List<string> MediaPaths = new List<string>
        {
            Path.Combine(MediaRoot, "images"),
            Path.Combine(MediaRoot, "videos"),
            Path.Combine(MediaRoot, "audio"),
            Path.Combine(MediaRoot, "models3d"),
            Path.Combine(MediaRoot, "data")
        };

        private static string GetMediaPath(string mediaType, string filename = null)
        {
            var mediaDir = Path.Combine(MediaRoot, mediaType);

            if (!Directory.Exists(mediaDir))
            {
                throw new ArgumentException($"Media directory not found: {mediaDir}");
            }

            if (filename == null)
            {
                // Get a random file from the directory
                var mediaFiles = Directory.GetFiles(mediaDir);
                if (mediaFiles.Length == 0)
                {
                    throw new ArgumentException($"No media files found in {mediaDir}");
                }
                var random = new Random();
                var fileIndex = random.Next(mediaFiles.Length);
                return Path.GetFullPath(mediaFiles[fileIndex]);
            }
            else
            {
                if (filename.StartsWith("http://") || filename.StartsWith("https://"))
                {
                    return filename;
                }

                var filePath = Path.Combine(mediaDir, filename);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Media file not found: {filePath}");
                }
                return Path.GetFullPath(filePath);
            }
        }

        public static string GetImage(string filename = null)
        {
            return GetMediaPath("images", filename);
        }

        public static string GetVideo(string filename = null)
        {
            return GetMediaPath("videos", filename);
        }

        public static string GetAudio(string filename = null)
        {
            return GetMediaPath("audio", filename);
        }

        public static string GetModel3D(string filename = null)
        {
            return GetMediaPath("models3d", filename);
        }

        public static string GetFile(string filename = null)
        {
            return GetMediaPath("data", filename);
        }
    }
}
