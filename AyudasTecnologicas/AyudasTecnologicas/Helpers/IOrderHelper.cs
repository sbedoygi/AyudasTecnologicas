using AyudasTecnologicas.Common;
using AyudasTecnologicas.Models;


namespace AyudasTecnologicas.Helpers
{
    public interface IOrderHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel showCartViewModel);
    }
}
