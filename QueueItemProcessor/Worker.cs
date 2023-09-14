using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace QueueItemProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAmazonSQS _amazonSQS;
        private readonly string _queueName = "https://sqs.ap-southeast-2.amazonaws.com/878770420070/simple-queue";
        private readonly string _processed = "https://sqs.ap-southeast-2.amazonaws.com/878770420070/processed-queue";

        public Worker(ILogger<Worker> logger, IAmazonSQS amazonSQS)
        {
            _logger = logger;
            _amazonSQS = amazonSQS;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var request = new ReceiveMessageRequest
                    {
                        QueueUrl = _queueName,
                        WaitTimeSeconds = 5
                    };

                    var result = await _amazonSQS.ReceiveMessageAsync(request);
                    if (result.Messages.Any())
                    {
                        foreach (var message in result.Messages)
                        {
                            _logger.LogInformation("Processing Message {message} | {time}", message.Body, DateTime.Now);

                            var processedMessage = new ProcessedMessage(message.Body);

                            var sendRequest = new SendMessageRequest(_processed, JsonConvert.SerializeObject(processedMessage));

                            var sendResult = await _amazonSQS.SendMessageAsync(sendRequest, stoppingToken);

                            if (sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK)
                                await _amazonSQS.DeleteMessageAsync(_queueName, message.ReceiptHandle);
                        }
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogInformation(ex.InnerException.Message);
                }



                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}