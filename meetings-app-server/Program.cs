using meetings_app_server.Controllers;
using meetings_app_server.Data;
using meetings_app_server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using meetings_app_server.CustomConverter;
using meetings_app_server.Mapping;
using System.Buffers.Text;

var builder = WebApplication.CreateBuilder(args);


// Configure DB service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite("Data Source=app.db"));


// Add Identity services
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        AuthenticationType = "Jwt",
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidAudience = builder.Configuration["Jwt:Audience"],
        // or try this if the above does not work
        // ValidAudiences = new[] { builder.Configuration["Jwt:Audience"] }

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("AscendionAPI")
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 5;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddScoped<ITokenRepository, JwtTokenRepository>();


builder.Services.AddEndpointsApiExplorer();

// Mody the AddSwaggerGen() call like so...
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "meetings-app-server", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    // Retrieve the SuperUserId directly from configuration (appsettings.json)
    var superUserId = builder.Configuration["SuperUserSettings:SuperUserId"];

    // Conditionally hide endpoints based on SuperUserId check
    options.DocInclusionPredicate((docName, apiDescription) =>
    {
        var userId = ""; // You may need to retrieve the current user's ID based on the authentication context.

        // Here, we're assuming you have the user's ID from the current authentication context, this needs to be handled based on how you're authenticating users
        if (userId != superUserId)
        {
            // Hide specific endpoints when user is not a super user
            if (apiDescription.RelativePath.Contains("AllMeetings") || apiDescription.RelativePath.Contains("DeleteMeeting/{id}"))
            {
                return false; // Don't show these endpoints for non-super users
            }
        }

        return true; // Show the endpoint for super users
    });
});
// Add other services and controllers
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Register custom TimeOnly JSON converter
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            //policy.WithOrigins(new string[] { "http://localhost:4200" }) // Replace with your allowed origin(s)
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure middleware and routing
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS policy
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
