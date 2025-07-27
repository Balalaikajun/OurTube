using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Exceptions;

namespace OurTube.Application.Extensions;

/// <summary>
/// Расширение с запросами для повторного использования
/// </summary>
public static class Repository
{
    /// <summary>
    /// Найти сущность по Id
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="canModify">Флаг отслеживания сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <returns>Сущность</returns>
    /// <exception cref="NotFoundException">Элемент с данным идентификатором не найден</exception>
    public static async Task<TEntity> GetByIdAsync<TEntity>(this DbSet<TEntity> dbSet, Guid id, bool canModify = false) where TEntity : Base
    {
        var query = canModify 
            ? dbSet 
            : dbSet.AsNoTracking();

        var entity = await query.SingleOrDefaultAsync(e => e.Id == id);

        if (entity == null)
            throw new NotFoundException(typeof(TEntity), id);

        return entity;
    }
    
    /// <summary>
    /// Найти сущность по Id (nullable).
    /// </summary>
    /// <param name="id">Идентификатор сущности (может быть null)</param>
    /// <param name="canModify">Флаг отслеживания сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <returns>Сущность или null, если <paramref name="id"/> равен null</returns>
    /// <remarks>
    /// Если <paramref name="id"/> не задан, возвращает null
    /// Если задан, но сущность не найдена, бросает <see cref="NotFoundException"/>
    /// </remarks>
    /// <exception cref="NotFoundException">Сущность с указанным Id не найдена</exception>
    public static async Task<TEntity?> GetByIdAsync<TEntity>(this DbSet<TEntity> dbSet, Guid? id, bool canModify = false) where TEntity : Base
    {
        return !id.HasValue ? null : await dbSet.GetByIdAsync<TEntity>(id.Value, canModify);
    }

    /// <summary>
    /// Найти сущность по Id, и промаппить её
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="mappingConfiguration">Настройки маппинга</param>
    /// <param name="canModify">Флаг отслеживания сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <typeparam name="TResultType">Тип сущности в который маппим</typeparam>
    /// <returns>Сущность</returns>
    /// <exception cref="NotFoundException">Элемент с данным идентификатором не найден</exception>
    public static async Task<TResultType> GetByIdAsync<TEntity, TResultType>(this DbSet<TEntity> dbSet, Guid id, IConfigurationProvider mappingConfiguration, bool canModify = false) where TEntity : Base
    {
        var query = canModify 
            ? dbSet 
            : dbSet.AsNoTracking();

        var entity = await query.Where(e => e.Id==id).ProjectTo<TResultType>(mappingConfiguration).SingleOrDefaultAsync();

        if (entity == null)
            throw new NotFoundException(typeof(TEntity), id);

        return entity;
    }

    /// <summary>а
    /// Найти сущность по предикту
    /// </summary>
    /// <param name="predicate">Условие поиска сущности</param>
    /// <param name="canModify">Флаг отслеживания сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <returns>Сущность</returns>
    /// <exception cref="NotFoundException"></exception>
    public static async Task<TEntity> GetAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity,bool>> predicate, bool canModify = false) where TEntity : Base
    {
        var query = canModify 
            ? dbSet 
            : dbSet.AsNoTracking();

        var entity = await query.SingleOrDefaultAsync(predicate);

        if (entity == null)
            throw new NotFoundException(typeof(TEntity));

        return entity;
    }

    /// <summary>а
    /// Найти сущность по предикту, с маппингом
    /// </summary>
    /// <param name="predicate">Условие поиска сущности</param>
    /// <param name="canModify">Флаг отслеживания сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <typeparam name="TResultType">Тип сущности в который маппим</typeparam>
    /// <returns>Сущность</returns>
    /// <exception cref="NotFoundException"></exception>
    public static async Task<TResultType> GetAsync<TEntity, TResultType>(this DbSet<TEntity> dbSet, Expression<Func<TEntity,bool>> predicate, IConfigurationProvider mappingConfiguration, bool canModify = false) where TEntity : Base
    {
        var query = canModify 
            ? dbSet 
            : dbSet.AsNoTracking();

        var entity = await query.Where(predicate).ProjectTo<TResultType>(mappingConfiguration).SingleOrDefaultAsync();

        if (entity == null)
            throw new NotFoundException(typeof(TEntity));

        return entity;
    }
    
    /// <summary>
    /// Проверяет, существует ли сущность по Id 
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <exception cref="NotFoundException"></exception>
    public static async Task EnsureExistAsync<TEntity>(this DbSet<TEntity> dbSet, Guid id) where TEntity : Base
    {
        if (!await dbSet.AsNoTracking().AnyAsync(e => e.Id == id))
            throw new NotFoundException(typeof(TEntity), id);
    }
    
    /// <summary>
    /// Проверяет, существует ли сущность по Id (nullable).
    /// </summary>
    /// <param name="id">Идентификатор сущности (может быть null)</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <remarks>
    /// Если <paramref name="id"/> не задан, возвращает null
    /// Если задан, но сущность не найдена, бросает <see cref="NotFoundException"/>
    /// </remarks>
    /// <exception cref="NotFoundException">Сущность с указанным Id не найдена</exception>
    public static async Task EnsureExistAsync<TEntity>(this DbSet<TEntity> dbSet, Guid? id) where TEntity : Base
    {
        if(id == null)
            return;

        await dbSet.EnsureExistAsync(id.Value);
    }

    /// <summary>
    /// Проверяет, существует ли сущность по Id 
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="predicate">Условие поиска сущности</param>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <exception cref="NotFoundException"></exception>
    public static async Task EnsureExistAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity,bool>> predicate) where TEntity : Base
    {
        if (!await dbSet.AsNoTracking().AnyAsync(predicate))
            throw new NotFoundException(typeof(TEntity));
    }
    
    /// <summary>
    /// Проверяет, что сущность с таким Id не существует.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <param name="id">Идентификатор сущности</param>
    /// <exception cref="AlreadyExistsException">Если сущность уже существует</exception>
    public static async Task EnsureNotExistAsync<TEntity>(this DbSet<TEntity> dbSet, Guid id) where TEntity : Base
    {
        if (await dbSet.AsNoTracking().AnyAsync(e => e.Id == id))
            throw new AlreadyExistsException(typeof(TEntity), id);
    }

    /// <summary>
    /// Проверяет, что сущность с таким Id не существует (nullable).
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <param name="id">Идентификатор сущности (может быть null)</param>
    /// <exception cref="AlreadyExistsException">Если сущность уже существует</exception>
    public static async Task EnsureNotExistAsync<TEntity>(this DbSet<TEntity> dbSet, Guid? id) where TEntity : Base
    {
        if (id == null)
            return;

        await dbSet.EnsureNotExistAsync(id.Value);
    }

    /// <summary>
    /// Проверяет, что не существует сущности, удовлетворяющей условию.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности, производный от Base</typeparam>
    /// <param name="predicate">Условие для поиска сущности</param>
    /// <exception cref="AlreadyExistsException">Если сущность уже существует</exception>
    public static async Task EnsureNotExistAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate) where TEntity : Base
    {
        if (await dbSet.AsNoTracking().AnyAsync(predicate))
            throw new AlreadyExistsException(typeof(TEntity));
    }
}