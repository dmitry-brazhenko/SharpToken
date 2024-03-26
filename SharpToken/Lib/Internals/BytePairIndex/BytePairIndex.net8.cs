#if NET8_0_OR_GREATER
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace SharpToken;

/// <summary>
/// Provides a fast byte[] lookup using multibyte cpu instructions introduced in net8.0
/// Supports <see cref="ReadOnlySpan{T}"/> as lookup key.
/// NOTE: it takes long to initialize but provides faster reads then a Dictionary.
/// </summary>
internal sealed class BytePairIndex : IReadOnlyCollection<KeyValuePair<byte[], int>>
{
    private readonly Node _rootNode;
    private readonly IDictionary<byte[], int> _values;

    public int MaxKeyLength { get; private set; }

    /// <summary>
    /// NOTE: does not support empty byte array keys!
    /// Keys must be unique! This implementation does not validate input!
    /// Input validation is done in Dictionary before instantiating this.
    /// </summary>
    public BytePairIndex(IDictionary<byte[], int> data)
    {
        _values = data;

        var result = Run(0, _values);

        _rootNode = MakeNode(result) ?? throw new ArgumentException("Parameter resulted in an empty collection!", nameof(data));

        static Node? MakeNode(InnerNode[] list)
        {
            if (list.Length == 0)
            {
                return null;
            }

            var node = new Node(
                SearchIndex: list.Select(_ => _.Key).ToArray(),
                Children: list.Select(_ => MakeNode(_.Nodes)).ToArray(),
                Values: list.Select(_ => _.Value).ToArray()
            );

            return node;
        }

        InnerNode[] Run(int i, IEnumerable<KeyValuePair<byte[], int>> values)
        {
            if (i > MaxKeyLength)
            {
                MaxKeyLength = i;
            }

            var nodes = values
                .Select(_ => (Byte: _.Key[i], Value: _))
                .GroupBy(_ => _.Byte)
                .Select(g =>
                {
                    var nextIndex = i + 1;

                    var values = g.Select(_ => _.Value).ToList();
                    var value = values.Where(_ => _.Key.Length == nextIndex).Select(_ => (int?) _.Value).SingleOrDefault();
                    var nextValues = values.Where(_ => _.Key.Length > nextIndex);
                    return new InnerNode(
                        Key: g.Key,
                        Value: value,
                        Nodes: Run(nextIndex, nextValues)
                    );
                })
                .ToArray();

            return nodes;
        }
    }


    public bool TryGetValue(ReadOnlySpan<byte> key, out int value)
    {
        var node = _rootNode;

        var i = 0;
        var len = key.Length - 1;
        while (i <= len)
        {
            var b = key[i];

            // indexOf uses modern multibyte cpu instructions
            var index = node.SearchIndex.Span.IndexOf(b);
            if (index == -1)
            {
                value = default;
                return false;
            }

            if (i == len)
            {
                var found = node.Values[index];
                value = found ?? default;
                return found is not null;
            }

            node = node.Children[index];
            if (node is null)
            {
                break;
            }

            i++;
        }

        value = default;
        return false;
    }


    public int this[ReadOnlySpan<byte> key] => TryGetValue(key, out var value) ? value : throw new Exception($"Key: [{string.Join(",", key.ToArray())}] was not present in Index.");


    private sealed record Node(
        ReadOnlyMemory<byte> SearchIndex,
        Node?[] Children,
        int?[] Values
    );

    private sealed record InnerNode(
        byte Key,
        int? Value,
        InnerNode[] Nodes
    );

    public IEnumerator<KeyValuePair<byte[], int>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _values.Count;
}

#endif
