using AutoMapper;
using MyFirstWebApi.Data;
using MyFirstWebApi.Interfaces;
using MyFirstWebApi.Models;

namespace MyFirstWebApi.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateReview(Review review)
        {
            _context.Add(review);
            return Save();
        }

        public bool DeleteReview(Review reviewid)
        {
            _context.Remove(reviewid);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _context.RemoveRange(reviews);
            return Save();
        }

        public Review GetReview(int reviewid)
        {
            return _context.Reviews.Where(r => r.Id == reviewid).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfADog(int dogId)
        {
            return _context.Reviews.Where(r => r.Dog.Id == dogId).ToList();
        }

        public bool ReviewExists(int reviewid)
        {
            return _context.Reviews.Any(r => r.Id == reviewid);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }
    }
}
