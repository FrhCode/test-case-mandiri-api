using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.RequestHelper;
using AuctionService.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
		options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSingleton<IFileUploadService, FileUploadService>();

builder.Services.AddDbContext<AuctionDbContext>(options =>
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("default"));
});

builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddMassTransit(x =>
{

	// bus outbox
	x.AddEntityFrameworkOutbox<AuctionDbContext>(opt =>
	{
		opt.UsePostgres();
		opt.QueryDelay = TimeSpan.FromSeconds(10);
		opt.UseBusOutbox();
	});

	// register consumers
	x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();
	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

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

		// no need to validate issuer
		options.TokenValidationParameters.ValidateIssuer = false;
	});

builder.Services.AddGrpc();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assets")),
	RequestPath = "/Assets"
});

app.UseAuthentication();
app.UseAuthorization();

// seed the database
app.UseMigration();

app.MapControllers();
app.MapGrpcService<GrpcAuctionService>();

app.Run();
