using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PlayerAndCameraRecipe", menuName = "Pipeline/PlayerAndCameraRecipe", order = 0)]
    public class PlayerAndCameraRecipe : GameWorldObjectRecipe
    {

        public GameObject cameraAndPlayerRig;
        public Vector3 basePosition;
        
        public override GameObject Cook(IGameWorldObject individual)
        {
            if (individual.Shape is OwPoint pos2d)
            {
                //move camera to position;
                Vector3 pos3d = new Vector3(pos2d.Position.x, 0, pos2d.Position.y);
                GameObject instantiated = Instantiate(cameraAndPlayerRig, basePosition + pos3d, Quaternion.identity);
                return instantiated;
            }

            return new GameObject();
        }
    }
}
