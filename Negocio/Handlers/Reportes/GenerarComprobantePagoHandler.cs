using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes;

public class GenerarComprobantePagoHandler
{
    private readonly CVDbContext _dbContext;

    public GenerarComprobantePagoHandler(CVDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ComprobantePagoDto?> HandleAsync(int ordenId)
    {
        var parametro = new SqlParameter("@OrdenId", ordenId);
        var resultados = await _dbContext.Database
            .SqlQueryRaw<ComprobantePagoRowDto>("EXEC sp_GenerarComprobantePago @OrdenId", parametro)
            .ToListAsync();
            
        if (!resultados.Any()) return null;

        var cabecera = resultados.First();
        
        var dto = new ComprobantePagoDto
        {
            OrdenId = cabecera.OrdenId,
            FechaPago = cabecera.FechaPago,
            Kilometraje = cabecera.Kilometraje,
            TallerNombre = cabecera.TallerNombre,
            TallerDireccion = cabecera.TallerDireccion,
            TallerTelefono = cabecera.TallerTelefono,
            ClienteNombre = cabecera.ClienteNombre,
            ClienteDocumento = cabecera.ClienteDocumento,
            ClienteTelefono = cabecera.ClienteTelefono,
            ClienteDireccion = cabecera.ClienteDireccion,
            ClienteEmail = cabecera.ClienteEmail,
            VehiculoPatente = cabecera.VehiculoPatente,
            VehiculoDescripcion = cabecera.VehiculoDescripcion,
            Detalles = new List<DetalleComprobanteDto>()
        };

        foreach (var row in resultados.Where(r => r.DetalleId.HasValue))
        {
            dto.Detalles.Add(new DetalleComprobanteDto
            {
                ProductoOServicio = row.ProductoOServicio ?? "Servicio",
                Precio = row.Precio ?? 0,
                Cantidad = row.Cantidad ?? 1,
                Subtotal = row.SubtotalDetalle ?? 0
            });

            if (row.Origen == "Taller")
                dto.TotalManoObra += row.SubtotalDetalle ?? 0;
            else
                dto.TotalRepuestos += row.SubtotalDetalle ?? 0;
        }

        dto.Subtotal = dto.TotalManoObra + dto.TotalRepuestos;
        dto.Iva = dto.Subtotal * 0.15m; // Usando 15% para coincidir con la imagen (7800 / 52000 = 0.15)
        dto.TotalGeneral = dto.Subtotal + dto.Iva;

        return dto;
    }
}
