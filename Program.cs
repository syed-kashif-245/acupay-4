
using AcuPay.Data;
using AcuPay.Repositories;
using AcuPay.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Register database connection factory
builder.Services.AddScoped<DbConnectionFactory>();

// 3. Register repositories
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

// 4. Register Invoice Matching Service
builder.Services.AddScoped<InvoiceMatchingService>(provider =>
{
    var invoiceRepository = provider.GetRequiredService<IInvoiceRepository>();
    var purchaseOrderRepository = provider.GetRequiredService<IPurchaseOrderRepository>();

    return new InvoiceMatchingService(
        5.0m,
        invoiceRepository,
        purchaseOrderRepository
    );
});

// 5. Build application
var app = builder.Build();

// 6. Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 7. Run application
app.Run();
