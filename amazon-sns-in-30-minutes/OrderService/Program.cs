using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using OrderService.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/", async (CreateOrderRequest request, IAmazonSimpleNotificationService sns) =>
{
    // assume the incoming request is processed and saved to the database.

    // create notification
    var notification = new OrderCreatedNotification(request.OrderId, request.CustomerId, request.ProductDetails);

    // create topic if needed
    var topicName = "OrderCreated";
    var topicArn = string.Empty;
    var topicExists = await sns.FindTopicAsync(topicName);
    if (topicExists != null)
    {
        topicArn = topicExists.TopicArn;
    }
    else
    {
        var newTopic = await sns.CreateTopicAsync(topicName);
        topicArn = newTopic.TopicArn;
    }

    // create publish request
    var publishRequest = new PublishRequest()
    {
        TopicArn = topicArn,
        Message = JsonSerializer.Serialize(notification),
        Subject = $"Order#{request.OrderId}"
    };

    publishRequest.MessageAttributes.Add("Scope", new MessageAttributeValue()
    {
        DataType = "String",
        StringValue = "Lambda"
    });

    await sns.PublishAsync(publishRequest);
});

app.Run();