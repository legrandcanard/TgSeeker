
namespace TgSeeker.Util
{
    public static class FileCacheManager
    {
        private static readonly string BaseDirPath = Path.Combine(Directory.GetCurrentDirectory(), "cache");

        public static async Task CacheFileAsync(string sourceFilePath, string dirName, string fileName)
        {
            Directory.CreateDirectory(Path.Combine(BaseDirPath, dirName));

            using var newFileFs = File.Create(GetFullFilePath(dirName, fileName));
            using var tdlibFileCacheFs = File.OpenRead(sourceFilePath);
            await tdlibFileCacheFs.CopyToAsync(newFileFs);
        }

        public static void Purge(string dirName, string fileName)
        {
            File.Delete(Path.Combine(BaseDirPath, dirName, fileName));
        }

        public static string GetFullFilePath(string dirName, string fileName)
            => Path.Combine(BaseDirPath, dirName, fileName);
    }
}
