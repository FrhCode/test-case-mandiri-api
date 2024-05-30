using MassTransit;
using SearchService.Consumer;
using SearchService.Mongo;
using SearchService.RequestHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddMassTransit(x =>
{
	x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
		{
			h.Username(builder.Configuration["RabbitMq:Username"]);
			h.Password(builder.Configuration["RabbitMq:Password"]);
		});

		// configure retry policy for search-auction-created
		cfg.ReceiveEndpoint("search-auction-created", e =>
		{
			e.UseMessageRetry(r => r.Interval(3, 1000));
			e.ConfigureConsumer<AuctionCreatedConsumer>(context);
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMongoDb(builder);

app.Run();
