using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Library.V1.Common;
using Newtonsoft.Json.Linq;

namespace Web.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // this code will influence Session working 
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            });
            

            // Jwt Bearer Tokken Authorization
            var key = Encoding.ASCII.GetBytes("infosecurity@shaolinworld.org");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Enable [JsonIgnore] attribute for Json Data Serialization and Deserialization
            services.AddControllers().AddNewtonsoftJson();
            
            services.AddDistributedMemoryCache(); //启用session之前必须先添加内存
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(8*3600);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            // MVC : enable MVC [JsonIgnore] attribute for Json Data Serialization and Deserialization
            services.AddMvc(
                options =>
                {
                    options.Filters.Add(new ErrorHandlingFilter());
                }

            ).AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Latest);
            
            AppSetting appSetting = this.Configuration.GetSection("Database").Get<AppSetting>();
            services.AddSingleton<AppSetting>(appSetting);
            //LanguageHelper langHelper = new LanguageHelper(appSetting);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || false)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // Force to use Https:  look at topic https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }
            app.UseCors("EnableCORS");
            app.UseHttpsRedirection();  // new 
            app.UseStaticFiles();
            app.UseRouting();           // new
            app.UseAuthorization();

            //UseCookiePolicy() will influence Session working
            app.UseCookiePolicy();
            app.UseSession();

            //Response Status Code Catch 
            //app.UseStatusCodePagesWithRedirects("/Home/Index?code={0}");
            //Important: don't use below code ,  it will cause multiple go through Home Controller and Index Page whatever page you request.
            #region Important: don't use below code, it will always route to HomeController and Index Page, then go to the page you request.
            /*
            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "text/plain";
                var path = context.HttpContext.Request.Path.Value;
                if (path.StartsWith("/Admin/api"))
                {
                    switch (context.HttpContext.Response.StatusCode)
                    {
                        case 401:
                            {
                                string errMsg = "invalid.authorization";
                                List<string> errMsgs = new List<string>();
                                errMsgs.Add(errMsg);
                                var err = new { error = new { code = ErrorCode.Authorization, message = errMsg, messages = errMsgs, memo = "/Admin/Home/Index" } };
                                await context.HttpContext.Response.WriteAsync(
                                    JObject.FromObject(err).ToString()
                                );
                            }
                            break;
                        case 404:
                            {
                                string errMsg = "invalid.notfound";
                                List<string> errMsgs = new List<string>();
                                errMsgs.Add(errMsg);
                                var err = new { error = new { code = ErrorCode.NotFound, message = errMsg, messages = errMsgs, memo = "" } };
                                await context.HttpContext.Response.WriteAsync(
                                    JObject.FromObject(err).ToString()
                                );
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (path.StartsWith("/Admin"))
                    {
                        switch (context.HttpContext.Response.StatusCode)
                        {
                            case 401:
                            case 404:
                                context.HttpContext.Response.Redirect("/Admin/Home/Index", false);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (context.HttpContext.Response.StatusCode)
                        {
                            case 401:
                            case 404:
                                context.HttpContext.Response.Redirect("/Home/Index", false);
                                break;
                            default:
                                break;
                        }
                    }
                }
            });
            */
            #endregion

            app.UseEndpoints(endpoints =>
            {
                /*
                endpoints.MapAreaControllerRoute(
                    name: "DefaultAreas",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
                */
                endpoints.MapControllerRoute(
                    name: "AllAreasApi",
                    pattern: "{area:exists}/api/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "AllAreas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "defaultApi",
                    pattern: "api/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HandleExceptionAsync(context);
            context.ExceptionHandled = true;
        }

        private static void HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is DivideByZeroException)
                SetExceptionResult(context, exception, HttpStatusCode.NotFound);
            else if (exception is FileNotFoundException)
                SetExceptionResult(context, exception, HttpStatusCode.Unauthorized);
            else if (exception is InvalidDataException)
                SetExceptionResult(context, exception, HttpStatusCode.BadRequest);
            else
                SetExceptionResult(context, exception, HttpStatusCode.InternalServerError);


        }
        private static void SetExceptionResult(ExceptionContext context, Exception exception, HttpStatusCode code)
        {
            List<string> errorMsg = new List<string>();
            errorMsg.Add(exception.Message);
            var errorObj = new { code = (int)code, messages = errorMsg, memo = "", message = exception.Message };

            context.Result = new JsonResult(new { error = errorObj })
            {
                StatusCode = (int)code
            };
        }
    }

}
