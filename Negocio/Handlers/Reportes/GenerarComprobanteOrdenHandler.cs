using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes;

public class GenerarComprobanteOrdenHandler
{
    private readonly CVDbContext _dbContext;

    public GenerarComprobanteOrdenHandler(CVDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ComprobanteOrdenDto?> HandleAsync(int ordenId)
    {
        var parametro = new SqlParameter("@OrdenId", ordenId);
        var resultados = await _dbContext.Database
            .SqlQueryRaw<ComprobanteOrdenRowDto>("EXEC sp_GenerarComprobanteOrden @OrdenId", parametro)
            .ToListAsync();
            
        if (!resultados.Any()) return null;

        var cabecera = resultados.First();
        var dto = new ComprobanteOrdenDto
        {
            OrdenId = cabecera.OrdenId,
            ClienteNombre = cabecera.ClienteNombre,
            ClienteDocumento = cabecera.ClienteDocumento,
            VehiculoPatente = cabecera.VehiculoPatente,
            VehiculoMarca = cabecera.VehiculoMarca,
            VehiculoModelo = cabecera.VehiculoModelo,
            FechaRecepcion = cabecera.FechaRecepcion,
            FechaEstimadaEntrega = cabecera.FechaEstimadaEntrega,
            Observaciones = cabecera.Observaciones,
            TallerNombre = cabecera.TallerNombre,
            TallerDireccion = cabecera.TallerDireccion,
            TallerTelefono = cabecera.TallerTelefono,
            Detalles = new List<DetalleOrdenDto>()
        };

        foreach (var row in resultados.Where(r => r.DetalleId.HasValue))
        {
            dto.Detalles.Add(new DetalleOrdenDto
            {
                ProductoOServicio = row.ProductoOServicio ?? "Servicio",
                Observacion = row.ObservacionDetalle ?? string.Empty
            });
        }

        return dto;
    }
}
