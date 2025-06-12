using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Data;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Repo
{
    public class TravelRequestRepository : ITravelRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public TravelRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TravelRequest> CreateRequestAsync(TravelRequest request, List<Document> documents)
        {
            request.RequestNumber = "REQ-" + Guid.NewGuid().ToString();
            request.CreatedDate = DateTime.UtcNow;
            request.Status = TravelStatus.Submitted;

            _context.TravelRequests.Add(request);
            await _context.SaveChangesAsync(); 
            if (documents != null && documents.Any())
            {
                foreach (var doc in documents)
                {
                    doc.TravelRequestId = request.Id;
                    _context.Documents.Add(doc);
                }

                await _context.SaveChangesAsync();
            }

            return request;
        }

        public async Task<IEnumerable<TravelRequest>> GetRequestsByUserAsync(int userId)
        {
            return await _context.TravelRequests
                .Include(r => r.Comments)
                .Include(r => r.Documents)
                .Include(r => r.RequestedBy)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<TravelRequest?> GetRequestByIdAsync(int requestId)
        {
            return await _context.TravelRequests
                .Include(r => r.Comments)
                .Include(r => r.Documents)
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        public async Task<bool> UpdateRequestAsync(TravelRequest updatedRequest)
        {
            _context.TravelRequests.Update(updatedRequest);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteRequestAsync(int requestId, int userId)
        {
            var request = await _context.TravelRequests
                .FirstOrDefaultAsync(r => r.Id == requestId && r.UserId == userId && r.Status == TravelStatus.Draft);

            if (request == null)
                return false;

            _context.TravelRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitRequestAsync(int requestId)
        {
            var request = await _context.TravelRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Status != TravelStatus.Draft)
                return false;

            request.Status = TravelStatus.Submitted;
            request.UpdatedDate = DateTime.UtcNow;

           

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRequestStatusAsync(TravelRequestStatusUpdateDto dto)
        {
            var request = await _context.TravelRequests
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(r => r.Id == dto.RequestId);

            if (request == null || request.Status == TravelStatus.Closed)
                return false;

            // Control editing logic
            if (request.Status == TravelStatus.Submitted &&
                (dto.NewStatus == TravelStatus.ApprovedByManager || dto.NewStatus == TravelStatus.ReturnedToEmployee))
            {
                request.Status = dto.NewStatus;
                request.UpdatedDate = DateTime.UtcNow;
            }
            else if (request.Status == TravelStatus.ApprovedByManager &&
                     dto.NewStatus == TravelStatus.BookedByAdmin)
            {
                request.Status = dto.NewStatus;
                request.UpdatedDate = DateTime.UtcNow;
            }
            else if (request.Status == TravelStatus.BookedByAdmin &&
                     dto.NewStatus == TravelStatus.Closed)
            {
                request.Status = dto.NewStatus;
                request.UpdatedDate = DateTime.UtcNow;
            }
            else if (dto.NewStatus == TravelStatus.ReturnedToEmployee)
            {
                request.Status = TravelStatus.ReturnedToEmployee;
                request.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                return false; // Invalid transition
            }

            // Add comment if provided
            if (!string.IsNullOrWhiteSpace(dto.Comment))
            {
                var commenter = await _context.Users.FindAsync(dto.UpdatedByUserId);
                if (commenter == null) return false;

                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = dto.UpdatedByUserId,
                    CommentedBy = commenter,
                    Content = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TravelRequest>> GetAllRequestsAsync()
        {
            return await _context.TravelRequests
                .Include(r => r.Comments)
                    .ThenInclude(c => c.CommentedBy)
                .Include(r => r.Documents)
                .Include(r => r.RequestedBy)
                .ToListAsync();
        }

        public async Task<IEnumerable<TravelRequest>> GetRequestsApprovedByManagerAsync()
        {
            return await _context.TravelRequests
                .Where(r => r.Status == TravelStatus.ApprovedByManager)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.CommentedBy)
                .Include(r => r.Documents)
                .Include(r => r.RequestedBy)
                .ToListAsync();
        }


        public async Task AddDocumentsAsync(int travelRequestId, List<Document> newDocuments)
        {
            // Get all existing documents for the request
            var existingDocs = _context.Documents.Where(d => d.TravelRequestId == travelRequestId);

            // Remove old documents
            _context.Documents.RemoveRange(existingDocs);
            await _context.SaveChangesAsync();

            // Add new documents
            foreach (var doc in newDocuments)
            {
                doc.TravelRequestId = travelRequestId;
            }

            _context.Documents.AddRange(newDocuments);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TravelRequest>> GetRequestsByStatusAsync(TravelStatus status)
        {
            return await _context.TravelRequests
                .Include(r => r.Documents)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.CommentedBy)
                .Include(r => r.RequestedBy)
                .Where(r =>r.Status == status)
                .ToListAsync();
        }



    }

}
