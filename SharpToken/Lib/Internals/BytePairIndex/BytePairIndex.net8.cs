#if NET8_0_OR_GREATER
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
    private readonly List<KeyValuePair<byte[], int>> _values;

    /// <summary>
    /// NOTE: does not support empty byte array keys!
    /// </summary>
    public BytePairIndex(IEnumerable<KeyValuePair<byte[], int>> data)
    {
        _values = data.ToList();

        var result = Run(0, _values);

        _rootNode = MakeNode(result);

        static Node MakeNode(InnerNode[] list)
        {
            var node = new Node(
                SearchIndex: list.Select(_ => _.Key).ToArray(),
                Children: list.Select(_ => MakeNode(_.Nodes)).ToArray(),
                Values: list.Select(_ => _.Value).ToArray()
            );

            return node;
        }

        static InnerNode[] Run(int i, IEnumerable<KeyValuePair<byte[], int>> values)
        {
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


    public bool ContainsKey(ReadOnlySpan<byte> key)
    {
        return TryGetValue(key, out _);
    }


    public bool TryGetValue(ReadOnlySpan<byte> key, out int value)
    {
        var node = _rootNode;

        var i = 0;
        var len = key.Length;
        while (i < len)
        {
            var b = key[i];

            // indexOf uses modern multibyte cpu instructions
            var index = node.SearchIndex.Span.IndexOf(b);
            if (index == -1)
            {
                value = default;
                return false;
            }

            if (i == len - 1)
            {
                var found = node.Values[index];
                value = found ?? default;
                return found is not null;
            }

            node = node.Children[index];

            i++;
        }

        value = default;
        return false;
    }


    public int this[ReadOnlySpan<byte> key] => TryGetValue(key, out var value) ? value : throw new Exception($"Key: [{string.Join(",", key.ToArray())}] was not present in Index.");


    private sealed record Node(
        ReadOnlyMemory<byte> SearchIndex,
        Node[] Children,
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
