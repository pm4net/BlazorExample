using BlazorExample.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.OCEL;

namespace BlazorExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Enrich.WithCorrelationId()
                .WriteTo.Console()
                .WriteTo.OcelLiteDbSink(new LiteDbSinkOptions(string.Empty, "blazor-logs.db", RollingPeriod.Never))
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddRazorPages();
                builder.Services.AddServerSideBlazor();
                builder.Services.AddSingleton<WeatherForecastService>();
                builder.Services.AddHttpContextAccessor();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseStaticFiles();

                app.UseRouting();

                app.MapBlazorHub();
                app.MapFallbackToPage("/_Host");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error setting up the application");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}