using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AyudasTecnologicas.DAL.Entities
{
  
    public class OrderDetailservices : Entity
    {
        public Order Order { get; set; }

        public TemporalService? TemporalService { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string? Remarks { get; set; }

        public TechnicalServices Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Valor")]
        public decimal Value => Product == null ? 0 : (decimal)Quantity * Product.Price;
    }
}
