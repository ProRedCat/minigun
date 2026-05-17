using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Minigun.ViewComponents
{
    public class RuntimeInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var runtimeVersion = RuntimeInformation.FrameworkDescription;
            return View("Default", runtimeVersion);
        }
    }
} 