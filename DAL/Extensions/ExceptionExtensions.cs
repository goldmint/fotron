using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fotron.DAL.Extensions {

	public static class ExceptionExtension {

		public static bool IsMySqlDuplicateException(this Exception e) {
			return
				e is DbUpdateException &&
				e.InnerException is MySql.Data.MySqlClient.MySqlException &&
				(e.InnerException as MySql.Data.MySqlClient.MySqlException).Number == 1062
			;
		}
	}
}
