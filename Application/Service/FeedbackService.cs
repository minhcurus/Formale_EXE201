using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
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

        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public FeedbackService(FeedbackRepository feedbackRepo, ProductRepository productRepo, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _feedbackRepository = feedbackRepo;
            _productRepository = productRepo;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
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
            feedback.ImageURL = dto.ImageFile != null ? await _cloudinaryService.UploadImageAsync(dto.ImageFile) : "";

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
            if (dto.ImageFile != null)
                feedback.ImageURL = await UpdateFeedbackImageAsync(feedback, dto.ImageFile);

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

        private async Task<string> UpdateFeedbackImageAsync(Feedback feedback, IFormFile newImage)
        {
            if (!string.IsNullOrEmpty(feedback.ImageURL))
            {
                var uri = new Uri(feedback.ImageURL);
                var fileName = uri.Segments.Last();
                var publicId = $"products/{fileName.Substring(0, fileName.LastIndexOf('.'))}";
                await _cloudinaryService.DeleteImageAsync(publicId);
            }
            return await _cloudinaryService.UploadImageAsync(newImage);
        }
    }

}
