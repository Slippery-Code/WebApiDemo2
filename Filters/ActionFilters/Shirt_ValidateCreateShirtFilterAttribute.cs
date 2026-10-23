using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Drawing;
using System.Reflection;
using System.Xml;
using WebApiDemo2.Data;
using WebApiDemo2.Models;
using WebApiDemo2.Models.Repositories;

namespace WebApiDemo2.Filters.ActionFilters
{
    public class Shirt_ValidateCreateShirtFilterAttribute : ActionFilterAttribute
    {
        private readonly AplicationDbContext db;

        //prwta dimiourgoume constructor kai dependency injection sto constructor
        public Shirt_ValidateCreateShirtFilterAttribute(AplicationDbContext db)
        {
            this.db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirt = context.ActionArguments["shirt"] as Shirt; //edw to shirt anaferomaste sto CreateShirt tou controller
            if (shirt==null)
            {
                context.ModelState.AddModelError("shirt", "shirt object is null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
            else
            {
                //to OrdinalIgnoreCase den mporei na metatrapei se sql statement sto entity framework core
                var existingShirt = db.Shirts.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(shirt.Brand)
                && !string.IsNullOrWhiteSpace(x.Brand) &&
                x.Brand.ToLower() == shirt.Brand.ToLower() 
                && !string.IsNullOrWhiteSpace(shirt.Gender) &&
                !string.IsNullOrWhiteSpace(x.Gender) &&
                x.Gender.ToLower() == shirt.Gender.ToLower() &&
                !string.IsNullOrWhiteSpace(shirt.Color) &&
                !string.IsNullOrWhiteSpace(x.Color) &&
                x.Color.ToLower() == shirt.Color.ToLower() &&
                shirt.Size.HasValue &&
                x.Size.HasValue &&
                shirt.Size.Value == x.Size.Value);

                //sto postman gia to [FromBody] prepei na epileksw raw kai json
                //gia to [FromForm] prepei na epileksw form data
                //var existingShirt = ShirtRepository.GetShirtByProperties(shirt.Brand, shirt.Gender, shirt.Color, shirt.Size); xoris in entity framework core
                if (existingShirt != null)
                {
                    context.ModelState.AddModelError("shirt", "shirt already exists null");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }
            }
            
           


            
        }
    }
}
