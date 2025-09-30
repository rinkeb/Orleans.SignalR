using ManagedCode.Orleans.SignalR.Core.Config;
using ManagedCode.Orleans.SignalR.Server.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Xunit;

namespace ManagedCode.Orleans.SignalR.Tests.Cluster;

[CollectionDefinition(nameof(SiloCluster))]
public class SiloCluster : ICollectionFixture<SiloCluster>, IAsyncLifetime
{
    public InProcessTestCluster Cluster { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var clusterBuilder = new InProcessTestClusterBuilder();
        clusterBuilder.ConfigureSilo((options, builder) => 
        { 
            builder.ConfigureOrleansSignalR();
            builder.AddMemoryGrainStorage(OrleansSignalROptions.OrleansSignalRStorage);

            builder.Services            
                .AddSignalR()
                .AddOrleans();
        });

        clusterBuilder.ConfigureClient(builder => 
            builder.Services
                .AddSignalR()
                .AddOrleans());

        Cluster = clusterBuilder.Build();
        await Cluster.DeployAsync();
    }

    public async Task DisposeAsync()
    {
        await Cluster.DisposeAsync();
    }
}