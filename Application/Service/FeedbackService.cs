using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackRepository _feedbackRepository;
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;

        public FeedbackService(FeedbackRepository feedbackRepo, ProductRepository productRepo, IMapper mapper)
        {
            _feedbackRepository = feedbackRepo;
            _productRepository = productRepo;
            _mapper = mapper;
        }


        public async Task CreateFeedbackAsync(int userId, Guid productId, FeedbackRequestDto dto)
        {
            var exists = await _feedbackRepository.Query()
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

            if (exists)
                throw new Exception("Feedback already exists.");

            var feedback = _mapper.Map<Feedback>(dto);
            feedback.UserId = userId;
            feedback.ProductId = productId;

            await _feedbackRepository.AddAsync(feedback);
            await UpdateProductStats(productId);
        }
        public async Task UpdateFeedbackAsync(int userId, Guid productId, FeedbackRequestDto dto)
        {
            var feedback = await _feedbackRepository.Query()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (feedback == null)
                throw new Exception("Feedback not found.");

            feedback.Rating = dto.Rating;
            feedback.Description = dto.Description;

            await _feedbackRepository.UpdateAsync(feedback);
            await UpdateProductStats(productId);
        }

        private async Task UpdateProductStats(Guid productId)
        {
            var feedbacks = await _feedbackRepository.Query()
                .Where(f => f.ProductId == productId)
                .ToListAsync();

            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.TotalFeedbacks = feedbacks.Count;
                product.AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
                product.UpdatedAt = DateTime.UtcNow;
                await _productRepository.UpdateAsync(product);
            }
        }
        public async Task<(int TotalFeedbacks, double AverageRating)> GetProductFeedbackStatsAsync(Guid productId)
        {
            var feedbacks = await _feedbackRepository.Query()
                .Where(f => f.ProductId == productId)
                .ToListAsync();

            int total = feedbacks.Count;
            double average = total > 0 ? feedbacks.Average(f => f.Rating) : 0;

            return (total, average);
        }

        public async Task<IEnumerable<FeedbackDto>> GetProductFeedbacksAsync(Guid productId)
        {
            var feedbacks = await _feedbackRepository.Query()
                .Where(f => f.ProductId == productId)
                .Include(f => f.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FeedbackDto>>(feedbacks);
        }
        public async Task<IEnumerable<FeedbackDto>> GetUserFeedbacksAsync(int userId)
        {
            var feedbacks = await _feedbackRepository.Query()
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .Include(f => f.Product)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FeedbackDto>>(feedbacks);
        }
    }

}
