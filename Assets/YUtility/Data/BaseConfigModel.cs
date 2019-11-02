/// <summary>
/// 配置类基础
/// </summary>
public class BaseConfigModel
{
    ByteArray ba;

    public virtual void Read(ByteArray b)
    {
        ba = b;
    }

    public int ReadInt()
    {
        return ba.ReadInt32();
    }

    public string ReadString()
    {
        return ba.ReadString();
    }

    public bool ReadBool()
    {
        return ba.ReadBool();
    }

    public long ReadLong()
    {
        return ba.ReadInt64();
    }
}

