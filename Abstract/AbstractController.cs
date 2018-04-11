using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ysriscorelibrary.Interfaces;
using System;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Abstract
{
    /// <summary>
    /// Abstract Controller CRUD actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractController<T> : Controller where T : class, IAbstractEntity, new()
    {
        protected DbContext _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">Db Context</param>
        public AbstractController(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Default List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual IQueryable<T> Get()
        {
            var set = _context.Set<T>();
            //if (set.Select(a => a.entityModel).Distinct().All(a => a == null))
            //    foreach (var a in set)
            //    {
            //        a.entityModel = _entityModel;
            //    }
            return set;
        }

        /// <summary>
        /// Get registres
        /// </summary>
        /// <param name="start">from query string</param>
        /// <param name="number">from query string</param>
        /// <param name="tableState">from query string</param>
        /// <returns></returns>
        [HttpGet("getfiltered")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "All")]
        public virtual IActionResult Get(int start = 0, int number = 100, TableStateEntity<T> tableStateObj = null)
        {
            var fullset = _context.Set<T>().AsQueryable();
            var set = fullset.Skip(start).Take(number).AsQueryable();
            return Ok(new
            {
                data = set,
                numberOfPages = Math.Ceiling(fullset.Count() / number * 1f),
            });
        }

        /// <summary>
        /// Get an empty entity
        /// </summary>
        /// <returns></returns>
        [HttpGet("empty")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual T GetEmpty()
        {
            var entity = new T { };
            //if (_entityModel != null)
            //    entity.entityModel = _entityModel;
            return entity;
        }

        /// <summary>
        /// Get a specific entity
        /// </summary>
        /// <param name="id">int identifier</param>
        /// <returns>Entity</returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _context.Set<T>().FindAsync(id);
            //if (_entityModel != null)
            //    entity.entityModel = _entityModel;

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> Put([FromRoute] int id, [FromBody] T client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // if (id != client.id)
            // {
            //     return BadRequest();
            // }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Create an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> Post([FromBody] T entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = entity.id }, entity);
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _context.Set<T>().FindAsync(id);
            if (client == null)
                return NotFound();

            _context.Set<T>().Remove(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        /// <summary>
        /// Check if entity exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual bool EntityExists(int id)
        {
            return _context.Set<T>().Find(id) != null;
        }
    }
}
