using AyudasTecnologicas.Models;
using Azure;

namespace AyudasTecnologicas.Helpers
{
    public interface IOrderHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel showCartViewModel);
    }
}
