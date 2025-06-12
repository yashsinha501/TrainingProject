using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Data;
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
            request.RequestNumber = "REQ-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
            request.CreatedDate = DateTime.UtcNow;
            request.Status = TravelStatus.Draft;

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

            // Optional: send notification to manager (you can implement this later)
            // await _emailService.SendToManager(request);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
