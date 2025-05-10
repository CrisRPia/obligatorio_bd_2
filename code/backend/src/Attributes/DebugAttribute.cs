using Microsoft.AspNetCore.Mvc.Filters;

namespace backend.src.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
class DebugAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
#if DEBUG
        base.OnActionExecuting(context);
#else
        context.Result = new NotFoundResult();
#endif
    }
}
