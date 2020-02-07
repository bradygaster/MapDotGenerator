using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Bogus.DataSets.Name;

namespace MapDotGenerator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _hubUrl;
        private HubConnection _hubConnection;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _hubUrl = _configuration.GetValue<string>("MapHubUrl");
        }
        
        private async Task StartConnectionAsync()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()  
                    .WithUrl(new Uri(_hubUrl))
                    .Build();

                _hubConnection.Closed += async (ex) => { await StartConnectionAsync(); };

                await _hubConnection.StartAsync();
            }
            catch
            {
                _logger.LogError("SignalR endpoint can't be reached");
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartConnectionAsync();
                
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if(_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                {
                    GenerateMapPoint();
                }
                else
                {
                    await StartConnectionAsync();
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        void GenerateMapPoint()
        {
            var testUsers = new Faker<User>()
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.Name, (f, u) => f.Name.FullName())
                .RuleFor(u => u.Latitude, (f, u) => f.Address.Latitude(47.3022815, 47.5517946))
                .RuleFor(u => u.Longitude, (f, u) => f.Address.Longitude(-122.7553483, -121.881312))
                .FinishWith((f, u) =>
                {
                    _hubConnection.SendAsync("ReceiveLocation", u.Name, u.Latitude, u.Longitude);
                    Console.WriteLine(u.AsJson());
                });

            var user = testUsers.Generate(1);
        }
    }
}
