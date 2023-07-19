using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaExpenBackend.DB;
using SeaExpenBackend.Models;

namespace SeaExpenBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Get()
        {
            var userid = HttpContext.User.Claims.First(i => i.Type == "UserId").Value;
            using (var context = new SeaExpenContext())
            {
                var data = from a in context.Expenses
                           join c in context.ExpenseCategories on a.Category equals c.Id
                           where a.UserId == Convert.ToInt32(userid)
                           select new ExpenseModel
                           {
                               Id = a.Id,
                               Category = a.Category,
                               CategoryName = c.Category,
                               Amount = a.Amount,
                               RecordedDate = a.ExpenseDate,
                               Note = a.Note,
                               UserId = a.UserId,
                           };
                
                return Ok(data.ToList());
            }
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            using (var context = new SeaExpenContext()) {
                var data = (from a in context.Expenses
                           join c in context.ExpenseCategories on a.Category equals c.Id
                           where a.Id == id
                           select new ExpenseModel
                           {
                               Id = a.Id,
                               Category = a.Category,
                               CategoryName = c.Category,
                               Amount = a.Amount,
                               RecordedDate = a.ExpenseDate,
                               Note = a.Note,
                               UserId = a.UserId,
                           }).SingleOrDefault();

                return Ok(data);
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Post([FromBody] ExpenseModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userid = HttpContext.User.Claims.First(i => i.Type == "UserId").Value.ToString();

            using (var context = new SeaExpenContext())
            {
                var i = context.Database.ExecuteSql($"exec usp_Manage_Expense @Action=1, @Category={model.Category}, @Amount={model.Amount}, @Note={model.Note}, @UserId={userid}");

                if(i != 0)
                {
                    return Ok( new { message = "Expense recoreded successful"});
                }

                return BadRequest(new { error= "Expense cannot be recorded"});
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] ExpenseModel model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var userid = HttpContext.User.Claims.First(i => i.Type == "UserId").Value.ToString();

            using (var context = new SeaExpenContext())
            {
                var i = context.Database.ExecuteSql($"exec usp_Manage_Expense @Action=2, @Category={model.Category}, @Amount={model.Amount}, @Note={model.Note}, @UserId={userid}, @Id={model.Id}");
                
                if(i != 0)
                {
                    return Ok(new { message = "Expense updated successful" });
                }

                return BadRequest(new { error = "Expense cannot be updated" });
            }
        }

        [HttpDelete]
         public IActionResult Delete(int id)
        {
            using (var context = new SeaExpenContext())
            {
                try
                {
                    var expense = context.Expenses.FirstOrDefault(i => i.Id == id);

                    if (expense != null)
                    {
                        context.Expenses.Remove(expense);
                        context.SaveChanges();
                    }

                    return Ok(new { message = "Expense deleted successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }
        }
    }
}
