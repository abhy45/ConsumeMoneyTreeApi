using ConsumeMoneyTreeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

namespace ConsumeMoneyTreeApi.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        string Baseurl = "https://localhost:44332/";

        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44332/") // Set the base address if needed
        };

        public async Task<ActionResult> Index()
        {
            List<Employeemodel> EmpInfo = new List<Employeemodel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Employee");
                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    EmpInfo = JsonConvert.DeserializeObject<List<Employeemodel>>(EmpResponse);
                }
                return View(EmpInfo);
            }
        }

        // GET: Employee/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Employeemodel employee = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44332/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/Employee/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employeemodel>(jsonResponse);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(employee);
        }


        // GET: Employee/Create
        public ActionResult Create()
        {
            var countries = GetCountriesStat();

            var model = new Employeemodel
            {
                Countries = ConvertCountriesToSelectListItems(countries),
                States = new List<SelectListItem>(), // Initialize empty list
                Cities = new List<SelectListItem>()   // Initialize empty list
            };
         
            return View(model);
        }

        // POST: Employee/Create

        [HttpPost]
        public async Task<ActionResult> Create(Employeemodel model)
        {
            if (ModelState.IsValid)
            {
              

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PostAsync("api/Employee", content);

                    if (Res.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            var countries = GetCountriesStat();
           

            var models = new Employeemodel
            {
                Countries = ConvertCountriesToSelectListItems(countries),
                States = new List<SelectListItem>(), 
                Cities = new List<SelectListItem>()  
            };

            return View(models);
        }

        // GET: Employee/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Employeemodel emp = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync($"api/Employee/{id}");
                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = await Res.Content.ReadAsStringAsync();
                    emp = JsonConvert.DeserializeObject<Employeemodel>(EmpResponse);

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(emp);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Employeemodel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage Res = await client.PutAsync($"api/Employee/{model.iEmpId}", content);

                    if (Res.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }

            return View(model);
        }

      
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44332/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"api/Employee/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetCountries()
        {
            var countries = GetCountriesStat();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStates(int countryId)
        {

            var states = Getstate(countryId).Select(s => new SelectListItem
            {
                Value = s.StateID.ToString(),
                Text = s.StateName
            }).ToList();


            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCities(int stateId)
        {
            var cities = GetCity(stateId).Select(s => new SelectListItem
            {
                Value = s.CityID.ToString(),
                Text = s.CityName
            }).ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> ConvertCountriesToSelectListItems(IEnumerable<Country> countries)
        {
            return countries.Select(c => new SelectListItem
            {
                Value = c.CountryID.ToString(),
                Text = c.CountryName         
            });
        }

        private IEnumerable<SelectListItem> ConvertCityToSelectListItems(IEnumerable<City> cities)
        {
            return cities.Select(c => new SelectListItem
            {
                Value = c.CityID.ToString(),
                Text = c.CityName        
            });
        }

        private IEnumerable<SelectListItem> ConvertStateToSelectListItems(IEnumerable<State> states)
        {
            return states.Select(c => new SelectListItem
            {
                Value = c.StateID.ToString(), 
                Text = c.StateName      
            });
        }
        public IEnumerable<State> Getstate(int countryId)
        {
            var url = $"https://localhost:44332/api/Dropdown/states/{countryId}";
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode(); 
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<State>>(content);
        }

        public IEnumerable<City> GetCity(int stateId)
        {
            var url = $"https://localhost:44332/api/Dropdown/cities/{stateId}";
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode(); 
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<City>>(content);
        }

        public IEnumerable<Country> GetCountriesStat()
        {
            var response = _httpClient.GetAsync("api/Dropdown").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode(); // Throws an exception if the response code is not successful
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Country>>(content);
        }

        
      
     



       


    }
}

