var builder = WebApplication.CreateBuilder(args);

// 1. Menambahkan layanan untuk Swagger/OpenAPI
// Ini adalah metode yang benar untuk template standar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// 2. Mengaktifkan middleware Swagger (biasanya hanya di mode Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();