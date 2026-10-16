using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo2.Data;
using WebApiDemo2.Models.Repositories;

namespace WebApiDemo2.Filters.ActionFilters
{ //kai auto gia annotation to kanoume
    public class Shirt_ValidateShirtIdFilterAttribute : ActionFilterAttribute
    {
        private readonly AplicationDbContext db;

        public Shirt_ValidateShirtIdFilterAttribute(AplicationDbContext db)
        {
            this.db = db;
        }



        //otan ginete h klash mas kleironomei apo authn thn sugkekrimeni 
        //mporoume na kanoume kapoia overrides

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirtId = context.ActionArguments["id"] as int?;
            //auto to id erxetai apo to ShirtsController sigkekrimena apo to get id
            if (shirtId.HasValue)
            {
                if (shirtId.Value <=0)
                {
                    //to ShirtId einai i parametros to opoio den einai to antikeimeno
                    context.ModelState.AddModelError("ShirtId", "ShirtId is invalid");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest //ousiastika leme edw oti theloume otan toid einai mikrotero tou mhdenos na bgazei auto to status code
                    };
                    context.Result = new BadRequestObjectResult(problemDetails); //kai etsi to pernas edw
                }
                else   // feugei gia na xrisimopoisoume dependency injection if (!ShirtRepository.ShirtExists(shirtId.Value))
                {
                    var shirt = db.Shirts.Find(shirtId.Value); //dependency injection
                    if (shirt == null)
                    {
                        context.ModelState.AddModelError("ShirtId", "Shirt does not exist"); //mallon prosthetw error me auton ton tropo
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status404NotFound //otan den briskeis kati me to id
                        };
                        context.Result = new NotFoundObjectResult(problemDetails);
                    }
                    else //otan broume kapoio shirt pername to shirt apo to action filter sto controller
                    {
                        context.HttpContext.Items["shirt"] = shirt; //to httpContext prosorina apothikeuei to shirt object ston controller
                    }
                    
                }
            }
        }
    }
}
