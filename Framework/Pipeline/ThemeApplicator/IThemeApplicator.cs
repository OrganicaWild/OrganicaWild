using System.Collections;

namespace Framework.Pipeline.ThemeApplicator
{
    public interface IThemeApplicator
    {
        /// <summary>
        /// Applies a the theme the implemented ThemeApplicator specifies.
        /// The method returns an IEnumerator, so that it can be implemented as a Coroutine.
        /// This allows the creation of the GameWorld to stretch across several frames and therefore not blocking the rest of the engine.
        /// </summary>
        /// <param name="world">GameWorld to apply theme on</param>
        /// <returns>Empty Iterator</returns>
        IEnumerator ApplyTheme(GameWorld world);
    }
}