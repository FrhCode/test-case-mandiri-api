
using AuctionService.RequestHelper;
using AuctionService.Services;
using BiddingService.Consumers;
using BiddingService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using SearchService.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddHostedService<CheckAuctionFinishedService>();

builder.Services.AddControllers();
builder.Services.AddMassTransit(x =>
{
	// register consumers
	x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = builder.Configuration["IdentityServer:Authority"];
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters.ValidateAudience = false;
		options.TokenValidationParameters.NameClaimType = "username";

		options.TokenValidationParameters.ValidateIssuer = false;
	});

builder.Services.AddScoped<GrpcAuctionClient>();

var app = builder.Build();


app.UseAuthorization();

app.MapControllers();

app.UseMongoDb(builder);

app.Run();
