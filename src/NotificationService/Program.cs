using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", builder => builder
				.WithOrigins("http://localhost:3000")
			 	.AllowAnyHeader()
			 	.AllowAnyMethod()
			 	.AllowCredentials()
			 	.SetIsOriginAllowed((host) => true));
});

builder.Services.AddMassTransit(x =>
{

	// register consumers
	x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));

	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
		{
			h.Username(builder.Configuration["RabbitMq:Username"]);
			h.Password(builder.Configuration["RabbitMq:Password"]);
		});

		cfg.ConfigureEndpoints(context);
	});

});
builder.Services.AddSignalR();

var app = builder.Build();

// end point to /test
app.MapGet("/test", () => "Hello World");

app.UseCors("CorsPolicy");
app.MapHub<NotificationHubs>("/notificationHub");
app.Run();
