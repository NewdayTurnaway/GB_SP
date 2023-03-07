using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolarSystemGame
{
    public class ShowObjectLabels : MonoBehaviour
    {
        private const int FONT_SIZE = 24;

        private readonly GUIStyle _style = new();

        private void Start()
        {
            _style.normal.background = new Texture2D(1, 1);
            _style.normal.textColor = Color.blue;
            _style.fontSize = FONT_SIZE;
            _style.alignment = TextAnchor.MiddleCenter;
        }

        public void DrawLabel(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            var objects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                var position = camera.WorldToScreenPoint(obj.transform.position);

                if (!obj.TryGetComponent<SpaceshipNetworkController>(out var shipController))
                {
                    continue;
                }

                if (!obj.TryGetComponent<Collider>(out var collider))
                {
                    continue;
                }

                var isVisible = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), collider.bounds);

                if (collider != null && isVisible)
                {
                    var name = shipController.PlayerData.Value.Name;
                    var width = name.Length * FONT_SIZE / 2 + 20;
                    var y = Screen.height - position.y - 100;
                    GUI.Label(new Rect(position.x - width / 2, y, width, FONT_SIZE + 5), name, _style);
                }
            }
        }
    } 
}
