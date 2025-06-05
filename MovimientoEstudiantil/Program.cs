using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Services;

var builder = WebApplication.CreateBuilder(args);

// CONTEXTO DE BASE DE DATOS
builder.Services.AddDbContext<MovimientoEstudiantilContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

// CORS para permitir llamadas desde MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWebMVC", policy =>
    {
        policy.WithOrigins("https://localhost:7187") // Puerto del proyecto MVC
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// AUTENTICACIÓN CON COOKIES
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".AspNetCore.Cookies";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/api/Auth/login";
        options.AccessDeniedPath = "/api/Auth/denegado";
    });

// POLÍTICAS DE AUTORIZACIÓN
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("Coordinador", policy => policy.RequireRole("Coordinador"));
});

// OTROS SERVICIOS
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HistorialService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("PermitirWebMVC");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// INICIALIZAR DATOS
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MovimientoEstudiantilContext>();

    try
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        // Crear provincia San José
        if (!await context.Provincias.AnyAsync(p => p.idProvincia == 1))
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SET IDENTITY_INSERT Provincia ON;";
            await cmd.ExecuteNonQueryAsync();

            context.Provincias.Add(new MovimientoEstudiantil.Models.Provincia
            {
                idProvincia = 1,
                nombre = "San José"
            });
            await context.SaveChangesAsync();

            cmd.CommandText = "SET IDENTITY_INSERT Provincia OFF;";
            await cmd.ExecuteNonQueryAsync();
        }

        // Crear sede Sede Central
        if (!await context.Sedes.AnyAsync(s => s.idSede == 1))
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SET IDENTITY_INSERT Sede ON;";
            await cmd.ExecuteNonQueryAsync();

            context.Sedes.Add(new MovimientoEstudiantil.Models.Sede
            {
                idSede = 1,
                nombre = "Sede Central",
                idProvincia = 1
            });
            await context.SaveChangesAsync();

            cmd.CommandText = "SET IDENTITY_INSERT Sede OFF;";
            await cmd.ExecuteNonQueryAsync();
        }

        // Crear usuario Coordinador
        if (!await context.Usuarios.AnyAsync(u => u.correo == "coordinador@ucr.ac.cr"))
        {
            context.Usuarios.Add(new MovimientoEstudiantil.Models.Usuario
            {
                correo = "coordinador@ucr.ac.cr",
                contrasena = BCrypt.Net.BCrypt.HashPassword("Coordinador123"),
                rol = "Coordinador",
                sede = 1,
                fechaRegistro = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al inicializar datos: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.Run();
