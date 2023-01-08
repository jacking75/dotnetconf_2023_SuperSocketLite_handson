using System.Text;

using MemoryPack;
using MessagePack;



//namespace PacketReadWrite;

public class BinaryUserData
{
    public UInt64 UId = 0;
    public string NickName = "";

    public byte[] Seserialize()
    {
        List<byte> dataSource = new List<byte>();
        dataSource.AddRange(BitConverter.GetBytes(UId));
        dataSource.AddRange(Encoding.UTF8.GetBytes(NickName));
        return dataSource.ToArray();
    }

    public void Deserialize(byte[] data)
    {
        UId = BitConverter.ToUInt64(data, 0);
        NickName = Encoding.UTF8.GetString(data, 8, data.Length - 8);
    }

    public void ToConsole()
    {
        Console.WriteLine($"UId: {UId}, NickName: {NickName}");
    }
}


[MessagePackObject]
public class MessagePackUserData
{
    [Key(0)]
    public UInt64 UId = 0;
    [Key(1)]
    public string NickName = "";

    public void ToConsole()
    {
        Console.WriteLine($"UId: {UId}, NickName: {NickName}");
    }
}



[MemoryPackable]
public partial class MemoryPackUserData
{
    public UInt64 UId = 0;
    public string NickName = "";

    public void ToConsole()
    {
        Console.WriteLine($"UId: {UId}, NickName: {NickName}");
    }
}
