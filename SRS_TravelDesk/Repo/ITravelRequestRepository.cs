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

        Task<bool> UpdateStatusByManagerAsync(TravelRequestStatusUpdateDto dto);
        Task<bool> UpdateStatusByTravelHrAsync(TravelRequestStatusUpdateDto dto);
        Task<IEnumerable<TravelRequest>> GetAllRequestsAsync();
        Task<IEnumerable<TravelRequest>> GetRequestsApprovedByManagerAsync();

        Task AddDocumentsAsync(int travelRequestId, List<Document> newDocuments);
        Task<IEnumerable<TravelRequest>> GetRequestsByStatusAsync(TravelStatus status);

        Task<IEnumerable<TravelRequest>> GetPendingRequestsForManagerAsync(int managerId);

        Task<bool> BookTravelAsync(int requestId, TravelHrBookingDto dto, int updatedByUserId);
        Task<bool> ReturnToManagerAsync(int requestId, string? comment, int updatedByUserId);
        Task<bool> ReturnToEmployeeAsync(int requestId, string? comment, int updatedByUserId);
        Task<IEnumerable<TravelRequest>> GetDisapprovedAndClosedRequestsAsync(int userId);
        Task<IEnumerable<TravelRequest>> GetRequestsReturnedToManagerAsync(int managerId);
        Task<IEnumerable<TravelRequest>> GetRequestsReturnedToEmployeeAsync(int employeeId);
        Task<IEnumerable<TravelRequest>> GetApprovedRequestsViaManagerAsync(int managerId);
        Task<IEnumerable<TravelRequest>> GetBookedRequestsAsync();
    }
}
