using Amazon.SimpleEmail;
using SESDemo.Models;
using SESDemo.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MailOptions>(builder.Configuration.GetSection("MailOptions"));
builder.Services.AddKeyedTransient<IEmailService, SmtpEmailService>("smtp");

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleEmailService>();
builder.Services.AddKeyedTransient<IEmailService, SdkEmailService>("sdk");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/mail/{type}", (IServiceProvider provider, string type, MailRequest request) =>
{
    var service = provider.GetKeyedService<IEmailService>(type);
    if (service != null)
        service.SendEmailAsync(request);
});

app.UseHttpsRedirection();
app.Run();