using AyudasTecnologicas.DAL.Entities;
using AyudasTecnologicas.Enum;
using AyudasTecnologicas.Helpers;
using AyudasTecnologicas.services;
using Microsoft.EntityFrameworkCore;

namespace AyudasTecnologicas.DAL
{
    public class SeederDb
    {
        private readonly DataBaseContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IAzureBlobHelper _azureBlobHelper;

        public SeederDb(DataBaseContext context, IUserHelper userHelper, IAzureBlobHelper azureBlobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _azureBlobHelper = azureBlobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await PopulateCategoriesAsync();
            await PopulateCountriesStatesCitiesAsync();
            await PopulateRolesAsync();
            await PopulateUserAsync("Steve", "Jobs", "sbedoygi@gmail.com", "3137846127", "Street Apple", "102030", "SteveJobs.png", UserType.Admin);
            await PopulateUserAsync("Bill", "Gates", "bill_gates_user@yopmail.com", "4005656656", "Street Microsoft", "405060", "BillGates.png", UserType.User);
            await PopulateProductAsync();

            await _context.SaveChangesAsync();
        }

        private async Task PopulateCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Services { Name = "Repuestos", Description = "Hardware", CreatedDate = DateTime.Now });
                _context.Categories.Add(new Services { Name = "ELetricidad", Description = "Diseño E instalacion de los circuitos eletricos.", CreatedDate = DateTime.Now });
                _context.Categories.Add(new Services { Name = "Soporte Tecnico", Description = "Mantienimiento correntivo", CreatedDate = DateTime.Now });
                _context.Categories.Add(new Services { Name = "Conectividad", Description = "Redes inalabrincas Automatizacion", CreatedDate = DateTime.Now });
                _context.Categories.Add(new Services { Name = "Manposteria", Description = "Mejoras en casa Drywall Enchape y acabados.", CreatedDate = DateTime.Now });
            }
        }

        private async Task PopulateCountriesStatesCitiesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(
                new Country
                {
                    Name = "Colombia",
                    CreatedDate = DateTime.Now,
                    States = new List<State>()
                    {
                        new State
                        {
                            Name = "Antioquia",
                            CreatedDate = DateTime.Now,
                            Cities = new List<City>()
                            {
                                new City { Name = "Medellín", CreatedDate= DateTime.Now },
                                new City { Name = "Bello", CreatedDate= DateTime.Now },
                                new City { Name = "Itagüí", CreatedDate= DateTime.Now },
                                new City { Name = "Sabaneta", CreatedDate= DateTime.Now },
                                new City { Name = "Envigado", CreatedDate= DateTime.Now },
                            }
                        },

                        new State
                        {
                            Name = "Cundinamarca",
                            CreatedDate = DateTime.Now,
                            Cities = new List<City>()
                            {
                                new City { Name = "Bogotá", CreatedDate= DateTime.Now },
                                new City { Name = "Fusagasugá", CreatedDate= DateTime.Now },
                                new City { Name = "Funza", CreatedDate= DateTime.Now },
                                new City { Name = "Sopó", CreatedDate= DateTime.Now },
                                new City { Name = "Chía", CreatedDate= DateTime.Now },
                            }
                        },

                        new State
                        {
                            Name = "Atlántico",
                            CreatedDate = DateTime.Now,
                            Cities = new List<City>()
                            {
                                new City { Name = "Barranquilla", CreatedDate= DateTime.Now },
                                new City { Name = "La Chinita", CreatedDate= DateTime.Now },
                            }
                        },
                    }
                });

                _context.Countries.Add(
                new Country
                {
                    Name = "Argentina",
                    CreatedDate = DateTime.Now,
                    States = new List<State>()
                    {
                        new State
                        {
                            Name = "Buenos Aires",
                            CreatedDate = DateTime.Now,
                            Cities = new List<City>()
                            {
                                new City { Name = "Avellaneda", CreatedDate= DateTime.Now },
                                new City { Name = "Ezeiza", CreatedDate= DateTime.Now },
                                new City { Name = "La Boca", CreatedDate= DateTime.Now },
                                new City { Name = "Río de la Plata", CreatedDate= DateTime.Now },
                            }
                        },

                        new State
                        {
                            Name = "La Pampa",
                            CreatedDate = DateTime.Now,
                            Cities = new List<City>()
                            {
                                new City { Name = "Santa María", CreatedDate= DateTime.Now },
                                new City { Name = "Obrero", CreatedDate= DateTime.Now },
                                new City { Name = "Rosario", CreatedDate= DateTime.Now }
                            }
                        }
                    }
                });
            }
        }


        private async Task PopulateRolesAsync()
        {
            await _userHelper.AddRoleAsync(UserType.Admin.ToString());
            await _userHelper.AddRoleAsync(UserType.User.ToString());
        }

        private async Task PopulateUserAsync(string firstName, string lastName, string email, string phone, string address, string document, string image, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _azureBlobHelper.UploadAzureBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");

                user = new User
                {
                    CreatedDate = DateTime.Now,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageId = imageId
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }
        }


        private async Task PopulateProductAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Discos mecánicos", 270000M, 12F, new List<string>() { " nuevos y usados ", "escritorio y laptops." }, new List<string>() { "Medias1.png" });
                await AddProductAsync("Memoria Ram", 300000M, 12F, new List<string>() { "capacidad total: 8gb", "ddr4 2133mhz" }, new List<string>() { "Medias2.png" });
                await AddProductAsync("Cable USB tipo C ", 5000000M, 12F, new List<string>() { "Tecnología", "USB " }, new List<string>() { "TvOled.png", "TvOled2.png" });
                await AddProductAsync("Disco estado solido", 5000000M, 12F, new List<string>() { " Kingston M.2 PCIE 500Gb" }, new List<string>() { "PS5.png", "PS52.png" });
                 
            }
        }

        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            TechnicalServices product = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ServicesCategories = new List<ServicesCategory>(),
                ServicesImages = new List<ServicesImage>()
            };



            foreach (string? category in categories)
            {
                product.ServicesCategories.Add(new ServicesCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                Guid imageId = await _azureBlobHelper.UploadAzureBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
                product.ServicesImages.Add(new ServicesImage { ImageId = imageId });
            }

            _context.Products.Add(product);
        }
    }
}
