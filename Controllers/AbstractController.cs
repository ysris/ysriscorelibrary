﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ysriscorelibrary.Interfaces;
using System;
using YsrisCoreLibrary.Models;
using System.Collections;
using System.Collections.Generic;

namespace YsrisCoreLibrary.Controllers
{
    /// <summary>
    /// Abstract Controller CRUD actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractController<T> : Controller where T : class, IAbstractEntity, new()
    {
        protected readonly DbContext _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">Db Context</param>
        public AbstractController(DbContext context)
        {
            _context = context;
        }

        ///// <summary>
        ///// Default List
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        //public virtual IQueryable<T> Get()
        //{
        //    var set = _context.Set<T>();
        //    return set;
        //}

        /// <summary>
        /// Get paginated T enumeration
        /// </summary>
        /// <param name="start">from item</param>
        /// <param name="number">number of items to take</param>
        /// <param name="tableState"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "All")]
        public virtual async Task<IActionResult> Get(int start = 0, int number = 100, string tableStateObj = null)
        {
            var data = await _get(start, number);
            return Ok(new { data = data.Item1, numberOfPages = data.Item2 });
        }

        /// <summary>
        /// Get an empty entity
        /// </summary>
        /// <returns></returns>
        [HttpGet("empty")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual IActionResult GetEmpty()
        {
            return Ok(_getEmpty());
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
                return BadRequest(ModelState);

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                    return NotFound();
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
        /// Create an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> Patch([FromBody] T entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Set<T>().Update(entity);
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

            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return NotFound();

            entity.deletionDate = DateTime.Now;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
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

        protected virtual async Task<Tuple<IEnumerable<T>, int>> _get(int start, int number)
        {
            var fullset = _context.Set<T>().AsQueryable();
            var set = await fullset.Skip(start).Take(number).ToListAsync();
            var numberOfPages = Convert.ToInt32(Math.Ceiling(fullset.Count() / number * 1f));
            return new Tuple<IEnumerable<T>, int>(set, numberOfPages);
        }

        protected virtual async Task<T> _getEmpty()
        {
            return await Task.Run(() =>
            {
                var entity = new T { };
                return entity;
            });
        }
    }
}
