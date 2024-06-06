using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.AspNetCore
{
    internal class EventWorker : BackgroundService
    {
        private readonly GradioApp _gradioApp;

        public EventWorker(GradioApp gradioApp)
        {
            _gradioApp = gradioApp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _gradioApp.StartEventLoopAsync(stoppingToken);
        }
    }
}
