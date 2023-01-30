using UsersStorrage.Models.Context;
using UsersStorrage.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using UserStorrage6.GraphQl.Query;
using UserStorrage6.GraphQl.Mutation;
using Microsoft.EntityFrameworkCore;
using UserStorrage6.Model.AutoMapper;
using UserStorrage6.Services;
using UserStorrage6.Model.Context;
using UserStorrage6.Model.DB;
using UserStorrage6.Services.Interfaces;
using UserStorrage6.Services.Brockers;
using UserStorrage6.Model.Context.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration(cb => { cb.AddEnvironmentVariables(); });

// Add services to the container.


builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;

    //o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
    //o.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
    //o.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error);

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddTransient<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>();

//builder.Services.AddScoped<IServicesService, ServicesService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IRoleService, RoleService>();
//builder.Services.AddScoped<IPermissionService, PermissionService>();
//builder.Services.AddScoped<IHistorryService, HistorryService>();
//builder.Services.AddScoped<ISynhronizeService<Permission>, SynhronizeService<Permission>>();
//builder.Services.AddScoped<ISynhronizeService<Role>, SynhronizeService<Role>>();


#if DEBUG
builder.Services.AddSingleton<TestBrocker>();
#endif

builder.Services.AddScoped<IRoleBrockerService, RoleBrockerService>();
builder.Services.AddScoped<IServicesBrockersService, ServicesBrockerService>();
builder.Services.AddScoped<IPermissionBrockerService, PermissionBrockerServices>();
builder.Services.AddScoped<IHistorryBrockerService, HistorryBrockerService>();
builder.Services.AddScoped<ISynhronizeService<Permission>, SynhronizeService<Permission>>();
builder.Services.AddScoped<ISynhronizeService<Role>, SynhronizeService<Role>>();
builder.Services.AddScoped<ISynhronizeService<User>, SynhronizeService<User>>();
builder.Services.AddScoped<IUserBrockerService, UserBrockerService>();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
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
