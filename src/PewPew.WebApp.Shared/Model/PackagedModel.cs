using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PewPew.WebApp.Shared.Model
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
							typeLookup.Add(searchType.Name, searchType);
						}
					}
				}
			}
		}

		public string Type { get; set; }
		public JObject Data { get; set; }

		public PackagedModel(string type, JObject data)
		{
			Type = type;
			Data = data;
		}

		public TModel Deserialize()
		{
			var type = typeLookup[Type];
			return Data.ToObject(type) is TModel result
				? result
				: throw new InvalidOperationException($"Failed to deserialize {Data} to type '{Type}'");
		}

		public static PackagedModel<TModel> Serialize(TModel model)
		{
			return new PackagedModel<TModel>(model.GetType().Name, JObject.FromObject(model));
		}
	}
}
