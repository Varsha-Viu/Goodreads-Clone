using System.Text;
using API.Constants;
using API.Data;
using API.Interfaces;
using API.Modals;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// sql connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity user 
builder.Services.AddIdentity<Users, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

// authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // jwt bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

builder.Services.AddAuthorization();

// swagger authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Goodreads API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "enter token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer "
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }

            },
            new List<string>()
        }
    });
    c.MapType<UserRoles>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(UserRoles))
        .Select(name => new OpenApiString(name))
                  .Cast<IOpenApiAny>()
                  .ToList()
    });
});

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors",
        b =>
        {
            b
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// add roles to IdentityRole
static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();

    string[] roleNames = { "Admin", "User", "Author" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminUser = await userManager.FindByEmailAsync("admin@yopmail.com");
    if(adminUser == null)
    {
        var user = new Users
        {
            Email = "admin@yopmail.com",
            UserName = "admin",
            FirstName = "varsha",
            LastName = "pathak",
            PhoneNumber = "1236547890",
            Address = "Ludhiana",
            PasswordHash = "Admin@1234",
            LockoutEnabled = true,
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            LockoutEnd = DateTime.Now
        };
        var result = await userManager.CreateAsync(user, "Admin@1234");
        if(result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

// services scoped
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BooksServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// add roles to IdentityRole
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRolesAsync(serviceProvider);
}

// user cors
app.UseCors("AppCors");

app.UseAuthorization();

// use authentication
app.UseAuthentication(); 

app.MapControllers();

app.Run();

