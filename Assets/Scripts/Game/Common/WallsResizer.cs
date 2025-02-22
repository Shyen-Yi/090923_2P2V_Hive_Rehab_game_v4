using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class WallsResizer
    /// @brief Resizes and repositions the walls in the game scene based on the camera's viewport size.
    /// 
    /// The `WallsResizer` class dynamically adjusts the size and position of the walls (top, bottom, left, and right) in the game scene.
    /// The walls are resized to fit the screen's aspect ratio, ensuring that the gameplay area adapts to different screen resolutions
    /// and aspect ratios. The class also updates the colliders of the walls to match their new size.
    public class WallsResizer : MonoBehaviour
    {
        public GameObject topWall;
        public GameObject bottomWall;
        public GameObject leftWall;
        public GameObject rightWall;

        /// <summary>
        /// Called at the start of the scene to resize the walls.
        /// </summary>
        void Start()
        {
            ResizeWalls();
        }

        /// <summary>
        /// Resizes the walls based on the camera's viewport size and adjusts their positions.
        /// </summary>
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

        /// <summary>
        /// Resizes a wall based on the given parameters (length, thickness, and position).
        /// </summary>
        /// <param name="wall">The wall GameObject to resize.</param>
        /// <param name="length">The desired length of the wall.</param>
        /// <param name="thickness">The desired thickness of the wall.</param>
        /// <param name="position">The desired position of the wall.</param>
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

        /// <summary>
        /// Updates the collider size and offset to match the wall's sprite size.
        /// </summary>
        /// <param name="wall">The wall GameObject whose collider needs updating.</param>
        /// <param name="wallSpriteRenderer">The SpriteRenderer of the wall used to get the new size.</param>
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