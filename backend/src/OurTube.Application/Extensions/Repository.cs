using System.Linq.Expressions;
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
    /// Найти сущность по Id
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
}