using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fotron.Common.Extensions {

	public static class DalExtensions {

		public class Page<T> where T:class {

			public T[] Selected { get; set; }
			public long TotalCount { get; set; }
		}

		public static async Task<Page<T>> PagerAsync<T, TSortField>(this IQueryable<T> query, long offset, long limit, System.Linq.Expressions.Expression<Func<T, TSortField>> sortBy = null, bool ascending = false) where T:class {

			var intOffset = Math.Max(0, (int)offset);
			var intLimit = Math.Max(1, (int)limit);

			var count = await query.LongCountAsync();

			if (sortBy != null) {
				query = ascending
					? query.OrderBy(sortBy)
					: query.OrderByDescending(sortBy);
				;
			}

			var rows = await query
				.Skip(intOffset)
				.Take(intLimit)
				.AsNoTracking()
				.ToArrayAsync()
			;

			return new Page<T>() {
				Selected = rows,
				TotalCount = count,
			};
		}

	}
}
