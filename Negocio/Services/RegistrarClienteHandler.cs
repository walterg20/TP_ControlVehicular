using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarClienteHandler
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public RegistrarClienteHandler(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<ClienteDto> HandleAsync(Cliente cliente)
        {
            if (cliente.Id > 0)
            {
                await _clienteRepository.UpdateAsync(cliente);
            }
            else
            {
                await _clienteRepository.AddAsync(cliente);
            }

            return _mapper.Map<ClienteDto>(cliente);
        }
    }
}
