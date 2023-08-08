namespace EWS.BlazorServer.Setup;

public static class WebApplicationStartup
{
    public static WebApplication vUseBlazorSetup(this WebApplication app)
    {
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
        app.MapRazorPages();
        app.MapFallbackToPage("/_Host");

        return app;
    }
}