using DEVTRACKR.API.Entities;
using DEVTRACKR.API.Models;
using DEVTRACKR.API.Persistence;
using DEVTRACKR.API.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DEVTRACKR.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase

    {
        private readonly IPackageRepository _repository;
        private readonly ISendGridClient _client;
        public PackagesController(IPackageRepository repository, ISendGridClient client)
        {
            _repository = repository;
            _client = client;
        }
        
        // GET api/packages
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _repository.GetAll();

            return Ok(packages);
        }

        // GET api/packages/1234-5678-1234-3212
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        // POST api/packages
        [HttpPost]
        public async Task<IActionResult> Post(AddPackageInputModel model)
        {
            if (model.Title.Length < 10)
            {
                return BadRequest("Title lenght must be at least 10 characteres long");
            }

            var package = new Package(model.Title, model.Weight);

            _repository.Add(package);

            var message = new SendGridMessage()
            {
                From = new EmailAddress("kojege8396@runchet.com", "Moraess"),
                Subject = "Your Package has been dispatched.",
                PlainTextContent = $"Your Package with code {package.Code} has been dispatched."
            };

            message.AddTo(model.SenderEmail, model.SenderName);

            var response = await _client.SendEmailAsync(message);

            return CreatedAtAction("GetByCode", new { code = package.Code}, package);
        }

        // POST api/packages/1234-5678-1234-3212/updates
        [HttpPost("{code}/updates")]
        public async Task<IActionResult> PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }

            package.AddUpdate(model.Status, model.Delivered);
            _repository.Update(package);

            //var message = new SendGridMessage
            //{
            //    From = new EmailAddress("kojege8396@runchet.com", "Moraess"),
            //    Subject = "We have a update of your package.",
            //    PlainTextContent = $"Your Package status with code {package.Code} ghas changed to."
            //};

            //message.AddTo(model.SenderEmail, model.SenderName);

            //await _client.SendEmailAsync(message);

            return CreatedAtAction("GetByCode", new { code = package.Code }, package);

            return NoContent();
        }
    }
}
