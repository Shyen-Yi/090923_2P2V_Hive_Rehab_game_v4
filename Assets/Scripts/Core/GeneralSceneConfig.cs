using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace com.hive.projectr
{
    public class GeneralSceneConfig : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Extra")]
        [SerializeField] private List<GeneralWidgetConfig> _extraWidgetConfigs;
        [SerializeField] private List<RectTransform> _extraRectTransforms;
        [SerializeField] private List<TMP_Text> _extraTextMeshPros;
        [SerializeField] private List<HiveButton> _extraButtons;
        [SerializeField] private List<Transform> _extraObjects;
        [SerializeField] private List<GameObject> _extraGameObjects;
        [SerializeField] private List<int> _extraInts;
        [SerializeField] private List<CanvasGroup> _extraCanvasGroups;
        [SerializeField] private List<Image> _extraImages;
        [SerializeField] private List<SpriteAtlas> _extraSpriteAtlases;
        [SerializeField] private List<string> _extraStrings;
        [SerializeField] private List<ScriptableObject> _extraScriptableObjects;
        [SerializeField] private List<Camera> _extraCameras;
        [SerializeField] private List<Sprite> _extraSprites;
        [SerializeField] private List<TMP_ColorGradient> _extraColorGradients;
        [SerializeField] private List<Animator> _extraAnimators;

        public CanvasGroup CanvasGroup => _canvasGroup;

        // extra
        public List<GeneralWidgetConfig> ExtraWidgetConfigs => _extraWidgetConfigs;
        public List<RectTransform> ExtraRectTransforms => _extraRectTransforms;
        public List<TMP_Text> ExtraTextMeshPros => _extraTextMeshPros;
        public List<HiveButton> ExtraButtons => _extraButtons;
        public List<Transform> ExtraObjects => _extraObjects;
        public List<GameObject> ExtraGameObjects => _extraGameObjects;
        public List<int> ExtraInts => _extraInts;
        public List<CanvasGroup> ExtraCanvasGroups => _extraCanvasGroups;
        public List<Image> ExtraImages => _extraImages;
        public List<SpriteAtlas> ExtraSpriteAtlases => _extraSpriteAtlases;
        public List<string> ExtraStrings => _extraStrings;
        public List<ScriptableObject> ExtraScriptableObjects => _extraScriptableObjects;
        public List<Camera> ExtraCameras => _extraCameras;
        public List<Sprite> ExtraSprites => _extraSprites;
        public List<TMP_ColorGradient> ExtraColorGradients => _extraColorGradients;
        public List<Animator> ExtraAnimators => _extraAnimators;
    }
}