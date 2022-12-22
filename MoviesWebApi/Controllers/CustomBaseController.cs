using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Helpers;

namespace MoviesWebApi.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(
            ApplicationDbContext context, 
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TRead>> Get<TEntity, TRead>() 
            where TEntity : class
        {
            var entities = await context
                .Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
            var dtos = mapper.Map<List<TRead>>(entities);
            return dtos;
        }

        protected async Task<List<TRead>> Get<TEntity, TRead>(Pagination pagination)
            where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            await HttpContext.InsertPaginationParameters(queryable, pagination.recordsPerPage);

            var entities = await queryable.Page(pagination).ToListAsync();
            return mapper.Map<List<TRead>>(entities);
        }
        
        protected async Task<List<TRead>> Get<TEntity, TRead>(Pagination pagination, IQueryable<TEntity> queryable)
            where TEntity : class
        {
            //var queryable = context.Set<TEntity>().AsQueryable();
            await HttpContext.InsertPaginationParameters(queryable, pagination.recordsPerPage);

            var entities = await queryable.Page(pagination).ToListAsync();
            return mapper.Map<List<TRead>>(entities);
        }

        protected async Task<ActionResult<TRead>> Get<TEntity, TRead>(int id) 
            where TEntity : class, IId
        {
            var entity = await context
                .Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if(entity is null)
            {
                return NotFound();
            }
            return mapper.Map<TRead>(entity);
        }

        protected async Task<ActionResult> Post<TCreation, TEntity, TRead>(
            TCreation creation, string routeName
            ) where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(creation);
            context.Add(entity);
            await context.SaveChangesAsync();
            var dtoRead = mapper.Map<TRead>(entity);
            return new CreatedAtRouteResult(routeName, new { id = entity.Id }, dtoRead);
        }

        protected async Task<ActionResult> Put<TUpdate, TEntity>(int id, TUpdate update)
            where TEntity : class, IId
        {
            var entity = mapper.Map<TEntity>(update);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TPatch>(int id, JsonPatchDocument<TPatch> jsonPatchDocument)
            where TEntity : class, IId
            where TPatch : class
        {
            if (jsonPatchDocument == null) return BadRequest();
            var entityDB = await context
                .Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entityDB is null) return NotFound();
            var patchEntity = mapper.Map<TPatch>(entityDB);
            jsonPatchDocument.ApplyTo(patchEntity, ModelState);
            var isValid = TryValidateModel(patchEntity);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(patchEntity, entityDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntity>(int id)
            where TEntity : class, IId, new()
        {
            var exists = await context
                .Set<TEntity>()
                .AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new TEntity(){ Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
