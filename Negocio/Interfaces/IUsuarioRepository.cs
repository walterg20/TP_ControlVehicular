using TP_ControlVehicular.Entidad; namespace TP_ControlVehicular.Negocio.Interfaces; public interface IUsuarioRepository : IRepository<Usuario> { Task<IEnumerable<Usuario>> GetActivosAsync(); }
