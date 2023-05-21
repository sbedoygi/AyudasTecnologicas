using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AyudasTecnologicas.DAL.Entities
{
    public class ServicesImage : Entity
    {
        public TechnicalServices Product { get; set; }

        [Display(Name = "Foto")]
        public Guid ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://localhost:7158/images/NoImage.png"
            : $"https://sales2023.blob.core.windows.net/products/{ImageId}";
    }
}
