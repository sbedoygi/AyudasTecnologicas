using AyudasTecnologicas.Controllers;
using AyudasTecnologicas.DAL;
using AyudasTecnologicas.DAL.Entities;
using AyudasTecnologicas.Helpers;
using AyudasTecnologicas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Data;


namespace AyudasTecnologicas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TechnicalServicesController : Controller
    {
        private readonly DataBaseContext _context;
        private readonly IAzureBlobHelper _azureBlobHelper;
        private readonly IDropDownListHelper _dropDownListHelper;

        public TechnicalServicesController(DataBaseContext context, IAzureBlobHelper azureBlobHelper, IDropDownListHelper dropDownListHelper)
        {
            _context = context;
            _azureBlobHelper = azureBlobHelper;
            _dropDownListHelper = dropDownListHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(p => p.ServicesImages)
                .Include(p => p.ServicesCategories)
                .ThenInclude(pc => pc.Category)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AddProductViewModel addProductViewModel = new()
            {
                Categories = await _dropDownListHelper.GetDDLCategoriesAsync(),
            };

            return View(addProductViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel addProductViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;
                    if (addProductViewModel.ImageFile != null)
                        imageId = await _azureBlobHelper.UploadAzureBlobAsync(addProductViewModel.ImageFile, "products");

                    TechnicalServices product = new()
                    {
                        Description = addProductViewModel.Description,
                        Name = addProductViewModel.Name,
                        Price = addProductViewModel.Price,
                        Stock = addProductViewModel.Stock,
                        CreatedDate = DateTime.Now,
                    };

                    product.ServicesCategories = new List<ServicesCategory>()
                    {
                        new ServicesCategory
                        {
                            Category = await _context.Categories.FindAsync(addProductViewModel.CategoryId)
                        }
                    };

                    if (imageId != Guid.Empty)
                    {
                        product.ServicesImages = new List<ServicesImage>()
                        {
                            new ServicesImage {
                                ImageId = imageId,
                                CreatedDate = DateTime.Now,
                            }
                        };
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un producto con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            addProductViewModel.Categories = await _dropDownListHelper.GetDDLCategoriesAsync();
            return View(addProductViewModel);
        }

        public async Task<IActionResult> Edit(Guid? productId)
        {
            if (productId == null) return NotFound();

            TechnicalServices product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            EditTechnicalServicesViewModel editProductViewModel = new()
            {
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
            };

            return View(editProductViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? Id, EditTechnicalServicesViewModel editProductViewModel)
        {
            if (Id != editProductViewModel.Id) return NotFound();

            try
            {
                TechnicalServices product = await _context.Products.FindAsync(editProductViewModel.Id);

                //Aquí sobreescribo para luego guardar los cambios en BD
                product.Description = editProductViewModel.Description;
                product.Name = editProductViewModel.Name;
                product.Price = editProductViewModel.Price;
                product.Stock = editProductViewModel.Stock;
                product.ModifiedDate = DateTime.Now;

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    ModelState.AddModelError(string.Empty, "Ya existe un producto con el mismo nombre.");
                else
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

            return View(editProductViewModel);
        }

        public async Task<IActionResult> Details(Guid? productId)
        {
            if (productId == null) return NotFound();

            TechnicalServices product = await _context.Products
                .Include(p => p.ServicesImages) // Inner Join entre Product - ProductImages
                .Include(p => p.ServicesCategories) // Inner Join entre Product - ProductCategories
                .ThenInclude(pc => pc.Category) // Inner Join entre ProductCategories - Categories
                .FirstOrDefaultAsync(p => p.Id == productId);



            if (product == null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> AddImage(Guid? productId)
        {
            if (productId == null) return NotFound();

          TechnicalServices product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            AddProductImageViewModel addProductImageViewModel = new()
            {
                ProductId = product.Id,
            };

            return View(addProductImageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel addProductImageViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = await _azureBlobHelper.UploadAzureBlobAsync(addProductImageViewModel.ImageFile, "products");

                    TechnicalServices product = await _context.Products.FindAsync(addProductImageViewModel.ProductId);

                    ServicesImage ServicesImage = new()
                    {
                        Product = product,
                        ImageId = imageId,
                        CreatedDate = DateTime.Now,
                    };

                    _context.Add(ServicesImage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { productId = product.Id });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(addProductImageViewModel);
        }

        public async Task<IActionResult> DeleteImage(Guid? imageId)
        {
            if (imageId == null) return NotFound();

            ServicesImage ServicesImage = await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == imageId);

            if (ServicesImage == null) return NotFound();

            await _azureBlobHelper.DeleteAzureBlobAsync(ServicesImage.ImageId, "products");

            _context.ProductImages.Remove(ServicesImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { productId = ServicesImage.Product.Id });
        }

        public async Task<IActionResult> AddCategory(Guid? productId)
        {
            if (productId == null) return NotFound();

            TechnicalServices product = await _context.Products
                .Include(p => p.ServicesCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            List<Services> categories = product.ServicesCategories.Select(pc => new Services
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name, //Aquí coloco las N categoríes que le agregué a ese prod: GAMERS, TECHOLOGY
            }).ToList();

            AddTechnicalServicesViewModel addProductCategoryViewModel = new()
            {
                ProductId = product.Id,
                Categories = await _dropDownListHelper.GetDDLCategoriesAsync(categories),
            };

            return View(addProductCategoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(AddTechnicalServicesViewModel AddTechnicalServicesViewModel)
        {
            TechnicalServices product = await _context.Products
                .Include(p => p.ServicesCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == AddTechnicalServicesViewModel.ProductId);

            if (ModelState.IsValid)
            {
                try
                {
                    Services category = await _context.Categories.FindAsync(AddTechnicalServicesViewModel.CategoryId);

                    if (product == null || category == null) return NotFound();

                  ServicesCategory productCategory = new()
                    {
                        Product = product,
                        Category = category
                    };

                    _context.Add(productCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { productId = product.Id });
                }
                catch (Exception exception)
                {
                    AddTechnicalServicesViewModel.Categories = await _dropDownListHelper.GetDDLCategoriesAsync();
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            List<Services> categories = product.ServicesCategories.Select(pc => new Services
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name, //Aquí coloco las N categoríes que le agregué a ese prod: GAMERS, TECHOLOGY
            }).ToList();

            AddTechnicalServicesViewModel.Categories = await _dropDownListHelper.GetDDLCategoriesAsync(categories);
            return View(AddTechnicalServicesViewModel);
        }

        public async Task<IActionResult> DeleteCategory(Guid? categoryId)
        {
            if (categoryId == null) return NotFound();

            ServicesCategory productCategory = await _context.ProductCategories
                .Include(pc => pc.Product)
                .FirstOrDefaultAsync(pc => pc.Id == categoryId);
            if (productCategory == null) return NotFound();

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { productId = productCategory.Product.Id });
        }

        public async Task<IActionResult> Delete(Guid? productId)
        {
            if (productId == null) return NotFound();

            TechnicalServices product = await _context.Products
                .Include(p => p.ServicesCategories)
                .Include(p => p.ServicesImages)
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TechnicalServices productModel)
        {
            TechnicalServices product = await _context.Products
                .Include(p => p.ServicesImages)
                .Include(p => p.ServicesCategories)
                .FirstOrDefaultAsync(p => p.Id == productModel.Id);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            foreach (ServicesImage productImage in product.ServicesImages)
                await _azureBlobHelper.DeleteAzureBlobAsync(productImage.ImageId, "products");

            return RedirectToAction(nameof(Index));
        }
    }
}
