using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo2.Data;
using WebApiDemo2.Models.Repositories;

namespace WebApiDemo2.Filters.ExceptionFilters
{
    public class Shirt_HandleUpdateExceptionsFilterAttribute : ExceptionFilterAttribute
    {
        private readonly AplicationDbContext db;

        public Shirt_HandleUpdateExceptionsFilterAttribute(AplicationDbContext db)
        {
            this.db = db;
        }

        public override void OnException(ExceptionContext context)
        {
            //epeidi edw den mporoume na xrisimopoisoume to action argument 
            var strShirtId = context.RouteData.Values["id"] as string;
            //auto epistrefei ena antikeimeno kai einai ena string
            if (int.TryParse(strShirtId, out int shirtId))
            {
                //to parakatw shmainei oti den yparxei pleon
                if (db.Shirts.FirstOrDefault(x => x.ShirtId == shirtId) == null) //retrieve data apeuthias apo ti basi
                {
                    context.ModelState.AddModelError("ShirtId", "shirt doesn't exist anymore");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                }
                    
            }
        
        }

    }
}
