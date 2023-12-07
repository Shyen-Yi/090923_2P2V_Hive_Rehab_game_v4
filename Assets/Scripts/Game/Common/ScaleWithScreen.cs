using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public enum ScaleWithScreenType
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3,
    }

    public class ScaleWithScreen : MonoBehaviour
    {
        private SpriteRenderer[] _spriteRenderers;

        private void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            for (var i = 0; i < _spriteRenderers.Length; ++i)
            {
                var spriteRenderer = _spriteRenderers[i];

                // Get the texture size of the sprite
                float textureWidth = spriteRenderer.sprite.texture.width;
                float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

                // Calculate the size of the sprite in world units
                float spriteSizeInUnits = textureWidth / pixelsPerUnit;

                // Calculate the width of the screen in world units
                float worldScreenHeight = Camera.main.orthographicSize * 2f;
                float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

                // Calculate the scale factor required to stretch the sprite to match the screen width
                float scaleFactor = worldScreenWidth / spriteSizeInUnits;

                // Apply this scale factor to the sprite's local scale
                transform.localScale = new Vector3(scaleFactor, transform.localScale.y, transform.localScale.z);

                Debug.LogError($"textureWidth: {textureWidth} | spriteSizeInUnits: {spriteSizeInUnits} | worldScreenHeight: {worldScreenHeight} | worldScreenWidth: {worldScreenWidth} | scaleFactor: {scaleFactor}");
            }
        }
    }
}