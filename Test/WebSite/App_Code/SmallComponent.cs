using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Web.Mvc;
using Radischevo.Wahha.Web.Routing;

/// <summary>
/// Summary description for SmallComponent
/// </summary>
public class SmallComponent : Controller
{
    [ActionCache("message", Duration = 10, VaryByUser = true)]
    public ActionResult WriteMessage(string message)
    {
        return View("Bar", message);
    }
}
