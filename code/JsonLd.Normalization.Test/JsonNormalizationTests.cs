using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonLd.Normalization.Test
{
    [TestFixture]
    public class JsonNormalizationTests
    {

        [Test]
        public async Task DocumentHasher_JsonNormalization_Should_Return_Proper_Hash_If_Properties_Are_Changed()
        {
            var blockcerts_v3_1 = Resources.Get(EmbeddedResourceName.BLOCKCERTS_V3);
            var blockcerts_v3_2 = Resources.Get(EmbeddedResourceName.BLOCKCERTS_V3_ALT);
         
            var result1 = await JsonLd.Normalize(blockcerts_v3_1);
            var result2 = await JsonLd.Normalize(blockcerts_v3_2);

            Assert.NotNull(result1, "Result1 should not be null");
            Assert.NotNull(result2, "Result2 should not be null");
            Assert.AreEqual(result2, result1);
        }

        [Test]
        public async Task DocumentHasher_Blockcerts_v3_Success()
        {
            var blockcerts_v3 = Resources.Get(EmbeddedResourceName.BLOCKCERTS_V3);
            var result = await JsonLd.Normalize(blockcerts_v3);
            Assert.NotNull(result);
            Assert.True(result.Length > 0);

            Assert.AreEqual(Resources.Get(EmbeddedResourceName.BLOCKCERTS_V3_NQUADS), result);
        }

        [Test]
        public async Task JsonCanonicalizer_JsonLdJs_Comparison()
        {   //result documents have been produced with "code/jsonld.js-normalizer/normalize_documents.js" Node.js program
            //powershell: node code/jsonld.js-normalizer/normalize_documents.js code/JsonLd.Normalization.Test/Resources
            var sourceFilesPathBase = "JsonLd.Normalization.Test.Resources";
            var resultFilesPathBase = $"{sourceFilesPathBase}.normalized";
            var testFilesDict = new Dictionary<string, string>();
            foreach (var resName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (resName.StartsWith(resultFilesPathBase) && resName.EndsWith("-out.nq"))
                {
                    var fileName = resName.Substring(resultFilesPathBase.Length + 1); //+1 for (back)slash separator
                    var inputResName = $"{sourceFilesPathBase}{resName[resultFilesPathBase.Length]}{fileName.Replace("-out.nq", "-in.jsonld")}";
                    testFilesDict[inputResName] = resName;
                }
            }
            //var inclusive = new string[] { };
            //var excluded = new string[] { };
            foreach (var kvp in testFilesDict)
            {
                //if ((inclusive.Any() && !inclusive.Contains(kvp.Key)) || excluded.Contains(kvp.Key))
                //    continue; //in case a specific input needs to be debugged
                var inputDocument = ReadAllResource(kvp.Key);
                var expectedDocument = ReadAllResource(kvp.Value);
                string normalized = null;
                try
                {
                    normalized = await JsonLd.Normalize(inputDocument);
                }
                catch (Exception e)
                {
                    Assert.Fail($"Exception during normalizing {kvp.Key} document: {e}");
                }
                Assert.AreEqual(expectedDocument, normalized, kvp.Key);
            }
            Assert.Pass($"Successfully tested {testFilesDict.Count} examples");
        }

        private string ReadAllResource(string resourceName)
        {
            using var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(resStream);
            return reader.ReadToEnd();
        }
    }
}
