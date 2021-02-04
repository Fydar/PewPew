using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Shared.Model
{
	public class PackagedModel<TModel>
		where TModel : class
	{
		private static readonly Dictionary<string, Type> typeLookup;

		static PackagedModel()
		{
			typeLookup = new Dictionary<string, Type>();
			var modelType = typeof(TModel);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				bool includeAssembly = false;
				if (assembly == modelType.Assembly)
				{
					includeAssembly = true;
				}
				else
				{
					var referencedAssemblies = assembly.GetReferencedAssemblies();

					foreach (var referencedAssembly in referencedAssemblies)
					{
						if (referencedAssembly.FullName == modelType.Assembly.FullName)
						{
							includeAssembly = true;
							break;
						}
					}
				}

				if (includeAssembly)
				{
					Type[] searchTypes;
					try
					{
						searchTypes = assembly.GetExportedTypes();
					}
					catch
					{
						continue;
					}

					foreach (var searchType in searchTypes)
					{
						if (modelType.IsAssignableFrom(searchType))
						{
							typeLookup.Add(searchType.FullName, searchType);
						}
					}
				}
			}
		}

		public string Type { get; set; }
		public JObject Data { get; set; }

		public TModel Deserialize()
		{
			var type = typeLookup[Type];
			return (TModel)Data.ToObject(type);
		}

		public static PackagedModel<TModel> Serialize(TModel model)
		{
			return new PackagedModel<TModel>()
			{
				Type = model.GetType().FullName,
				Data = JObject.FromObject(model)
			};
		}
	}
}
