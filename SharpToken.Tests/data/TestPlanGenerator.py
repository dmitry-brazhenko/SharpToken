import tiktoken


def read_test_samples(filename):
    with open(filename, 'r') as f:
        test_samples = f.read().splitlines()
    return test_samples


def get_special_token_samples(encoding):
    """Generate special token test samples for the given encoding"""
    special_token_samples = []
    
    # Get all special tokens for this encoding
    special_tokens = list(encoding._special_tokens.keys())
    
    if not special_tokens:
        return special_token_samples
    
    # Add individual special tokens
    for token in special_tokens[:10]:  # Limit to first 10 to avoid too many tests
        special_token_samples.append(token)
    
    # Add special tokens with text
    if "<|endoftext|>" in special_tokens:
        special_token_samples.extend([
            "Hello <|endoftext|> World",
            "<|endoftext|>This is a test<|endoftext|>"
        ])
    
    # Add fill-in-the-middle combinations for encodings that support it
    if all(token in special_tokens for token in ["<|fim_prefix|>", "<|fim_middle|>", "<|fim_suffix|>"]):
        special_token_samples.extend([
            "<|fim_prefix|>def hello():<|fim_suffix|>    print('world')<|fim_middle|>",
            "<|fim_prefix|>function test() {<|fim_suffix|>}<|fim_middle|>    return true;"
        ])
    
    # Add endofprompt combinations
    if "<|endofprompt|>" in special_tokens:
        special_token_samples.extend([
            "User: Hello<|endofprompt|>Assistant: Hi there!",
            "Question<|endofprompt|>Answer"
        ])
    
    # Add o200k_harmony specific combinations
    if "<|startoftext|>" in special_tokens and "<|call|>" in special_tokens:
        special_token_samples.extend([
            "<|startoftext|>Hello World<|endoftext|>",
            "<|call|>function_name<|return|>result",
            "<|message|>user<|constrain|>safe<|channel|>text",
            "<|start|>conversation<|message|>content<|end|>"
        ])
    
    # Add some reserved tokens for o200k_harmony
    reserved_tokens = [token for token in special_tokens if token.startswith("<|reserved_")]
    if reserved_tokens:
        special_token_samples.extend([
            reserved_tokens[0],  # First reserved token
            f"Text with {reserved_tokens[0]} reserved token" if len(reserved_tokens) > 0 else ""
        ])
    
    return special_token_samples


def generate_test_plans(test_samples, encodings):
    test_plans = []

    for encoding in encodings:
        # Process regular test samples (disallow special tokens)
        for sample in test_samples:
            if sample.strip():  # Skip empty lines
                encoded = encoding.encode(sample, allowed_special=set())
                test_plans.append((encoding.name, sample, encoded))
        
        # Process special token samples (allow special tokens)
        special_samples = get_special_token_samples(encoding)
        for sample in special_samples:
            if sample.strip():  # Skip empty samples
                try:
                    # Allow all special tokens for this encoding
                    all_special_tokens = set(encoding._special_tokens.keys())
                    encoded = encoding.encode(sample, allowed_special=all_special_tokens)
                    test_plans.append((encoding.name, sample, encoded))
                except Exception as e:
                    # Skip samples that cause encoding errors
                    print(f"Skipping sample '{sample}' for {encoding.name}: {e}")
                    continue

    return test_plans


def save_test_plans(test_plans, filename):
    with open(filename, 'w') as f:
        for name, sample, encoded in test_plans:
            f.write(f"EncodingName: {name}\n")
            f.write(f"Sample: {sample}\n")
            f.write(f"Encoded: {encoded}\n\n")


if __name__ == "__main__":
    samples_filename = 'GptEncoderTestSamples.txt'
    test_plans_filename = 'TestPlans.txt'

    encodings = [
        tiktoken.get_encoding("r50k_base"),
        tiktoken.get_encoding("p50k_base"),
        tiktoken.get_encoding("p50k_edit"),
        tiktoken.get_encoding("cl100k_base"),
        tiktoken.get_encoding("o200k_base"),
        tiktoken.get_encoding("o200k_harmony"),
    ]

    test_samples = read_test_samples(samples_filename)
    test_plans = generate_test_plans(test_samples, encodings)
    save_test_plans(test_plans, test_plans_filename)
