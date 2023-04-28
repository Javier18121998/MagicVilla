using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_API.Repositorio.IRepositorio;
using System.Linq.Expressions;
using MagicVilla_API.Datos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public async Task Crear(T entidad)
        {
            await dbSet.AddAsync(entidad);
            await Grabar();
        }
        public async Task Grabar()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<T> Obtener(Expression<Func<T, bool>>? filtro = null, 
                                     bool tracked = true,
                                     string? incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            //necesitamos propiedades de otra table (datos de otra tabla y necesita estar relacionada) *Villa,Otro Modelo
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                   query = query.Include(incluirProp); 
                }
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null,
                                                string? incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if(incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                   query = query.Include(incluirProp); 
                }
            }
            return await query.ToListAsync();
        }
        public async Task Remover(T entidad)
        {
            dbSet.Remove(entidad);
            await Grabar();
        }
    }
}