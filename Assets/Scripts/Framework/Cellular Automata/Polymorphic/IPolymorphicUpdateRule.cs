namespace Framework.Cellular_Automata.Polymorphic
{
    public interface IPolymorphicUpdateRule
    {
        PolymorphicCellState ApplyTo(PolymorphicCell cell);
    }
}