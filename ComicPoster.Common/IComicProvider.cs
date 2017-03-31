namespace ComicPoster.Common
{
    public interface IComicProvider
    {
        Comic DownloadComic(string oldId);
    }
}