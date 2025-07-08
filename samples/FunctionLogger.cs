using Microsoft.SemanticKernel;

public class FunctionLogger : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        var arguments = context.Arguments?.Select(a => $"{a.Key}={a.Value}") ?? Enumerable.Empty<string>();
        Console.WriteLine($"[Invoking] Plugin: {context.Function.PluginName}, Function: {context.Function.Name}({string.Join(',', arguments)})");

        await next(context);
        
        Console.WriteLine($"[Invoked] Plugin: {context.Function.PluginName}, Function: {context.Function.Name}({string.Join(',', arguments)}) with result: {context.Result}");
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var arguments = context.Arguments.Select(a => $"{a.Key}={a.Value}");
        Console.WriteLine($"Plugin: {context.Function.PluginName}, Function: {context.Function.Name}({string.Join(',', arguments)})");
        
        await next(context);
    }
}