using AyudasTecnologicas.DAL.Entities;

namespace AyudasTecnologicas.Models
{
    public class CityViewModel : City
    {
        public Guid StateId { get; set; }
    }
}
