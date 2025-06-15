using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Models.Entities;
using SRS_TravelDesk.Repo;
using System.Security.Claims;

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
              
                EmployeeName = request.RequestedBy != null
                    ? $"{request.RequestedBy.FirstName} {request.RequestedBy.LastName}"
                    : "Unknown",

                Department = request.RequestedBy?.Department,
                ProjectName = request.ProjectName,

                ReasonForTravelling = request.ReasonForTravelling,
                BookingType = request.BookingType.ToString(),
                Status = request.Status.ToString(),
                TravelDate = request.TravelDate.ToString("MM/dd/yyyy hh:mm tt"),
                AadharCardNumber = request.AadharCardNumber,
                PassportNumber = request.PassportNumber,
                DaysOfStay = request.DaysOfStay,
                MealRequired = request.MealRequired,
                MealPreference = request.MealPreference,
                CreatedDate = request.CreatedDate.ToString("MM/dd/yyyy hh:mm tt"),
                UpdatedDate = request.UpdatedDate?.ToString("MM/dd/yyyy hh:mm tt"),

                Comments = request.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CommentedByUserId = c.CommentedByUserId,
                    CommentedByName = c.CommentedBy != null
                        ? $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}"
                        : "Unknown"
                }).ToList() ?? new List<CommentDto>(),

                Documents = request.Documents?.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    DocumentType = d.DocumentType.ToString()
                }).ToList() ?? new List<DocumentDto>()
            };
        }

        [Authorize]
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

            // Map documents safely
            var documents = new List<Document>();
            if (dto.Documents != null && dto.Documents.Any())
            {
                foreach (var d in dto.Documents)
                {
                    if (string.IsNullOrWhiteSpace(d.FileContentBase64) || string.IsNullOrWhiteSpace(d.DocumentType))
                        continue;

                    if (!Enum.TryParse<DocumentType>(d.DocumentType, true, out var docType))
                        return BadRequest($"Invalid document type: {d.DocumentType}");

                    documents.Add(new Document
                    {
                        FileName = d.FileName,
                        FileContent = Convert.FromBase64String(d.FileContentBase64),
                        DocumentType = docType
                    });
                }
            }

            var created = await _travelRepo.CreateRequestAsync(request, documents);

            return Ok(new { created.Id, Status = created.Status.ToString() });
        }

        [Authorize]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user.");

            var requests = await _travelRepo.GetRequestsByUserAsync(userId);


            return Ok(requests.Select(r => new
            {
                r.Id,
                Status = r.Status.ToString(),
                CreatedDate = r.CreatedDate.ToString("MM/dd/yyyy hh:mm tt")
            }));
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] TravelRequestCreateDto dto)
        {
            var existing = await _travelRepo.GetRequestByIdAsync(id);
            if (existing == null)
                return NotFound("Request not found.");

            if (existing.Status != TravelStatus.ReturnedToEmployee)
                return BadRequest("Request cannot be edited unless returned by Manager or Travel Admin.");

            existing.ProjectName = dto.ProjectName;
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


        [HttpPost("update-status")]
        [Authorize(Roles = "Manager,TravelHr")]
        public async Task<IActionResult> UpdateStatus([FromBody] TravelRequestStatusUpdateDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid token.");

            dto.UpdatedByUserId = userId;

            bool result = false;

            if (userRole == "Manager")
            {
                result = await _travelRepo.UpdateStatusByManagerAsync(dto);
            }
            else if (userRole == "TravelHr")
            {
                result = await _travelRepo.UpdateStatusByTravelHrAsync(dto);
            }

            return result
                ? Ok("Status updated successfully.")
                : BadRequest("Invalid request or insufficient permissions.");
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _travelRepo.GetAllRequestsAsync();
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);
        }

        [Authorize(Roles = "Manager,TravelHr")]
        [HttpGet("approved-by-manager")]
        public async Task<IActionResult> GetRequestsApprovedByManager()
        {
            var requests = await _travelRepo.GetRequestsApprovedByManagerAsync();
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);
        }

        [HttpGet("pending/manager-approval")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPendingRequestsForManager()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int managerId))
                return Unauthorized("Invalid token.");

            var requests = await _travelRepo.GetPendingRequestsForManagerAsync(managerId);
            var response = requests.Select(MapToDto).ToList();

            return Ok(response);
        }

        [Authorize]
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
                    
                }).ToList();

            await _travelRepo.AddDocumentsAsync(id, parsedDocs); 

            return Ok("Documents uploaded successfully.");
        }

        [Authorize]
        [HttpGet("by-status")]
        public async Task<IActionResult> GetRequestsByStatus([FromQuery] TravelStatus status)
        {
            var requests = await _travelRepo.GetRequestsByStatusAsync(status);
            var response = requests.Select(MapToDto).ToList();
            return Ok(response);

        }

        [Authorize(Roles = "TravelHr")]
        [HttpPost("book")]
        public async Task<IActionResult> BookTravel([FromBody] TravelHrBookingDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user.");

            var success = await _travelRepo.BookTravelAsync(dto.TravelRequestId, dto, userId);
            return success
                ? Ok("Travel request booked successfully.")
                : BadRequest("Booking failed.");
        }

        [HttpPost("{id}/return-to-manager")]
        [Authorize(Roles = "TravelHr")]
        public async Task<IActionResult> ReturnToManager(int id, [FromBody] string? comment = null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user.");
            var success = await _travelRepo.ReturnToManagerAsync(id, comment, userId);
            return success ? Ok("Returned to manager.") : BadRequest("Failed to return to manager.");
        }

        [HttpPost("{id}/return-to-employee")]
        [Authorize(Roles = "TravelHr")]
        public async Task<IActionResult> ReturnToEmployee(int id, [FromBody] string? comment = null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user.");

            var success = await _travelRepo.ReturnToEmployeeAsync(id, comment, userId);
            return success ? Ok("Returned to employee.") : BadRequest("Failed to return to employee.");
        }

        [HttpGet("disapproved-or-closed/{userId}")]
        public async Task<IActionResult> GetDisapprovedOrClosedRequests(int userId)
        {
            var requests = await _travelRepo.GetDisapprovedAndClosedRequestsAsync(userId);

            var response = requests.Select(MapToDto).ToList();
            return Ok(response);
        }


    }
}
