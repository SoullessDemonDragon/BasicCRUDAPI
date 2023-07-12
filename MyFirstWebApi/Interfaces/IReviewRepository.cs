using MyFirstWebApi.Models;

namespace MyFirstWebApi.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewid);
        ICollection<Review> GetReviewsOfADog(int dogId);
        bool ReviewExists(int reviewid);
        bool CreateReview(Review review);
        bool UpdateReview(Review review);
        bool DeleteReview(Review reviewid);
        bool DeleteReviews(List<Review> reviews);
        bool Save();
    }
}
