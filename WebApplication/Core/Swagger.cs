using System;
using System.Collections.Generic;
using Fotron.WebApplication.Core.Response;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fotron.WebApplication.Core {

	public static class Swagger {

		public static string TagSelector(ApiDescription api) {
			return api.RelativePath.Substring(4, api.RelativePath.LastIndexOf('/') - 4);
		}

		public class JWTHeaderParameter : IOperationFilter {

			public void Apply(Operation operation, OperationFilterContext context) {
				if (operation.Parameters == null) {
					operation.Parameters = new List<IParameter>();
				}

				operation.Parameters.Add(new NonBodyParameter {
					Name = "Authorization",
					In = "header",
					Type = "string",
					Default = "Bearer ",
					Required = false,
				});
			}
		}

		public class DefaultErrorResponse : IOperationFilter {

			public void Apply(Operation operation, OperationFilterContext context) {
				if (operation.Responses == null) {
					operation.Responses = new Dictionary<string, Swashbuckle.AspNetCore.Swagger.Response>();
				}

				operation.Responses.Add("!200", new Swashbuckle.AspNetCore.Swagger.Response() {
					Description = "Common failure response",
					Schema = context.SchemaRegistry.GetOrRegister(typeof(APIResponse.Response)),
				});
			}
		}

		public class EnumDescription : IDocumentFilter {

			private void DescribeEnumParameters(IList<IParameter> parameters) {
				if (parameters != null) {
					foreach (var param in parameters) {
						var bparam = param as BodyParameter;
						if (bparam != null) {
							IList<object> paramEnums = bparam.Schema.Enum;
							if (paramEnums != null && paramEnums.Count > 0) {
								param.Description += DescribeEnum(paramEnums);
							}
						}
					}
				}
			}

			private string DescribeEnum(IList<object> enums) {
				List<string> enumDescriptions = new List<string>();
				foreach (object enumOption in enums) {
					enumDescriptions.Add(string.Format("{0} = {1}", (int)enumOption, Enum.GetName(enumOption.GetType(), enumOption)));
				}
				return string.Join(",\n ", enumDescriptions.ToArray());
			}

			public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {

				// add enum descriptions to result models
				foreach (KeyValuePair<string, Schema> schemaDictionaryItem in swaggerDoc.Definitions) {

					Schema schema = schemaDictionaryItem.Value;

					foreach (KeyValuePair<string, Schema> propertyDictionaryItem in schema.Properties) {
						Schema property = propertyDictionaryItem.Value;
						IList<object> propertyEnums = property.Enum;
						if (propertyEnums != null && propertyEnums.Count > 0) {
							property.Description += DescribeEnum(propertyEnums);
						}
					}
				}
			}
		}
	}
}
