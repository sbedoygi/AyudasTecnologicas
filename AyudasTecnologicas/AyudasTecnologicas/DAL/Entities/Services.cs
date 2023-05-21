using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AyudasTecnologicas.DAL.Entities
{
    public class Services : Entity
    {
        [Display(Name = "Services")] //Nombre que quiero mostrar en la web
        [MaxLength(100)] //varchar(50)
        [Required(ErrorMessage = "El campo {0} es obligatorio")] //Not Null
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        public ICollection<ServicesCategory> servicesCategories { get; set; }
    }
}

