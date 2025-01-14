using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VioVid.Core.Identity;
using VioVid.Core.ServiceContracts;
using VioVid.Core.Services;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.CustomJsonConverter;
using VioVid.WebAPI.Filters;
using VioVid.WebAPI.Middlewares;
using VioVid.WebAPI.ServiceContracts;
using VioVid.WebAPI.Services;

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("viovid-e5899-firebase-adminsdk-97e76-ef3d3b9af9.json")
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        // Authorization policy
        // var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        // options.Filters.Add(new AuthorizeFilter(policy));
        // Model Validation
        options.Filters.Add<ModelValidation>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddTransient<IJwtService, JwtService>()
    .AddTransient<IEmailSender, EmailSender>()
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IGenreService, GenreService>()
    .AddScoped<IVnPayService, VnPayService>()
    .AddScoped<IStripeService, StripeService>()
    .AddScoped<IMomoService, MomoService>()
    .AddScoped<IPaymentService, PaymentService>()
    .AddScoped<IGenreService, GenreService>()
    .AddScoped<IPlanService, PlanService>()
    .AddScoped<IPersonService, PersonService>()
    .AddScoped<ITopicService, TopicService>()
    .AddScoped<IFilmService, FilmService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IPostService, PostService>()
    .AddScoped<IPostCommentService, PostCommentService>()
    .AddScoped<IChannelService, ChannelService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IDashboardService, DashboardService>()
    .AddScoped<INotificationService, NotificationService>()
    .AddScoped<IPushNotificationService, PushNotificationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS:
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        // options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            /*
             * Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
             * Tức là nó phải pass những điều kiện ở trên "TokenValidationParameters"
             * Thì OnTokenValidated mới được gọi
             */
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal.FindFirstValue("UserId");
                if (userIdClaim == null) context.Fail("Token không chứa thuộc tính UserId.");

                var userManager =
                    context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

                var user = await userManager.FindByIdAsync(userIdClaim);
                var tokenVersionClaim = context.Principal.FindFirstValue("tokenVersion");

                if (user == null)
                    context.Fail("Không tìm thấy người dùng");
                else if (!int.TryParse(tokenVersionClaim, out var intTokenVersion) ||
                         intTokenVersion != user.TokenVersion) context.Fail("Access Token không hợp lệ.");
            },
            /* Invoked before a challenge is sent back to the caller. */
            OnChallenge = context =>
            {
                // Customize the response if token validation fails
                if (!context.Handled)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        error = "invalid_token",
                        error_description = context.AuthenticateFailure?.Message ?? "User authentication failed"
                    });

                    return context.Response.WriteAsync(result);
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // The email confirmation token will expire after 3 hours
    options.TokenLifespan = TimeSpan.FromHours(3);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();