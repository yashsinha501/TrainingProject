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
                    .ThenInclude(c => c.CommentedBy)
                .Include(r => r.Documents)
                .Include(r => r.RequestedBy)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<TravelRequest?> GetRequestByIdAsync(int requestId)
        {
            return await _context.TravelRequests
                .Include(r => r.Comments)
                    .ThenInclude(c => c.CommentedBy)
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
                .FirstOrDefaultAsync(r => r.Id == requestId && r.UserId == userId && r.Status == TravelStatus.Submitted);

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

        public async Task<bool> UpdateStatusByManagerAsync(TravelRequestStatusUpdateDto dto)
        {
            var request = await _context.TravelRequests
                .Include(r => r.RequestedBy)
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(r => r.Id == dto.RequestId);

            if (request.Status != TravelStatus.Submitted && request.Status != TravelStatus.ReturnedToManager)
                return false;

            if (request.RequestedBy.ManagerId != dto.UpdatedByUserId)
                return false; // Only their own manager can approve

            if (dto.NewStatus != TravelStatus.ApprovedByManager &&
    dto.NewStatus != TravelStatus.ReturnedToEmployee &&
    dto.NewStatus != TravelStatus.Disapprove)
                return false; // Manager can approve, disapprove or return

            request.Status = dto.NewStatus;
            request.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Comment))
            {
                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = dto.UpdatedByUserId,
                    Content = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusByTravelHrAsync(TravelRequestStatusUpdateDto dto)
        {
            var request = await _context.TravelRequests
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(r => r.Id == dto.RequestId);

            if (request == null)
                return false;

            bool isValid = false;

            if (request.Status == TravelStatus.ApprovedByManager &&
                dto.NewStatus == TravelStatus.BookedByAdmin)
            {
                isValid = true;
            }
            else if (request.Status == TravelStatus.BookedByAdmin &&
                     dto.NewStatus == TravelStatus.Closed)
            {
                isValid = true;
            }
            else if (dto.NewStatus == TravelStatus.ReturnedToEmployee)
            {
                isValid = true;
            }
            else if (request.Status == TravelStatus.ApprovedByManager && dto.NewStatus == TravelStatus.ReturnedToManager)
            {
                isValid = true;
            }
            else if (dto.NewStatus == TravelStatus.Disapprove)
            {
                isValid = true;
            }

            if (!isValid)
                return false;

            request.Status = dto.NewStatus;
            request.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Comment))
            {
                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = dto.UpdatedByUserId,
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

        public async Task<IEnumerable<TravelRequest>> GetPendingRequestsForManagerAsync(int managerId)
        {
            return await _context.TravelRequests
                .Include(r => r.RequestedBy)
                .Include(r => r.Documents)
                .Include(r => r.Comments)
                .Where(r => (r.Status == TravelStatus.Submitted || r.Status == TravelStatus.ReturnedToManager) && r.RequestedBy.ManagerId == managerId)
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
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<bool> BookTravelAsync(int requestId, TravelHrBookingDto dto, int updatedByUserId)
        {
            var request = await _context.TravelRequests
                .Include(r => r.Documents)
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Status != TravelStatus.ApprovedByManager)
                return false;

            request.Status = TravelStatus.BookedByAdmin;
            request.UpdatedDate = DateTime.UtcNow;

            // Add documents
            foreach (var doc in dto.Documents)
            {
                if (string.IsNullOrWhiteSpace(doc.FileContentBase64)) continue;

                var document = new Document
                {
                    TravelRequestId = request.Id,
                    FileName = doc.FileName,
                    FileContent = Convert.FromBase64String(doc.FileContentBase64),
                    DocumentType = Enum.Parse<DocumentType>(doc.DocumentType, true)
                };
                _context.Documents.Add(document);
            }

            // Add comment
            if (!string.IsNullOrWhiteSpace(dto.Comment))
            {
                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = updatedByUserId,
                    Content = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> ReturnToManagerAsync(int requestId, string? comment, int updatedByUserId)
        {
            var request = await _context.TravelRequests.FindAsync(requestId);
            if (request == null || request.Status != TravelStatus.ApprovedByManager)
                return false;

            request.Status = TravelStatus.ReturnedToManager;
            request.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(comment))
            {
                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = updatedByUserId,
                    Content = comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReturnToEmployeeAsync(int requestId, string? comment, int updatedByUserId)
        {
            var request = await _context.TravelRequests.FindAsync(requestId);
            if (request == null || (request.Status != TravelStatus.Submitted && request.Status != TravelStatus.ApprovedByManager))
                return false;

            request.Status = TravelStatus.ReturnedToEmployee;
            request.UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(comment))
            {
                _context.Comments.Add(new Comment
                {
                    TravelRequestId = request.Id,
                    CommentedByUserId = updatedByUserId,
                    Content = comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TravelRequest>> GetDisapprovedAndClosedRequestsAsync(int userId)
        {
            return await _context.TravelRequests
                .Include(r => r.RequestedBy)
                .Include(r => r.Documents)
                .Include(r => r.Comments)
                .Where(r => r.UserId == userId &&
                       (r.Status == TravelStatus.ReturnedToEmployee ||
                        r.Status == TravelStatus.ReturnedToManager ||
                        r.Status == TravelStatus.Closed || r.Status == TravelStatus.Disapprove))
                .ToListAsync();
        }


    }


}
