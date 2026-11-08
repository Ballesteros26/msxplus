using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NDP_Online;

// Token: 0x02000006 RID: 6
[CompilerGenerated]
internal class Program
{
	// Token: 0x0600001E RID: 30 RVA: 0x000026D4 File Offset: 0x000008D4
	private static async Task <Main>$(string[] args)
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");
		builder.Services.AddScoped<HttpClient>((IServiceProvider _) => new HttpClient
		{
			BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
		});
		await builder.Build().RunAsync();
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002720 File Offset: 0x00000920
	private static void <Main>(string[] args)
	{
		Program.<Main>$(args).GetAwaiter().GetResult();
	}
}
