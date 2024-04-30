using UnityEngine;

namespace com.hive.projectr
{
    public class WallsResizer : MonoBehaviour
    {
        public GameObject topWall;
        public GameObject bottomWall;
        public GameObject leftWall;
        public GameObject rightWall;

        void Start()
        {
            ResizeWalls();
        }

        void ResizeWalls()
        {
            var cam = CameraManager.Instance.UICamera;
            Vector2 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));

            float width = screenBounds.x * 2;
            float height = screenBounds.y * 2;

            ResizeWall(topWall, width, 1, new Vector2(0, screenBounds.y));
            ResizeWall(bottomWall, width, 1, new Vector2(0, -screenBounds.y));
            ResizeWall(leftWall, height, 1, new Vector2(-screenBounds.x, 0));
            ResizeWall(rightWall, height, 1, new Vector2(screenBounds.x, 0));
        }

        void ResizeWall(GameObject wall, float length, float thickness, Vector2 position)
        {
            SpriteRenderer wallSpriteRenderer = wall.GetComponentInChildren<SpriteRenderer>();
            Vector2 wallSpriteSize = wallSpriteRenderer.sprite.bounds.size;
            Vector3 wallLocalScale = wall.transform.localScale;

            float scaleFactorLength = length / wallSpriteSize.x;
            float scaleFactorThickness = thickness / wallSpriteSize.y;

            wall.transform.localScale = new Vector2(wallLocalScale.x * scaleFactorLength, wallLocalScale.y * scaleFactorThickness);

            wall.transform.position = position;

            UpdateCollider(wall, wallSpriteRenderer);
        }

        void UpdateCollider(GameObject wall, SpriteRenderer wallSpriteRenderer)
        {
            BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = wallSpriteRenderer.size;
                collider.offset = new Vector2(0, 0);
            }
        }
    }
}