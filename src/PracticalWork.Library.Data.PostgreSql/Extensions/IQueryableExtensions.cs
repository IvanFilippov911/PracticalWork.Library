using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Data.PostgreSql.Extensions
{
    /// <summary>
    /// Методы расширений для IQueryable
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Применить пагинацию
        /// </summary>
        /// <typeparam name="T">Тип IQueryable</typeparam>
        /// <param name="queryable">IQueryable</param>
        /// <param name="pagination">Пагинация</param>
        /// <returns>IQueryable с пагинацией</returns>
        public static IQueryable<T> SkipTake<T>(this IQueryable<T> queryable, PageRequest pagination)
        {
            ArgumentNullException.ThrowIfNull(queryable);

            if (pagination is not { PageNumber: > 0 } || pagination.PageSize <= 0)
                return queryable;

            return queryable
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);
        }
    }
}
