using AutoMapper;

using System.Collections.ObjectModel;

using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class MarcaViewModel : BaseViewModel
    {
        private readonly IMarcaRepository _marcaRepository;
        private readonly IMapper _mapper;

        public MarcaViewModel(IMarcaRepository marcaRepository, IMapper mapper)
        {
            _marcaRepository = marcaRepository;
            _mapper = mapper;
            Marcas = new ObservableCollection<MarcaDto>();
            ListarCommand = new RelayCommand(async () => await ListarMarcas());
        }

        public ObservableCollection<MarcaDto> Marcas { get; set; }
        public string NombreMarca { get; set; } = string.Empty;

        public ICommand ListarCommand { get; }

        private async Task ListarMarcas()
        {
            Marcas.Clear();
            var marca = await _marcaRepository.GetByNombreAsync(NombreMarca);
            if (marca != null)
                Marcas.Add(_mapper.Map<MarcaDto>(marca));
        }
    }
}
