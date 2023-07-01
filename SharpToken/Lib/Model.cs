using System;
using System.Collections.Generic;

namespace SharpToken.Lib
{
    public static class Model
    {
        private static readonly Dictionary<string, string> ModelToEncodingMapping = new Dictionary<string, string>
        {
            // chat
            { "gpt-4", "cl100k_base" },
            { "gpt-3.5-turbo", "cl100k_base" },
            // text
            { "text-davinci-003", "p50k_base" },
            { "text-davinci-002", "p50k_base" },
            { "text-davinci-001", "r50k_base" },
            { "text-curie-001", "r50k_base" },
            { "text-babbage-001", "r50k_base" },
            { "text-ada-001", "r50k_base" },
            { "davinci", "r50k_base" },
            { "curie", "r50k_base" },
            { "babbage", "r50k_base" },
            { "ada", "r50k_base" },
            // code
            { "code-davinci-002", "p50k_base" },
            { "code-davinci-001", "p50k_base" },
            { "code-cushman-002", "p50k_base" },
            { "code-cushman-001", "p50k_base" },
            { "davinci-codex", "p50k_base" },
            { "cushman-codex", "p50k_base" },
            // edit
            { "text-davinci-edit-001", "p50k_edit" },
            { "code-davinci-edit-001", "p50k_edit" },
            // embeddings
            { "text-embedding-ada-002", "cl100k_base" },
            // old embeddings
            { "text-similarity-davinci-001", "r50k_base" },
            { "text-similarity-curie-001", "r50k_base" },
            { "text-similarity-babbage-001", "r50k_base" },
            { "text-similarity-ada-001", "r50k_base" },
            { "text-search-davinci-doc-001", "r50k_base" },
            { "text-search-curie-doc-001", "r50k_base" },
            { "text-search-babbage-doc-001", "r50k_base" },
            { "text-search-ada-doc-001", "r50k_base" },
            { "code-search-babbage-code-001", "r50k_base" },
            { "code-search-ada-code-001", "r50k_base" }
        };

        public static string GetEncodingNameForModel(string modelName)
        {
            if (ModelToEncodingMapping.TryGetValue(modelName, out var encodingName))
            {
                return encodingName;
            }

            throw new Exception(
                $"Could not automatically map {modelName} to a tokenizer. " +
                $"Please use {nameof(GptEncoding.GetEncoding)} to explicitly get the tokenizer you expect.");
        }
    }
}
