using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Custom Action Filter to validate model state.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid == false)
        {
            // Get the first argument (the view model)
            var model = context.ActionArguments.Values.FirstOrDefault();
            // Get the action name (e.g., "Create" or "Update")
            var actionName = context.ActionDescriptor.RouteValues["action"];
            // Return the same view with the invalid model
            context.Result = new ViewResult
            {
                ViewName = actionName,
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                    context.HttpContext.RequestServices.GetService<IModelMetadataProvider>(), // Fix: Provide IModelMetadataProvider
                    context.ModelState)
                {
                    Model = model
                }
            };
        }
    }
}
