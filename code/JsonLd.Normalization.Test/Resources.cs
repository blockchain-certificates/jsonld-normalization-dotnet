using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace JsonLd.Normalization.Test
{
	public enum EmbeddedResourceName
	{
		[Description("blockcerts_v3.json")]
		BLOCKCERTS_V3,
		[Description("normalized.blockcerts_v3.nq")]
		BLOCKCERTS_V3_NQUADS, //this file was generated with jsonld.js library
		[Description("blockcerts_v3_alt.json")]
		BLOCKCERTS_V3_ALT,
        [Description("proof-chain-exampleE2.json")]
        PROOF_CHAIN_EXAMPLE,
	}

	public static class Resources
	{
		public static Dictionary<string, string> ResourceStrings { get; } = new Dictionary<string, string>();

		static Resources()
		{
			string ROOT_NAMESPACE = $"{typeof(Resources).Namespace}.Resources.";

			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			foreach (string name in thisAssembly.GetManifestResourceNames())
			{
				using Stream stream = thisAssembly.GetManifestResourceStream(name);
				using StreamReader reader = new(stream);
				ResourceStrings.Add(name.Remove(0, ROOT_NAMESPACE.Length), reader.ReadToEnd());
			}
		}

		public static string Get(EmbeddedResourceName resourceName)
		{
			FieldInfo info = resourceName.GetType().GetField(resourceName.ToString());
			return ResourceStrings[info.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty];
		}

		public static string Get(string resourceName)
		{
			return ResourceStrings[resourceName];
		}
	}
}
