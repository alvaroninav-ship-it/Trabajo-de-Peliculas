using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Exceptions;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;

namespace Movies.Core.Services
{

    public class ReviewServices : IReviewServices
    {
        public readonly IUnitOfWork _unitOfWork;
        public ReviewServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }

        public async Task<ResponseData> GetAllReviewAsync(
         ReviewQueryFilter reviewQueryFilter)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAll();
            if (reviewQueryFilter.UserId != null)
            {
                reviews = reviews.Where(c => c.UserId == reviewQueryFilter.UserId);
            }
            if (reviewQueryFilter.Date != null)
            {
                reviews = reviews.Where(c => c.Date.ToShortDateString() ==
                reviewQueryFilter.Date);
            }
            if (reviewQueryFilter.Description != null)
            {
                reviews = reviews.Where(x => x.Description.ToLower().Contains(reviewQueryFilter.Description.ToLower()));
            }
            if (reviewQueryFilter.MovieId != null)
            {
                reviews = reviews.Where(c => c.MovieId == reviewQueryFilter.MovieId);
            }
            if (reviewQueryFilter.Grade != null)
            {
                reviews = reviews.Where(c => c.Grade == reviewQueryFilter.Grade);
            }

            
            var totalCount = reviews.Count();

            
            var pageReviews = PageList<object>.Create(reviews, reviewQueryFilter.PageNumber, reviewQueryFilter.PageSize);

            if (pageReviews.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de reviews recuperados correctamente" } },
                    Pagination = pageReviews,
                    StatusCode = HttpStatusCode.OK,
                    totalcount = totalCount
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pageReviews,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }
        public async Task DeleteReviewAsync(Review review)
        {
           await _unitOfWork.ReviewRepository.Delete(review.Id);
        }

        public Task<IEnumerable<Review>> GetAllReviewAsyncDapper()
        {
            return _unitOfWork.ReviewRepository.GetAll();
        }

        public Task<Review> GetReviewAsync(int id)
        {
            return _unitOfWork.ReviewRepository.GetById(id);
        }

        public Task InsertReviewAsync(Review review)
        {
            return _unitOfWork.ReviewRepository.Add(review);
        }

        public Task UpdateReviewAsync(Review review)
        {
            if(review.Grade < 1 || review.Grade > 10)
            {
                throw new BusinessException("La calificacion debe estar entre 1 y 10");
            }
            if(ContainsForbiddenWords(review.Description))
            {
                throw new BusinessException("La descripcion contiene palabras prohibidas");
            }
            return _unitOfWork.ReviewRepository.Update(review);
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

            await _unitOfWork.ReviewRepository.Add(review);
        }

        public async Task<IEnumerable<Top10MostCommentedReviews>> GetTop10MostCommentedReviews()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetTop10MostCommentedReviews();
            return reviews;
        }

        public async Task<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>> GetReviewsThatWereDoneByUsers20YearsOldOrYounger()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetReviewsThatWereDoneByUsers20YearsOldOrYounger();
            return reviews;
        }

        public async Task<IEnumerable<ReviewsThatRefersAnSpecificGenre>> GetReviewsThatRefersAnSpecificGenre(string genre)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetReviewsThatRefersAnSpecificGenre(genre);
            return reviews;
        }
    }
}
