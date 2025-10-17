using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace Movies.Core.Services
{

    public class ReviewServices : IReviewServices
    {
        public readonly IReviewRepository _reviewRepository;
        public readonly IUserRepository _userRepository;
        public readonly IMovieRepository _movieRepository;
        public ReviewServices(IReviewRepository reviewRepository,IUserRepository userRepository,IMovieRepository movieRepository)
        {
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
        }
        public async Task DeleteReviewAsync(Review review)
        {
            await _reviewRepository.DeleteReviewAsync(review);
        }

        public Task<IEnumerable<Review>> GetAllReviewAsync()
        {
            return _reviewRepository.GetAllReviewAsync();
        }

        public Task<Review> GetReviewAsync(int id)
        {
            return _reviewRepository.GetReviewAsync(id);
        }

        public Task InsertReviewAsync(Review review)
        {
            return _reviewRepository.InsertReviewAsync(review);
        }

        public Task UpdateReviewAsync(Review review)
        {
            if(review.Grade < 1 || review.Grade > 10)
            {
                throw new Exception("La calificacion debe estar entre 1 y 10");
            }
            if(ContainsForbiddenWords(review.Description))
            {
                throw new Exception("La descripcion contiene palabras prohibidas");
            }
            return _reviewRepository.InsertReviewAsync(review);
        }

        //lista de palabras prohibidas
        public readonly List<string> ForbiddenWords = new List<string>
            {
                "mierda",
                "puta",
                "cabron",
                "joder",
                "coño"
            };

        public bool ContainsForbiddenWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            foreach (var word in ForbiddenWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }   
        public async Task InsertReviewByUserAsync(Review review)
        {
            if(review.Grade < 1 || review.Grade > 10)
            {
                throw new Exception("La calificacion debe estar entre 1 y 10");
            }
            if (ContainsForbiddenWords(review.Description))
            {
                throw new Exception("La descripcion contiene palabras prohibidas");
            }

            await _reviewRepository.InsertReviewAsync(review);
        }
    }
}
