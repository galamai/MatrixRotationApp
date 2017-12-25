using CsvHelper;
using MatrixRotationApp.Models;
using MatrixRotationApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MatrixRotationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMatrixService _matrixService;

        public HomeController(IMatrixService matrixService)
        {
            _matrixService = matrixService;
        }

        public async Task<ActionResult> Index(string error = null)
        {
            var matrix = await _matrixService.ReadMatrixAsync();
            return View(new HomeViewModel() { Error = error, Matrix = matrix });
        }

        [HttpPost]
        public async Task<ActionResult> Actions(string action)
        {
            if (action != null)
            {
                switch (action)
                {
                    case "RotateСlockwise":
                        var matrix = await _matrixService.ReadMatrixAsync();
                        _matrixService.RotateMatrixСlockwise(matrix);
                        await _matrixService.WriteMatrixAsync(matrix);
                        break;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> UploadMatrix(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var matrix = await _matrixService.ToMatrixAsync(file.InputStream);
                try
                {
                    await _matrixService.WriteMatrixAsync(matrix);
                }
                catch(ArgumentOutOfRangeException)
                {
                    return RedirectToAction("Index", "Home", new { Error = "Матрица не квадратная" });
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}