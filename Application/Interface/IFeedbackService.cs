using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IFeedbackService
    {
        Task CreateFeedbackAsync(int userId, Guid productId, FeedbackRequestDto dto);
        Task UpdateFeedbackAsync(int userId, Guid productId, FeedbackRequestDto dto);
        Task<(int TotalFeedbacks, double AverageRating)> GetProductFeedbackStatsAsync(Guid productId);
        Task<IEnumerable<FeedbackDto>> GetProductFeedbacksAsync(Guid productId);
        Task<IEnumerable<FeedbackDto>> GetUserFeedbacksAsync(int userId);
        Task DeleteFeedbackAsync(Guid feedbackId);
        Task<IEnumerable<FeedbackDto>> GetAllFeedbacksAsync();

    }
}
