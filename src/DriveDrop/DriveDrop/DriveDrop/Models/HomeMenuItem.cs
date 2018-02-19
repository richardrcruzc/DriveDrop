using System;
using System.Collections.Generic;
using System.Text;

namespace DriveDrop.Core.Models
{
    public enum MenuType
    {
        Info,
        Address,
        Package,
        Password,
        About,

    }
    public class HomeMenuItem : BaseModel
    {
        public HomeMenuItem()
        {
            MenuType = MenuType.About;
        }
        public string Icon { get; set; }
        public MenuType MenuType { get; set; }
    }
}
