using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Models.Entities;
using SRS_TravelDesk.Repo;

namespace SRS_TravelDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTravelController : ControllerBase
    {
        private readonly ITravelRequestRepository _travelRepo;
        private readonly IUserRepository _userRepo;

        public EmployeeTravelController(ITravelRequestRepository travelRepo, IUserRepository userRepo)
        {
            _travelRepo = travelRepo;
            _userRepo = userRepo;
        }
        private TravelRequestDetailDto MapToDto(TravelRequest request)
        {
            return new TravelRequestDetailDto
            {
                Id = request.Id,
                UserId = request.UserId,
                EmployeeName = $"{request.RequestedBy.FirstName} {request.RequestedBy.LastName}",
                RequestNumber = request.RequestNumber,
                ProjectName = request.ProjectName,
                DepartmentName = request.DepartmentName,
                ReasonForTravelling = request.ReasonForTravelling,
                BookingType = request.BookingType,
                Status = request.Status,
                TravelDate = request.TravelDate,
                AadharCardNumber = request.AadharCardNumber,
                PassportNumber = request.PassportNumber,
                DaysOfStay = request.DaysOfStay,
                MealRequired = request.MealRequired,
                MealPreference = request.MealPreference,
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate,
                Comments = request.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CommentedByUserId = c.CommentedByUserId,
                    CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}"
                }).ToList(),
                Documents = request.Documents.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    DocumentType = d.DocumentType.ToString()
                }).ToList()
            };
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRequest([FromBody] TravelRequestCreateDto dto)
        {
            if (dto.UserId == null)
                return BadRequest("UserId is required.");

            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            // Conditional validation
            if (dto.BookingType == BookingType.AirTicketOnly

 || dto.BookingType == BookingType.AirAndHotel)
            {
                if (string.IsNullOrWhiteSpace(dto.AadharCardNumber))
                    return BadRequest("Aadhar Card Number is required for Air Ticket booking.");

                if (dto.TravelDate == default)
                    return BadRequest("Travel Date is required for Air Ticket booking.");

                var isInternational = dto.Documents?.Any(d => d.DocumentType == "Passport") ?? false;
                if (isInternational)
                {
                    if (string.IsNullOrWhiteSpace(dto.PassportNumber))
                        return BadRequest("Passport Number is required for international travel.");

                    var hasDocs = dto.Documents.Any(d => d.DocumentType == "Passport" || d.DocumentType == "Visa");
                    if (!hasDocs)
                        return BadRequest("Passport and Visa documents are required for international travel.");
                }
            }

            if (dto.BookingType == BookingType.HotelOnly || dto.BookingType == BookingType.AirAndHotel)
            {
                if (dto.DaysOfStay == null || dto.DaysOfStay <= 0)
                    return BadRequest("Days of stay is required for hotel booking.");

                if (string.IsNullOrWhiteSpace(dto.MealRequired))
                    return BadRequest("Meal Required is required for hotel booking.");

                if (string.IsNullOrWhiteSpace(dto.MealPreference))
                    return BadRequest("Meal Preference is required for hotel booking.");
            }

            //  Create TravelRequest entity
            var request = new TravelRequest
            {
                UserId = dto.UserId,
                RequestedBy = user,
                ProjectName = dto.ProjectName,
                DepartmentName = dto.DepartmentName,
                ReasonForTravelling = dto.ReasonForTravelling,
                BookingType = dto.BookingType,
                TravelDate = dto.TravelDate,
                AadharCardNumber = dto.AadharCardNumber,
                PassportNumber = dto.PassportNumber,
                DaysOfStay = dto.DaysOfStay,
                MealRequired = dto.MealRequired,
                MealPreference = dto.MealPreference,
                Status = TravelStatus.Submitted,
                CreatedDate = DateTime.UtcNow
            };

            var documents = dto.Documents?
     .Where(d => !string.IsNullOrWhiteSpace(d.FileContentBase64) && !string.IsNullOrWhiteSpace(d.DocumentType))
     .Select(d => new Document
     {
         FileName = d.FileName,
         FileContent = Convert.FromBase64String(d.FileContentBase64),
         DocumentType = Enum.Parse<DocumentType>(d.DocumentType, ignoreCase: true)
     }).ToList() ?? new List<Document>();

            var created = await _travelRepo.CreateRequestAsync(request, documents);

            return Ok(new { created.Id, created.RequestNumber, Status = created.Status.ToString() });
        }

        [HttpGet("my-requests/{userId}")]
        public async Task<IActionResult> GetMyRequests(int userId)
        {
            var requests = await _travelRepo.GetRequestsByUserAsync(userId);

            return Ok(requests.Select(r => new
            {
                r.Id,
                r.RequestNumber,
                Status = r.Status.ToString(),
                r.ProjectName,
                r.CreatedDate
            }));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] TravelRequestCreateDto dto)
        {
            var existing = await _travelRepo.GetRequestByIdAsync(id);
            if (existing == null)
                return NotFound("Request not found.");

            if (existing.Status != TravelStatus.ReturnedToEmployee)
                return BadRequest("Request cannot be edited unless returned by Manager or Travel Admin.");

            // Apply updates
            existing.ProjectName = dto.ProjectName;
            existing.DepartmentName = dto.DepartmentName;
            existing.ReasonForTravelling = dto.ReasonForTravelling;
            existing.BookingType = dto.BookingType;
            existing.TravelDate = dto.TravelDate;
            existing.AadharCardNumber = dto.AadharCardNumber;
            existing.PassportNumber = dto.PassportNumber;
            existing.DaysOfStay = dto.DaysOfStay;
            existing.MealRequired = dto.MealRequired;
            existing.MealPreference = dto.MealPreference;
            existing.UpdatedDate = DateTime.UtcNow;

            await _travelRepo.UpdateRequestAsync(existing);

            return Ok("Request updated successfully.");
        }


        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteDraftRequest(int id, int userId)
        {
            var result = await _travelRepo.DeleteRequestAsync(id, userId);
            return result ? NoContent() : NotFound("Request not found or not deletable.");
        }

        [HttpPost("submit/{id}")]
        public async Task<IActionResult> SubmitRequest(int id)
        {
            var result = await _travelRepo.SubmitRequestAsync(id);
            return result ? Ok("Submitted successfully") : BadRequest("Submit failed");
        }

        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnToEmployee(int id)
        {
            var request = await _travelRepo.GetRequestByIdAsync(id);
            if (request == null || request.Status != TravelStatus.Submitted)
                return BadRequest("Only submitted requests can be returned.");

            request.Status = TravelStatus.ReturnedToEmployee;
            request.UpdatedDate = DateTime.UtcNow;

            await _travelRepo.UpdateRequestAsync(request);
            return Ok("Returned to employee.");
        }


        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] TravelRequestStatusUpdateDto dto)
        {
            var result = await _travelRepo.UpdateRequestStatusAsync(dto);
            return result ? Ok("Status updated successfully") : BadRequest("Invalid request or transition.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _travelRepo.GetAllRequestsAsync();
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);
        }

        [HttpGet("approved-by-manager")]
        public async Task<IActionResult> GetRequestsApprovedByManager()
        {
            var requests = await _travelRepo.GetRequestsApprovedByManagerAsync();
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);
        }


        [HttpPost("{id}/upload-documents")]
        public async Task<IActionResult> UploadAdditionalDocuments(int id, [FromBody] List<DocumentUploadDto> newDocuments)
        {
            var request = await _travelRepo.GetRequestByIdAsync(id);

            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != TravelStatus.ReturnedToEmployee)
                return BadRequest("Documents can only be uploaded when request is returned to employee.");

            if (newDocuments == null || !newDocuments.Any())
                return BadRequest("No documents provided.");

            var parsedDocs = newDocuments
                .Where(d => !string.IsNullOrWhiteSpace(d.FileContentBase64) && !string.IsNullOrWhiteSpace(d.DocumentType))
                .Select(d => new Document
                {
                    FileName = d.FileName,
                    FileContent = Convert.FromBase64String(d.FileContentBase64),
                    DocumentType = Enum.Parse<DocumentType>(d.DocumentType, ignoreCase: true)
                    // TravelRequestId is set in the repo
                }).ToList();

            await _travelRepo.AddDocumentsAsync(id, parsedDocs); 

            return Ok("Documents uploaded successfully.");
        }


        [HttpGet("by-status")]
        public async Task<IActionResult> GetRequestsByStatus([FromQuery] TravelStatus status)
        {
            var requests = await _travelRepo.GetRequestsByStatusAsync(status);
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);

        }


    }
}
