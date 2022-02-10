using System.Collections;

namespace Lillisp.Core;

public class Bytevector : Node, IList<byte>
{
    private readonly List<byte> _bytes;

    public Bytevector()
    {
        _bytes = new List<byte>();
    }

    public Bytevector(IEnumerable<byte> bytes)
    {
        _bytes = bytes.ToList();
    }

    public IEnumerator<byte> GetEnumerator() => _bytes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _bytes.GetEnumerator();

    public void Add(byte item) => _bytes.Add(item);

    public void Clear() => _bytes.Clear();

    public bool Contains(byte item) => _bytes.Contains(item);

    public void CopyTo(byte[] array, int arrayIndex) => _bytes.CopyTo(array, arrayIndex);

    public bool Remove(byte item) => _bytes.Remove(item);

    public int Count => _bytes.Count;

    public bool IsReadOnly => false;

    public int IndexOf(byte item) => _bytes.IndexOf(item);

    public void Insert(int index, byte item) => _bytes.Insert(index, item);

    public void RemoveAt(int index) => _bytes.RemoveAt(index);

    public byte this[int index]
    {
        get => _bytes[index];
        set => _bytes[index] = value;
    }

    public override string ToString() => $"#u8({string.Join(' ', _bytes)})";

    public Bytevector Slice(int start, int length)
    {
        var slice = new byte[length];
        _bytes.CopyTo(start, slice, 0, length);
        return new Bytevector(slice);
    }

    public byte[] ToByteArray() => _bytes.ToArray();
}