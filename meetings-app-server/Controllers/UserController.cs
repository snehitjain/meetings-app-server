using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Services;


namespace MyApp.Controllers
{
    // Ensure you are using the correct namespace for ASP.NET Core
    public class UserController : Controller
    {
        private readonly EmailService _emailService;

        // Constructor to inject the EmailService
        public UserController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(User model)  // Changed to IActionResult
        {
            if (ModelState.IsValid)
            {
                // Save the user to the database (assuming you have user repository or context)
                // Example: _userRepository.SaveUser(model);
                // or save to the database using Entity Framework:
                // _dbContext.Users.Add(model);
                // await _dbContext.SaveChangesAsync();

                // Send email notification after registration
                try
                {
                    _emailService.SendRegistrationEmail(model.Email, model.Name);
                }
                catch (Exception ex)
                {
                    // Optionally handle exceptions here
                    ModelState.AddModelError("", $"Email could not be sent: {ex.Message}");
                    return View(model);  // Return the view with the model if email fails
                }

                // Redirect to a success page after registration
                return RedirectToAction("RegistrationSuccess");
            }

            // If model state is invalid, return to the view with validation errors
            return View(model);
        }

        // GET: /User/RegistrationSuccess
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
    }
}