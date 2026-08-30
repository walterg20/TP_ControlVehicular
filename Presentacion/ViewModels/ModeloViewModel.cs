using AutoMapper;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ModeloViewModel : BaseViewModel
    {
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;

        public ModeloViewModel(IModeloRepository modeloRepository, IMapper mapper)
        {
            _modeloRepository = modeloRepository;
            _mapper = mapper;
            Modelos = new ObservableCollection<ModeloDto>();
            ListarCommand = new RelayCommand(async () => await ListarModelos());
        }

        public ObservableCollection<ModeloDto> Modelos { get; set; }
        public int IdMarca { get; set; }

        public ICommand ListarCommand { get; }

        private async Task ListarModelos()
        {
            Modelos.Clear();
            var lista = await _modeloRepository.GetByMarcaAsync(IdMarca);
            foreach (var m in lista)
                Modelos.Add(_mapper.Map<ModeloDto>(m));
        }
    }
}
