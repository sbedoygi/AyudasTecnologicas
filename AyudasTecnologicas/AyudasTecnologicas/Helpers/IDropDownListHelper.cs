using AyudasTecnologicas.DAL.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AyudasTecnologicas.Helpers
{
    public interface IDropDownListHelper
    {
        Task<IEnumerable<SelectListItem>> GetDDLCategoriesAsync(); //DDL = Drop Down List

        Task<IEnumerable<SelectListItem>> GetDDLCategoriesAsync(IEnumerable<Services> filterCategories); //DDL = Drop Down List

        Task<IEnumerable<SelectListItem>> GetDDLCountriesAsync();

        Task<IEnumerable<SelectListItem>> GetDDLStatesAsync(Guid countryId);

        Task<IEnumerable<SelectListItem>> GetDDLCitiesAsync(Guid stateId);
    }
}
