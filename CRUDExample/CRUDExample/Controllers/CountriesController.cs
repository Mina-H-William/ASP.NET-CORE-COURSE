﻿using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Threading.Tasks;

namespace CRUDExample.Controllers
{
    [Route("/[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file.";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported File, 'xlsx' file is expected";
                return View();
            }

            int response = await _countriesService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{response} Countries Uploaded";
            return View();
        }
    }
}
