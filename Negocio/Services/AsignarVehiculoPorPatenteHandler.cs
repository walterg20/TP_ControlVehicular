using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class AsignarVehiculoPorPatenteHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IRepository<PropietarioVehiculo> _propietarioRepository;
        private readonly IMapper _mapper;

        public AsignarVehiculoPorPatenteHandler(
            IVehiculoRepository vehiculoRepository,
            IRepository<PropietarioVehiculo> propietarioRepository,
            IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _propietarioRepository = propietarioRepository;
            _mapper = mapper;
        }

        public async Task<VehiculoDto?> HandleAsync(string patente, int idCliente)
        {
            var vehiculo = await _vehiculoRepository.GetByPatenteAsync(patente);
            
            if (vehiculo == null)
            {
                return null;
            }

            var propietarioActual = vehiculo.Propietarios.FirstOrDefault(p => p.EsActual);
            
            if (propietarioActual != null && propietarioActual.ClienteId == idCliente)
            {
                return _mapper.Map<VehiculoDto>(vehiculo);
            }

            if (propietarioActual != null)
            {
                propietarioActual.EsActual = false;
                propietarioActual.FechaVenta = DateTime.Now;
                await _propietarioRepository.UpdateAsync(propietarioActual);
            }

            var nuevoPropietario = new PropietarioVehiculo
            {
                ClienteId = idCliente,
                VehiculoId = vehiculo.Id,
                FechaAdquisicion = DateTime.Now,
                EsActual = true
            };

            await _propietarioRepository.AddAsync(nuevoPropietario);

            var vehiculoActualizado = await _vehiculoRepository.GetByPatenteAsync(patente);
            return _mapper.Map<VehiculoDto>(vehiculoActualizado);
        }
    }
}
