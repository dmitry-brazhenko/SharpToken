using System;
using System.Collections.Generic;

namespace SharpToken
{
    public static class Model
    {
        private static readonly Dictionary<string, string> ModelToEncodingMapping = new Dictionary<string, string>
        {
            // chat
            { "gpt-4o", "o200k_base" },
            { "gpt-5", "o200k_base" },
            { "gpt-5-mini", "o200k_base" },
            { "gpt-5-nano", "o200k_base" },
            { "gpt-5-pro", "o200k_base" },
            { "gpt-5-thinking", "o200k_base" },
            { "gpt-4", "cl100k_base" },
            { "gpt-3.5-turbo-16k", "cl100k_base" },
            { "gpt-35-turbo-16k", "cl100k_base" }, // Azure deployment name
            { "gpt-3.5-turbo", "cl100k_base" },
            { "gpt-35-turbo", "cl100k_base" }, // Azure deployment name
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
            { "text-embedding-3-large", "cl100k_base" },
            { "text-embedding-3-small", "cl100k_base" },
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

        private static readonly Dictionary<string, string> ModelPrefixToEncodingMapping = new Dictionary<string, string>
        {
            { "gpt-5", "o200k_base" }, // e.g., gpt-5-2024-08-07, gpt-5-chat-latest, etc.
            { "gpt-4o", "o200k_base" }, // (NOTE: no trailing dash, on purpose). E.g., gpt-4o, gpt-4o-2024-05-13, etc.,
            { "gpt-4-", "cl100k_base" }, // e.g., gpt-4-0314, etc., plus gpt-4-32k
            { "gpt-3.5-turbo-", "cl100k_base" }, // e.g, gpt-3.5-turbo-0301, -0401, etc.
            { "gpt-35-turbo", "cl100k_base" }, // Azure deployment name
        };

        public static string GetEncodingNameForModel(string modelName)
        {
            if (ModelToEncodingMapping.TryGetValue(modelName, out var encodingName))
            {
                return encodingName;
            }
            else
            {
                foreach (var prefix in ModelPrefixToEncodingMapping.Keys)
                {
                    if (modelName.StartsWith(prefix))
                    {
                        return ModelPrefixToEncodingMapping[prefix];
                    }
                }
            }

            throw new Exception(
                $"Could not automatically map {modelName} to a tokenizer. " +
                $"Please use {nameof(GptEncoding.GetEncoding)} to explicitly get the tokenizer you expect.");
        }
    }
}
