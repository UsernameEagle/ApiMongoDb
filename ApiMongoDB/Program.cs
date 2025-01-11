using ApiMongoDB;
using ApiMongoDB.Models;
using ApiMongoDB.Repositories;
using ApiMongoDB.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*********** Para utilizar EF CORE en Courses ************/

var mongoDBSettings = builder.Configuration.GetSection("SchoolDatabaseSettings").Get<SchoolDatabaseSettings>();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseMongoDB(mongoDBSettings?.ConnectionString!, mongoDBSettings?.DatabaseName!));

/*********** FIN para utilizar EF CORE ************/


/*********** INICIO para NO utilizar EF CORE en Students ************/

builder.Services.Configure<SchoolDatabaseSettings>(
    builder.Configuration.GetSection("SchoolDatabaseSettings")
  );

builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var connectionString = builder.Configuration.GetSection("SchoolDatabaseSettings:ConnectionString")?.Value;

    return new MongoClient(connectionString);
});

/*********** FIN para NO utilizar EF CORE  ************/

builder.Services.AddSingleton<IStudentService, StudentService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<MongoDbContext>();
builder.Services.AddSingleton<Instrumentor>();

var otlURL = $"http://{builder.Configuration["OTL:AgentHost"]}:{builder.Configuration["OTL:AgentPort"]}" ?? "http://localhost:4317";

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(Instrumentor.ServiceName))
        .AddConsoleExporter();

    options.AddOtlpExporter(otl => { otl.Endpoint = new Uri(otlURL); });
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(Instrumentor.ServiceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddConsoleExporter()
          .AddSource($"Tracing.{Instrumentor.ServiceName}")
          .AddOtlpExporter(otl =>
          {
              otl.Endpoint = new Uri(otlURL);
          })
          )
      .WithMetrics(metrics => metrics
        .AddMeter(Instrumentor.ServiceName)
        .ConfigureResource(resource => resource
            .AddService(Instrumentor.ServiceName))
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEventCountersInstrumentation(c =>
        {
            // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
            c.AddEventSources(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft-AspNetCore-Server-Kestrel",
                "System.Net.Http",
                "System.Net.Sockets");
        })
        .AddOtlpExporter(otl =>
        {
            otl.Endpoint = new Uri(otlURL);
        })
        );




//builder.Services.AddHangfire(configuration => configuration
//            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//            .UseSimpleAssemblyNameTypeSerializer()
//            .UseRecommendedSerializerSettings()
//            .UseConsole()
//            .UseMongoStorage(builder.Configuration.GetSection("SchoolDatabaseSettings:ConnectionString")?.Value, new MongoStorageOptions
//            {
//                MigrationOptions = new MongoMigrationOptions
//                {
//                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
//                    BackupStrategy = new CollectionMongoBackupStrategy(),
//                },
//                Prefix = "todo.hangfire",
//                CheckConnection = false,
//                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
//            })
//        );

//builder.Services.AddHangfireServer();


//// Configurar Serilog
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.Async(a => a.File("logs/log-.txt", rollingInterval: RollingInterval.Day))
//    .CreateLogger();

const string outputTemplate =
    "[{Level:w}]: {Timestamp:dd-MM-yyyy:HH:mm:ss} {MachineName} {EnvironmentName} {SourceContext} {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.OpenTelemetry(opts =>
    {
        opts.ResourceAttributes = new Dictionary<string, object>
        {
            ["app"] = "WebApi",
            ["runtime"] = "dotnet",
            ["service.name"] = "ApiMongoDB"
        };
    })
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API mongoDB",
        Description = "API de ejemplo para uso de mongoDB y otros casos futuros",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ejemplo de API"));


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();




// Usa Hangfire
//app.UseHangfireDashboard();
//app.UseHangfireDashboard();


// Configura un trabajo recurrente que se ejecuta cada minuto
//RecurringJob.AddOrUpdate(
//    "recurring-job",
//    () => HangFireJobs.RunRecurringJob(),
//    Cron.Minutely // Configuración para que se ejecute cada minuto
//);


app.Run();

public partial class Program { }