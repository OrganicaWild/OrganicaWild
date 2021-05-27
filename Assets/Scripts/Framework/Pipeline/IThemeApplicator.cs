using System;
using System.Collections;
using UnityEngine;

namespace Framework.Pipeline
{
    public interface IThemeApplicator
    {
        IEnumerator ApplyTheme(GameWorld world);
    }
}