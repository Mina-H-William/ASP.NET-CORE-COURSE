using Microsoft.AspNetCore.Mvc;

namespace ControllersAssignment.Controllers
{
    public class BankController : Controller
    {

        private record BankAccount(int accountNumber, string accountHolderName, int currentBalance);

        private BankAccount Account = new BankAccount(1001, "Example name", 5000);

        [Route("/")]
        public IActionResult Index()
        {
            return Content("Welcome to the best Bank");
        }

        [Route("/account-details")]
        public IActionResult accountDetails()
        {
            return Json(Account);
        }

        [Route("/account-statment")]
        public IActionResult accountStatment()
        {
            return File("/Asp.net.pdf","application/pdf");
        }

        [Route("/get-current-balance/{id}")]
        public IActionResult getCurrentBalance()
        {
            if (!Request.RouteValues.ContainsKey("id"))
            {
                return BadRequest("Account number should be supplied");
            }

            int id = Convert.ToInt32(Request.RouteValues["id"]);

            if (id != 1001)
            {
                return BadRequest("Account number should be 1001");
            }

            return Content($"Account balance equal to {Account.currentBalance}");
        }
    }
}
