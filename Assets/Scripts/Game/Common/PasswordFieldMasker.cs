using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.hive.projectr
{
    [RequireComponent(typeof(TMP_InputField))]
    public class PasswordFieldMasker : MonoBehaviour
    {
        public string RealContent { get; private set; }

        [SerializeField, Range(0, 5f)] private float _maskDelay = 1f;

        private TMP_InputField _inputField;
        private Coroutine _routine;

        private void Awake()
        {
            if (_inputField == null)
            {
                _inputField = GetComponent<TMP_InputField>();
            }
        }

        private void OnEnable()
        {
            _inputField.onValueChanged.AddListener(OnValueChanged);

            StopMaskRoutine();
        }

        private void OnDisable()
        {
            _inputField.onValueChanged.RemoveAllListeners();

            StopMaskRoutine();
        }

        private void OnValueChanged(string value)
        {
            if (value.Length > RealContent.Length)
            {
                RealContent += value.Substring(RealContent.Length);
                StopMaskRoutine();
                StartMaskRoutine();
            }
            else if (value.Length < RealContent.Length)
            {
                RealContent = RealContent.Substring(0, value.Length);
            }

            Logger.LogError($"RealContent: {RealContent}");
        }

        private void StopMaskRoutine()
        {
            if (_routine != null)
            {
                MonoBehaviourUtil.Instance.StopCoroutine(_routine);
                _routine = null;
            }
        }

        private void StartMaskRoutine()
        {
            _routine = MonoBehaviourUtil.Instance.StartCoroutine(MaskRoutine());
        }

        private IEnumerator MaskRoutine()
        {
            var maskedContent = new string('*', RealContent.Length - 1);
            if (RealContent.Length > 0)
            {
                maskedContent += RealContent[RealContent.Length - 1];
            }
            _inputField.text = maskedContent;

            yield return new WaitForSeconds(_maskDelay);

            _inputField.text = new string('*', RealContent.Length);
        }
    }
}