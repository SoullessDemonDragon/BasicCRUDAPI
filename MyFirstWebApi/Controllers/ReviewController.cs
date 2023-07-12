using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFirstWebApi.Dto;
using MyFirstWebApi.Interfaces;
using MyFirstWebApi.Models;
using MyFirstWebApi.Repository;

namespace MyFirstWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IDogRepository _dogRepository;
        private readonly IReviewerRepository _reviewerRepository;

        public ReviewController(IReviewRepository reviewRepository,
            IMapper mapper,IDogRepository dogRepository,IReviewerRepository reviewerRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _dogRepository = dogRepository;
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(
                _reviewRepository.GetReviews());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetDog(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(
                _reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(review);
        }

        [HttpGet("dog/{dogId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForADog(int dogId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(
                _reviewRepository.GetReviewsOfADog(dogId));

            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int dogId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null)
                return BadRequest(ModelState);

            var review = _reviewRepository.GetReviews().Where(
                c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                .FirstOrDefault();
            if (review != null)
            {
                ModelState.AddModelError("", "Review already Exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(); }

            var reviewMap = _mapper.Map<Review>(reviewCreate);

            reviewMap.Dog = _dogRepository.GetDog(dogId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Created");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
                return BadRequest(ModelState);
            if (reviewId != updatedReview.Id)
                return BadRequest(ModelState);
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();

            var reviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something Went Wrong Updating Review");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully Updated Review");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", "Something Went Wrong while Deleting.");
            }
            return Ok("Successfully Deleted.");
        }
    }
}
