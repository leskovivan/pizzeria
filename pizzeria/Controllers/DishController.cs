using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzeria.Interfaces;
using pizzeria.Models;
using pizzeria.ViewModels;

namespace pizzeria.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class DishController : Controller
    {
        private readonly IProduct _products;
        private readonly ICategory _categories;
        private readonly IWebHostEnvironment _appEnvironment;

        public DishController(IProduct products, ICategory categories, IWebHostEnvironment appEnvironment)
        {
            _products = products;
            _appEnvironment = appEnvironment;
            _categories = categories;
        }

        [Route("/panel/dishes")]
        [HttpGet]
        public IActionResult Dishes(QueryOptions options)
            => View(_products.GetAllProducts(options));

        [Route("/panel/delete-product")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var currentProduct = await _products.GetProductAsync(productId);
            if (currentProduct != null)
            {
                if (currentProduct.Image != null)
                {
                    var physical = _appEnvironment.WebRootPath + currentProduct.Image;
                    if (System.IO.File.Exists(physical))
                        System.IO.File.Delete(physical);
                }
                await _products.DeleteProductAsync(currentProduct);
            }
            return Ok();
        }

        [Route("/panel/create-product")]
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _categories.GetAllCategoriesAsync();
            return View(new ProductViewModel
            {
                AllCategories = new SelectList(categories, "Id", "Name")
            });
        }

        [Route("/panel/create-product")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var currentProduct = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Type = vm.Type,
                Brand = vm.Brand,
                Calories = vm.Calories,
                Price = vm.Price,
                Weight = vm.Weight,
                CategoryId = vm.CategoryId,
                DateOfPublication = System.DateTime.Now
            };

            if (vm.File != null)
            {
                var fileName = Path.GetFileName(vm.File.FileName);
                var filePath = "/productFiles/" + System.Guid.NewGuid() + fileName;
                currentProduct.Image = filePath;
                using var fs = new FileStream(_appEnvironment.WebRootPath + filePath, FileMode.Create);
                await vm.File.CopyToAsync(fs);
            }

            await _products.AddProductAsync(currentProduct);
            return RedirectToAction(nameof(Dishes));
        }

        [Route("/panel/edit-product")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var p = await _products.GetProductAsync(productId);
            if (p == null) return NotFound();

            var categories = await _categories.GetAllCategoriesAsync();
            var vm = new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image,
                Description = p.Description,
                Brand = p.Brand,
                Calories = p.Calories,
                Price = p.Price,
                Type = p.Type,
                Weight = p.Weight,
                CategoryId = p.CategoryId,
                AllCategories = new SelectList(categories, "Id", "Name")
            };
            return View(vm);
        }

        [Route("/panel/edit-product")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var selected = await _products.GetProductAsync(vm.Id ?? 0);
            if (selected == null) return NotFound();

            var currentProduct = new Product
            {
                Id = vm.Id ?? 0,
                Name = vm.Name,
                Description = vm.Description,
                Image = selected.Image,
                Brand = vm.Brand,
                Calories = vm.Calories,
                Price = vm.Price,
                Type = vm.Type,
                Weight = vm.Weight,
                CategoryId = vm.CategoryId
            };

            if (vm.File != null)
            {
                if (selected.Image != null)
                {
                    var physical = _appEnvironment.WebRootPath + selected.Image;
                    if (System.IO.File.Exists(physical))
                        System.IO.File.Delete(physical);
                }

                var fileName = Path.GetFileName(vm.File.FileName);
                var filePath = "/productFiles/" + System.Guid.NewGuid() + fileName;
                currentProduct.Image = filePath;
                using var fs = new FileStream(_appEnvironment.WebRootPath + filePath, FileMode.Create);
                await vm.File.CopyToAsync(fs);
            }

            await _products.EditProductAsync(currentProduct);
            return RedirectToAction(nameof(Dishes));
        }
    }
}
