using Microsoft.AspNetCore.Mvc;
using WebApiDemo2.Data;
using WebApiDemo2.Filters.ActionFilters;
using WebApiDemo2.Filters.ExceptionFilters;
using WebApiDemo2.Models;
using WebApiDemo2.Models.Repositories;

namespace WebApiDemo2.Controllers
{
    //gia na einai enas webApi controller prepei na exei to attribute [ApiControllers] kai na klironomei apo to ControllerBase
    [ApiController]
    [Route("api/[controller]")] //anti na les se kathe ena katw pou routareis to kaneis me auton ton tropo edw 
   //auto episeis allazei kai pws xtypas to api prepei na to xtyphseis localhost/api/shirts
    public class ShirtsController : ControllerBase //kleironomoume apo to controllerBase giati edw den exoume views
    {
       


        private readonly AplicationDbContext db;

        //xrisimopoioume constructor gia dependency injection
        public ShirtsController(AplicationDbContext db) 
        {
            this.db = db; //otan h asp.net tha ekkinisei ton controloer tha koitakseiton constructor
            //kai kserei oti apaitei ena instance DB context  meta paei sthn lista me ta services kai tokai tote blepei ti exei mesa to applicationDbContext
            //kai tote einai etoimo na dimiourghsei to instance tou application DB context kai na to feedarei ston constructor
        }


        //[Route("/shirts")] settaroume to url
        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList()); //otan xrisimopoioume to ToList anagkazoume to entity framework core 
            //amesos query the databasekai na metatrepsei to apotelesma sse lista apo shirt
        }
        
        //ta data mporoun na ginonte mapp apo to route stin parametro tis methodou
        /*mporeis na tropopoihseis apo pou theleis na erxonte  ta data dld mporeis na peis [FromRoute] string color  auto shmainei oti erxonte apo route mporeis na to kaneis kai me exception an den sou dwsoun to color
        mporei episis na erthei apo query string  [FromQuery] an to kaneis etsi kai an theleis na ta lambaneis apo header [FromHeader] to header einai key value pair opote sto header prepei na dieukrinisw to "xrwma" tou kleidiou dld [FromHeader(Name="color")]
        // [Route("/shirts/{id}")] apo panw bazeis etsi to id an exeis balei panw route controller */
        [HttpGet("{id}")]
        //[Shirt_ValidateShirtIdFilter] epeidi balame dependency injection ston constructor den mporoume na xrisimopoisoume to filtro
        //to kanoume omws me ton akoloutho tropo kai prepei na antikatastisoume opou kanoume to Shirt_ValidateShirtIdFilter me to TypeFilter
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult GetShirtById(int id)  //to IActionResult san return type to xrisimopoieis otan theleis na epistrepseis kapoio status code ok not found klp
        {
            /*if (id <= 0)
                return BadRequest();

            var shirt = ShirtRepository.GetShirtById(id); //etsi pairnoume ta data apo to ShirtRepository
            if (shirt ==null)
            {
                return NotFound();
            }*/
            // var shirt = db.Shirts.Find(id); an to kanoume auto shmainei oti epikoinonoume me tin basi 2 fores

            

            return Ok(HttpContext.Items["shirt"]);
                //ShirtRepository.GetShirtById(id));
            
        }
        [HttpPost] //otan dinoume dedomena apo body sinithos ta xrisimopoioume sto post h sto put
        [TypeFilter(typeof(Shirt_ValidateCreateShirtFilterAttribute))] //to onomazei single responsibility pronciple
        public IActionResult CreateShirt([FromBody]Shirt shirt) //edw leme oti theloume na einai ena model Shirt kai entikeimeno shirt kai o xristi prepei na balei sigoura tous xaraktires pou den einai nullable kai an thelei kai tous nullable 
        {
            //to db einai hdh dependency injected mesw tou constructor sto controller
            this.db.Shirts.Add(shirt);
            this.db.SaveChanges();

            ShirtRepository.AddShirt(shirt);

            return CreatedAtAction(nameof(GetShirtById),
             new
             {
                 id = shirt.ShirtId //we can use dynamic object for the route parameter
             }, shirt); //otan xrisimopoioume ena create endpoint prepei na epistrefoume created status code me to location header(id=ShirtId)
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        public IActionResult UpdateShirt(int id,Shirt shirt)
        {

            // antikatastasi me in framework core ShirtRepository.UpdateShirt(shirt); //to try catch ginetai sto filtro
            //edw kanoume try catch as poyme giati mporei na diagraftei to shirt prin prolaboume na to enimerosoume

            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt; //twra prepei na xrisimopoisoume tin shirt parametros gia na kanoume update thn shirt metabliti auth pame sto shirtRepository
            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Price = shirt.Price;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.Color = shirt.Color;
            shirtToUpdate.Gender = shirt.Gender;

            db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult DeleteShirt(int id)
        {
            /*var shirt = ShirtRepository.GetShirtById(id); //to kanoume gia na kanoume display to id pou diagrafoume
            ShirtRepository.DeleteShirt(id); */
            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

            db.Shirts.Remove(shirtToDelete);
            db.SaveChanges();
            // otan trexei to save changes blepei poio shirt tha diagraftei kai to markarei deleted tote tha paragei ena delete sql query ston sql server
            return Ok(shirtToDelete);
        }
    }
}
