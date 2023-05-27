using AyudasTecnologicas.DAL.Entities;
using AyudasTecnologicas.DAL;
using AyudasTecnologicas.Enum;
using AyudasTecnologicas.Helpers;
using AyudasTecnologicas.Models;
using AyudasTecnologicas.Common;

namespace AyudasTecnologicas.servicios
{
    public class OrderHelper : IOrderHelper
    {
        private readonly DataBaseContext _context;

        public OrderHelper(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Response> ProcessOrderAsync(ShowCartViewModel showCartViewModel)
        {
            Response response = await CheckInventoryAsync(showCartViewModel);
            if (!response.IsSuccess) return response;

            Order order = new()
            {
                CreatedDate = DateTime.Now,
                User = showCartViewModel.User,
                Remarks = showCartViewModel.Remarks,
                OrderDetails = new List<OrderDetailservices>(),
                OrderStatus = OrderStatus.Nuevo
            };

            foreach (TemporalService? item in showCartViewModel.TemporalSales)
            {
                order.OrderDetails.Add(new OrderDetailservices
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });

                TechnicalServices product = await _context.Products.FindAsync(item.Product.Id);
                if (product != null)
                {
                    //Aquí actualizo inventario
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);
                }

                _context.TemporalSales.Remove(item);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return response;
        }

        private async Task<Response> CheckInventoryAsync(ShowCartViewModel showCartViewModel)
        {
            Response response = new()
            {
                IsSuccess = true
            };

            foreach (TemporalService item in showCartViewModel.TemporalSales)
            {
                TechnicalServices product = await _context.Products.FindAsync(item.Product.Id);

                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"El producto {item.Product.Name}, ya no está disponible";
                    return response;
                }

                if (product.Stock < item.Quantity)
                {
                    response.IsSuccess = false;
                    response.Message = $"Lo sentimos, solo tenemos {item.Quantity} unidades del producto {item.Product.Name}, para tomar su pedido. Dismuya la cantidad.";
                    return response;
                }
            }

            return response;
        }

    }
}
