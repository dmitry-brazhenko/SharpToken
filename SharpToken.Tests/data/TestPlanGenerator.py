import tiktoken


def read_test_samples(filename):
    with open(filename, 'r') as f:
        test_samples = f.read().splitlines()
    return test_samples


def generate_test_plans(test_samples, encodings):
    test_plans = []

    for encoding in encodings:
        for sample in test_samples:
            encoded = encoding.encode(sample, allowed_special={""})
            test_plans.append((encoding.name, sample, encoded))

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
    ]

    test_samples = read_test_samples(samples_filename)
    test_plans = generate_test_plans(test_samples, encodings)
    save_test_plans(test_plans, test_plans_filename)
