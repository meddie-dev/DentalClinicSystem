using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace CMS.Controllers
{
    public class AddOnsController : Controller
    {

        private readonly HttpClient _httpClient;

        public AddOnsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Calendar()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AiTool()
        {
            return View();
        }

       

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile prescriptionImage)
        {
            if (prescriptionImage == null || prescriptionImage.Length == 0)
                return Content("Please select a file");

            using var content = new MultipartFormDataContent();
            using var stream = prescriptionImage.OpenReadStream();
            content.Add(new StreamContent(stream), "image", prescriptionImage.FileName);

            var response = await _httpClient.PostAsync("http://localhost:5000/process_prescription", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Content($"Error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            ViewBag.SimplifiedText = data.simplified_text.ToString();

            return View("Result");
        }

        
    }
}
