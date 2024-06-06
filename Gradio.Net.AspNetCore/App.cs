using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public static class App
    {
        public static void Launch(Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null, params string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGradio();

            WebApplication app = builder.Build();

            app.UseGradio(blocks, additionalConfigurationAction);

            app.Run();
        }
    }
}
