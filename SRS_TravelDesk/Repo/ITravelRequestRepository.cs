using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Repo
{
    public interface ITravelRequestRepository
    {
        Task<TravelRequest> CreateRequestAsync(TravelRequest request, List<Document> documents);
        Task<IEnumerable<TravelRequest>> GetRequestsByUserAsync(int userId);
        Task<TravelRequest> GetRequestByIdAsync(int requestId);
        Task<bool> DeleteRequestAsync(int requestId, int userId);
        Task<bool> SubmitRequestAsync(int requestId);
        Task<bool> UpdateRequestAsync(TravelRequest updatedRequest);

        Task<bool> UpdateRequestStatusAsync(TravelRequestStatusUpdateDto dto);
        Task<IEnumerable<TravelRequest>> GetAllRequestsAsync();
        Task<IEnumerable<TravelRequest>> GetRequestsApprovedByManagerAsync();

        Task AddDocumentsAsync(int travelRequestId, List<Document> newDocuments);
        Task<IEnumerable<TravelRequest>> GetRequestsByStatusAsync(TravelStatus status);
    }

}
