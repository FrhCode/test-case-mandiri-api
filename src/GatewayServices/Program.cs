using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// reverse proxy
builder.Services.AddReverseProxy()
		.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = builder.Configuration["IdentityServer:Authority"];
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters.ValidateAudience = false;
		options.TokenValidationParameters.NameClaimType = "username";

		options.TokenValidationParameters.ValidateIssuer = false;
	});

builder.Services.AddCors(options =>
{
	var origins = builder.Configuration.GetSection("Origins").Get<string[]>();
	options.AddPolicy("CorsPolicy", builder => builder
				.WithOrigins(origins)
			 	.AllowAnyHeader()
			 	.AllowAnyMethod()
			 	.AllowCredentials()
			 	.SetIsOriginAllowed((host) => true));
});

var app = builder.Build();

app.UseCors("CorsPolicy");

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
