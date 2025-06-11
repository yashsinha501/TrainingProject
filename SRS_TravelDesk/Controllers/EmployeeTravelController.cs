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

            // 🏗️ Create TravelRequest entity
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
                Status = TravelStatus.Draft,
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

            return Ok(new { created.Id, created.RequestNumber });
        }

        [HttpGet("my-requests/{userId}")]
        public async Task<IActionResult> GetMyRequests(int userId)
        {
            var requests = await _travelRepo.GetRequestsByUserAsync(userId);

            return Ok(requests.Select(r => new
            {
                r.Id,
                r.RequestNumber,
                r.Status,
                r.ProjectName,
                r.CreatedDate
            }));
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
    }
}
