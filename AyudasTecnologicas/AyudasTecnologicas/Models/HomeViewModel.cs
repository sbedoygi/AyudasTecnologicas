using AyudasTecnologicas.DAL.Entities;


namespace AyudasTecnologicas.Models
{
    public class HomeViewModel
    {
        public ICollection<TechnicalServices> Products { get; set; }

        //Esta propiedad me muestra cuánto productos llevo agregados al carrito de compras.
        public float Quantity { get; set; }
    }
}
