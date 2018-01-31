using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewComponents
{
    public class PerMilesViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<RateDetailModel> model )
        {
            await Task.Delay(10);
            model.Add(new RateDetailModel());

            return View(model);
        }
        
    }
}
