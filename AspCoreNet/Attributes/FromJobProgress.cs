using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ForgeSharp.Results.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
internal class FromJobProgress : Attribute, IBindingSourceMetadata
{
    public BindingSource? BindingSource { get; } = new BindingSource(
        "JobProgress",
        displayName: "Job Progress",
        isGreedy: false,
        isFromRequest: false
    );
}
