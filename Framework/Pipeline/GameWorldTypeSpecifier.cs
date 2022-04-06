using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Standard;

namespace Framework.Pipeline
{
    public class GameWorldTypeSpecifier
    {
        public static readonly GameWorldTypeSpecifier OneArea =
            new GameWorldTypeSpecifier(typeof(Area), AmountSpecifier.One);

        public static readonly GameWorldTypeSpecifier ManyAreas =
            new GameWorldTypeSpecifier(typeof(Area), AmountSpecifier.Many);

        public static readonly GameWorldTypeSpecifier OneMainPath =
            new GameWorldTypeSpecifier(typeof(MainPath), AmountSpecifier.One);

        public static readonly GameWorldTypeSpecifier ManyMainPaths =
            new GameWorldTypeSpecifier(typeof(MainPath), AmountSpecifier.Many);

        public static readonly GameWorldTypeSpecifier OneLandmark =
            new GameWorldTypeSpecifier(typeof(Landmark), AmountSpecifier.One);

        public static readonly GameWorldTypeSpecifier ManyLandmarks =
            new GameWorldTypeSpecifier(typeof(Landmark), AmountSpecifier.Many);

        public static readonly GameWorldTypeSpecifier OneAreaConnection =
            new GameWorldTypeSpecifier(typeof(AreaConnection), AmountSpecifier.One);

        public static readonly GameWorldTypeSpecifier ManyAreaConnections =
            new GameWorldTypeSpecifier(typeof(AreaConnection), AmountSpecifier.Many);

        public readonly AmountSpecifier amountSpecifier;
        public readonly Type iGameWorldObjectType;
        public IGameWorldObject InjectedInstance { get; set; }

        public GameWorldTypeSpecifier(Type gameWorldObjectType, AmountSpecifier specifier)
        {
            //check if passed type is actually an IGameWorldObject (either the interface or inherited)
            // if (typeof(IGameWorldObject) == gameWorldObjectType ||
            //     typeof(IGameWorldObject).IsAssignableFrom(gameWorldObjectType))
            // {
            //     throw new ArgumentException("Type of gameWorldObjectType must inherit from IGameWorldObject");
            // }

            iGameWorldObjectType = gameWorldObjectType;
            amountSpecifier = specifier;
        }
    }
}