using UsersStorrage.Models.Context;
using UsersStorrage.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using UserStorrage6.GraphQl.Query;
using UserStorrage6.GraphQl.Mutation;
using Microsoft.EntityFrameworkCore;
using UserStorrage6.Model.AutoMapper;
using UserStorrage6.Services;
using UserStorrage6.Model.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration(cb => { cb.AddEnvironmentVariables(); });

// Add services to the container.


builder.Services.AddControllers().AddNewtonsoftJson(o =>
    o.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error);

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    //.AddQueryableOffsetPagingProviderW()
    //.AddDefaultTransactionScopeHandler()
    .AddQueryType(q => q.Name("Query"))
    .AddType<UserQuery>()
    .AddType<PermissionQuery>()
    .AddType<RoleQuery>()
    .AddType<ServicesQuery>()
    .AddMutationType(m => m.Name("Mutation"))
    .AddType<UserMutation>()
    .AddType<PermissionMutation>()
    .AddType<ServiceMutation>()
    .AddType<RoleMutation>()
    ;

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", context =>
    {
        return Task.Run(() => context.Response.Redirect("graphql/"));
    });
});

app.MapGraphQL();
app.Run();
