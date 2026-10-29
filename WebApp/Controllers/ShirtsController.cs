using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    public class ShirtsController : Controller // edw klironomoume ton Controller gia na mas dwsei ta views
    {
        private readonly IWebApiExecuter webApiExecuter;

        //dependency inject sto constructor 
        public ShirtsController(IWebApiExecuter webApiExecuter)
        {
            this.webApiExecuter = webApiExecuter;
        }

        public async Task<IActionResult> Index()
        {
            return View(await webApiExecuter.InvokeGet<List<Shirt>>("shirts")); //prepei na dwsoume kai to relative url auto stin periptosi mas einai h lelksi sto url meta to api
            //auta pane sto index page pou ftiaksame emeis prin 



            // pairnoume static data edw: return View(ShirtRepository.GetShirts()); //to mono pou kanoume edw einai na xrisimopoisoume to in memory repository 
            //edw exoume mia lista apo shirts kai blepoume oti epistreftai mia lista
            
        }

        //http post method
        public IActionResult CreateShirt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShirt(Shirt shirt) //epitrepoume stin froma na postarei to shirt edw
        {
           //sto web API ta validations ginonte trigger ek twn proterwn                                 //return View(await webApiExecuter.InvokeGet<List<Shirt>>("shirts"));
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var response = await webApiExecuter.InvokePost("shirts", shirt); //sta autakia einai to endpoint , to shirt pou dinoume sto endpoint 
                        if (response != null)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch(WebApiException ex)
                {
                    HandleWebApiException(ex);
                }
            }
            
            //an to validation den pernaei tote epistrefoume to parakatw view
            return View(shirt); 
        }

        //an den baleis http verb apo default einai get method
        public async Task<IActionResult> UpdateShirt(int shirtId) //shirtId dld poio shirt theloume na kanoume update
        {
            try
            {
                var shirt = await webApiExecuter.InvokeGet<Shirt>($"shirts/{shirtId}");
                if (shirt != null)
                {
                    return View(shirt); //kanw bind to shirt sto view
                }

                
            }
            catch(WebApiException ex)
            {
                HandleWebApiException(ex);
                return View(); //refreshes page so the user will see the error directly an den thelame o xristis na blepei to exception den tha epistrefame view
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateShirt(Shirt shirt) //epitrepoume stin froma na postarei to shirt edw
        {
            //sto web API ta validations ginonte trigger ek twn proterwn                                 //return View(await webApiExecuter.InvokeGet<List<Shirt>>("shirts"));

            if (ModelState.IsValid)
            {
                try
                {
                    // to put den epistrefei tipota opote den to vazoume se metabliti
                    await webApiExecuter.InvokePut($"shirts/{shirt.ShirtId}", shirt); //sta autakia einai to endpoint , to shirt pou dinoume sto endpoint 
                                                                                      //den xreiazomaste mono to object alla kai to id //to 2o orisma tou post paei sto body

                    return RedirectToAction(nameof(Index));
                }
                catch(WebApiException ex)
                {
                    HandleWebApiException(ex);
                }
            }

            //an to validation den pernaei tote epistrefoume to parakatw view
            return View(shirt); //kai mporei o xristis na ksanaeisagei ta stoixeia sthn forma
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShirt([FromForm]int shirtId) //gia na eimaste sigouroi oti erxete apo tin forma bazoume to FromForm
        {
            try
            {
                await webApiExecuter.InvokeDelete($"shirts/{shirtId}");
                return RedirectToAction(nameof(Index));
            }
            catch(WebApiException ex)
            {
                HandleWebApiException(ex);
                return View(nameof(Index), await webApiExecuter.InvokeGet<List<Shirt>>("shirts"));
                //me auton ton tropo den xanoume ta model state kai to error mnm emfanizonte opws epishs kai ta shirts

                //katw exoume mazepsei kapoia errors pou einai ModelState an kanoume RedirectToAction  xanonte ata ta ModelState
            }
        }


        private void HandleWebApiException(WebApiException ex)
        {
            if (ex.ErrorResponse != null &&
                        ex.ErrorResponse.Errors != null &&
                        ex.ErrorResponse.Errors.Count > 0)
            {
                foreach (var error in ex.ErrorResponse.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join("; ", error.Value));
                    //den einai meros tou model validation, 
                }
            }
            else if (ex.ErrorResponse !=null)
            {
                ModelState.AddModelError("Error", ex.ErrorResponse.Title);
            }
            else
            {
                ModelState.AddModelError("Error", ex.Message);
            }
        }

    }
}
