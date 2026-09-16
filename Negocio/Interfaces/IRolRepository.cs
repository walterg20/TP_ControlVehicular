using TP_ControlVehicular.Entidad; namespace TP_ControlVehicular.Negocio.Interfaces; public interface IRolRepository : IRepository<Rol> { Task<IEnumerable<Rol>> GetActivosAsync(); }
