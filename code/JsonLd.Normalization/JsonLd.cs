using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonLd.Normalization
{
    public static class JsonLd
    {
        /// <summary>
        /// Implements URDNA2015 normalization/canonicalization algorithm based on jsonld.js v5.2.0 implementation, 
        /// the same as Normalize
        /// </summary>
        /// <param name="json">serialized json document</param>
        /// <returns>normalized n-quads document as string</returns>
        public static async Task<string> Canonize(string json)
        {
            return await Normalize(json);
        }

        /// <summary>
        /// Implements URDNA2015 normalization/canonicalization algorithm based on jsonld.js v5.2.0 implementation,
        /// the same as Canonize
        /// </summary>
        /// <param name="json">serialized json document</param>
        /// <returns>normalized n-quads document as string</returns>
        public static async Task<string> Normalize(string json)
        {
            var dataset = await ToRDF(json);
            return URDNA2015.Normalize(dataset);
        }

        private static async Task<List<Quad>> ToRDF(string json)
        {
            var expanded = await Expand(json);
            return expanded.ToRDF();
        }

        private static async Task<JToken> Expand(string json)
        {
            var activeCtx = new ExpandContext();
            var token = JToken.Parse(json);
            var objects = new List<JObject>();
            if (token.Type == JTokenType.Array)
                objects.AddRange(token.ToArray().Where(t => t.Type == JTokenType.Object).Cast<JObject>());
            if (token.Type == JTokenType.Object)
                objects.Add((JObject)token);
            var result = new JArray();
            foreach (var doc in objects)
            {
                var options = new Dictionary<string, object>();
                options["contextResolver"] = new ContextResolver();

                var expanded = await Expansion.Expand(activeCtx, doc, null, options);

                // optimize away @graph with no other properties
                if (expanded?.Type == JTokenType.Object)
                {
                    var expandedObj = (JObject)expanded;
                    if (expandedObj.TryGetValue("@graph", out var graphProp) && expandedObj.Properties().Count() == 1)
                        expanded = graphProp;
                }

                if (expanded != null)
                    result.Add(expanded);
            }

            return result;
        }
    }
}
