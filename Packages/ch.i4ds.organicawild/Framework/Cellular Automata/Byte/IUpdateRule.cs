namespace Framework.Cellular_Automata.Byte
{
    public interface IByteUpdateRule
    {
        byte ApplyTo(ByteCell cell);
    }
}