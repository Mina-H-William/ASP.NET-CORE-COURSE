using Microsoft.AspNetCore.Mvc;
using ViewComponentExample.Models;

namespace ViewComponentExample.ViewComponents
{
    //[ViewComponent]
    public class GridViewComponent : ViewComponent
    {
        // ViewComponent takes View data only from ViewComponent Not from parent view
        public async Task<IViewComponentResult> InvokeAsync(PersonGridModel grid)
        {
            //PersonGridModel model = new PersonGridModel()
            //{
            //    GridTitle = "Person List",
            //    Persons = new List<Person> {
            //        new Person { PersonName = "Mina", JobTitle = "Manger" },
            //        new Person { PersonName = "Hany", JobTitle = "Asst.Manger" },
            //        new Person { PersonName = "eslam", JobTitle = "Developer" }
            //    }
            //};
            //ViewData["Grid"] = model;
            return View(grid); // invoke a partial view from Views/Shared/Components/[ViewComponentName ex:Grid]/Default.cshtml
        }
    }
}
