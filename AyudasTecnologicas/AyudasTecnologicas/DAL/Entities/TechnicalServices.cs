using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AyudasTecnologicas.DAL.Entities
{
   
        public class TechnicalServices : Entity
        {
            public ICollection<OrderDetailservices> OrderDetails { get; set; }

            [Display(Name = "Nombre")]
            [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            public string Name { get; set; }

            [DataType(DataType.MultilineText)]
            [Display(Name = "Descripción")]
            [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
            public string? Description { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            [DisplayFormat(DataFormatString = "{0:C2}")]
            [Display(Name = "Precio")]
            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            public decimal Price { get; set; }

            [DisplayFormat(DataFormatString = "{0:N2}")]
            [Display(Name = "Inventario")]
            [Required(ErrorMessage = "El campo {0} es obligatorio.")]
            public float Stock { get; set; }

            public ICollection<ServicesCategory> ServicesCategories { get; set; }

            [Display(Name = "Categorías")]
            public int CategoriesNumber => ServicesCategories == null ? 0 : ServicesCategories.Count;

            public ICollection<ServicesImage> ProductImages { get; set; }

            [Display(Name = "Número Fotos")]
            public int ImagesNumber => ProductImages == null ? 0 : ProductImages.Count;

            [Display(Name = "Foto")]
            public string ImageFullPath => ProductImages == null || ProductImages.Count == 0
                ? $"https://localhost:7158/images/NoImage.png"
                : ProductImages.FirstOrDefault().ImageFullPath;
        }
    }