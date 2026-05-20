using STMS.Data;
using STMS.Repos;
using STMS.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        builder.Services.AddDbContext<StmsDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("StmsDbContext")));

        builder.Services.AddScoped<UserRepo>();
        builder.Services.AddScoped<TaskRepo>();          
        builder.Services.AddScoped<TaskStepRepo>();    
        builder.Services.AddScoped<CommentRepo>();       
        builder.Services.AddScoped<AttachmentRepo>();    

        builder.Services.AddAuthentication("StmsAuth")
            .AddCookie("StmsAuth", options =>
            {
                options.LoginPath         = "/Auth/Login";
                options.AccessDeniedPath  = "/Auth/Denied";
                options.ExpireTimeSpan    = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly   = true;
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}");

        app.MapControllers();

        app.Run();
    }
}
