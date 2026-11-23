namespace Cairo_Cinema.API.Repositories
{
    public interface IMovieSubImageRepository : IRepository<MovieSubImage>
    {
        void RemoveRange(IEnumerable<MovieSubImage> movieSubImages);
    }
}
