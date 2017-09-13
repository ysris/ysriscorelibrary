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
        protected readonly DbContext _context;

        public AbstractController(DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public virtual IEnumerable<T> Get()
        {
            return _context.Set<T>();
        }

        [HttpGet("empty")]
        public virtual T GetEmpty()
        {
            return new T();
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Set<T>().FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> PutClient([FromRoute] int id, [FromBody] T client)
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
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostClient([FromBody] T client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Set<T>().Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.id }, client);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Set<T>().FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Set<T>().Remove(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        protected virtual bool ClientExists(int id)
        {
            return _context.Set<T>().Find(id) != null;
        }
    }
}
