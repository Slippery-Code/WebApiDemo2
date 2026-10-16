using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo2.Models;

namespace WebApiDemo2.Filters.ActionFilters
{
    public class Shirt_ValidateUpdateShirtFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var id = context.ActionArguments["id"] as int?;
            var shirt = context.ActionArguments["shirt"] as Shirt;
            
            if (id.HasValue && shirt !=null && id !=shirt.ShirtId)
            {
                context.ModelState.AddModelError("ShirtId", "Shirt is not the same as id "); //mallon prosthetw error me auton ton tropo
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest //otan den briskeis kati me to id
                };
                context.Result = new NotFoundObjectResult(problemDetails);
            }
        }
        
        }
}

