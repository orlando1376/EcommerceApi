using BusinessLogic.Data;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    /// <summary>
    /// administrar todos los repositorios
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly MarketDbContext _context;

        public UnitOfWork(MarketDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Guarda la orden de compra en la base de datos
        /// </summary>
        /// <returns></returns>
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cuando se elimina la instancia de la clase
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Devuelve la isntancia de un objeto de tipo Repository seteando el valor de la entidad (Producto, marca, categoria, oden de compra ...)
        /// Ej: GenericRepository<Producto>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ClaseBase
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            // entidad a setear
            var type = typeof(TEntity).Name;

            if (!_repositories.Contains(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>) _repositories[type];
        }
    }
}
