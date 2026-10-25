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

            // Forzamos todo a Mano de Obra y Repuestos a 0
            dto.TotalManoObra += row.SubtotalDetalle ?? 0;
            dto.TotalRepuestos = 0;
        }

        dto.Subtotal = dto.TotalManoObra + dto.TotalRepuestos;
        dto.Iva = 0; // IVA en 0 según solicitud
        dto.TotalGeneral = dto.Subtotal + dto.Iva;

        var registro = await _dbContext.RegistroServicios.FirstOrDefaultAsync(r => r.Id == ordenId);
        dto.EstadoOrden = registro?.Estado ?? "";

        return dto;
    }
}
