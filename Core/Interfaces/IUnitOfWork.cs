using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Esta interfaz imprementa el partron de diseño "Unit Of Work" y la utilizamos para administrar todos los repositorios
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Devuelve la instancia del repositorio dependiendo de la entidad seteada
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ClaseBase;

        /// <summary>
        /// Devuelve la cantidad de registros actualizados en la base de datos
        /// </summary>
        /// <returns></returns>
        Task<int> Complete();
    }
}
