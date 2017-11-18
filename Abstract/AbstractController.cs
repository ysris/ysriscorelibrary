using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using YsrisCoreLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Abstract
{
    public class AbstractController<T> : Controller where T : class, IAbstractEntity, new()
    {
        protected DbContext _context;

        protected List<object> _entityModel;

        public AbstractController(DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public virtual IQueryable<T> Get()
        {
            var set = _context.Set<T>();
            if (set.Select(a => a.entityModel).Distinct().All(a => a == null))
                foreach (var a in set)
                {
                    a.entityModel = _entityModel;
                }
            return set;
        }

        [HttpGet("empty")]
        [Authorize]
        public virtual T GetEmpty()
        {
            var entity = new T { };
            if (_entityModel != null)
                entity.entityModel = _entityModel;
            return entity;
        }

        [HttpGet("{id}")]
        [Authorize]
        public virtual async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _context.Set<T>().FindAsync(id);
            if (_entityModel != null)
                entity.entityModel = _entityModel;

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPut("{id}")]
        [Authorize]
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

        [HttpPost]
        [Authorize]
        public virtual async Task<IActionResult> Post([FromBody] T entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = entity.id }, entity);
        }

        [HttpDelete("{id}")]
        [Authorize]
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

        protected virtual bool EntityExists(int id)
        {
            return _context.Set<T>().Find(id) != null;
        }
    }
}
