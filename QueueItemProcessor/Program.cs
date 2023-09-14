using Amazon.SQS;
using QueueItemProcessor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var options = hostContext.Configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(options);
        services.AddAWSService<IAmazonSQS>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
