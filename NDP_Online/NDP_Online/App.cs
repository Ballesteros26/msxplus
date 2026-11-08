using System;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

namespace NDP_Online
{
	// Token: 0x02000007 RID: 7
	public class App : ComponentBase
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00002740 File Offset: 0x00000940
		protected override void BuildRenderTree(RenderTreeBuilder __builder)
		{
			__builder.OpenComponent<Router>(0);
			__builder.AddComponentParameter(1, "AppAssembly", RuntimeHelpers.TypeCheck<Assembly>(typeof(App).Assembly));
			__builder.AddAttribute(2, "Found", new RenderFragment<RouteData>((RouteData routeData) => delegate(RenderTreeBuilder __builder2)
			{
				__builder2.OpenComponent<RouteView>(3);
				__builder2.AddComponentParameter(4, "RouteData", RuntimeHelpers.TypeCheck<RouteData>(routeData));
				__builder2.CloseComponent();
			}));
			__builder.AddAttribute(5, "NotFound", new RenderFragment(delegate(RenderTreeBuilder __builder2)
			{
				__builder2.AddMarkupContent(6, "<p>Page not found.</p>");
			}));
			__builder.CloseComponent();
		}
	}
}
