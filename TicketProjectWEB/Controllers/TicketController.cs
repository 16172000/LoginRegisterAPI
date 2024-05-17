using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net;
using System.Reflection;
using System.Text;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    [SessionAuthorize]
    public class TicketController : Controller
    {

        private readonly HttpClient _client;

        public TicketController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:44382/");
        }

        public IActionResult Home()
        {

            return View();
        }

        public async Task<IActionResult> Index()
        {
            List<Register> entryList = new List<Register>();

            try
            {
                var response = await _client.GetAsync("api/Project/getRegister");

                if (response.IsSuccessStatusCode)
                {
                    entryList = await response.Content.ReadAsAsync<List<Register>>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
            }

            return View(entryList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Register register)
        {
            try
            {
                string data = JsonConvert.SerializeObject(register);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("api/Project/register", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Data Added...";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(register);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            try
            {
                Register register = new Register();
                HttpResponseMessage response = _client.GetAsync($"api/Project/getRegister/{Id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    register = JsonConvert.DeserializeObject<Register>(data);
                    return View(register);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {response.ReasonPhrase}";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, Register register)
        {
            try
            {
                string data = JsonConvert.SerializeObject(register);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync($"api/Project/register/{Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Data Updated Successfully...";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(register);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"api/Project/getRegister/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    Register register = JsonConvert.DeserializeObject<Register>(data);
                    return View(register);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {response.ReasonPhrase}";

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        TempData["InternalErrorDetails"] = $"Internal Server Error Details: {errorContent}";
                    }

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmAsync(int id)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync($"api/Project/delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Data Deleted Successfully...";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {response.ReasonPhrase}";

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        TempData["InternalErrorDetails"] = $"Internal Server Error Details: {errorContent}";
                    }

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }





    }
}






























//private readonly MyService _myService;

//public TicketController(MyService myService)
//{
//    _myService = myService;
//}

//public async Task<IActionResult> Index()
//{
//    var data = await _myService.GetDataFromApi();
//    // Use the data in your view or return it as needed
//    return View(data);
//}
