using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Bogus.DataSets.Name;

namespace MapDotGenerator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                RunSample();

                await Task.Delay(5000, stoppingToken);
            }
        }

        void RunSample()
        {
            var testUsers = new Faker<User>()
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.Name, (f, u) => f.Name.FullName())
                .RuleFor(u => u.Latitude, (f, u) => f.Address.Latitude(47.3022815, 47.5517946))
                .RuleFor(u => u.Longitude, (f, u) => f.Address.Longitude(-122.7553483, -121.881312))
                .FinishWith((f, u) =>
                {
                    Console.WriteLine(u.AsJson());
                });

            var user = testUsers.Generate(10);
        }
    }
}
