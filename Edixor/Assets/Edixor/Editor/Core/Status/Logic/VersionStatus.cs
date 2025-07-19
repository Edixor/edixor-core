using UnityEngine;
using UnityEngine.UIElements;
using ExTools;

namespace ExTools
{
    public class VersionStatus : ExStatus
    {
        private Label _label;
        private VisualElement _item;

        public override VisualElement LoadUI()
        {
            if (_item != null)
                return _item;

            _item = new VisualElement();
            _item.AddToClassList("status-bar-item");

            _label = new Label();
            _item.Add(_label);

            UpdateLabel();

            return _item;
        }

        private void UpdateLabel()
        {
            if (_label == null)
            {
                Debug.LogWarning("[VersionStatus] Label is null — cannot update");
                return;
            }

            if (container == null)
            {
                Debug.LogError("[VersionStatus] Container is null — cannot resolve setting");
                return;
            }

            string currentVersion = container.ResolveNamed<OtherEdixorSetting>(ServiceNameKeys.OtherEdixorSetting).GetEdixorVersion();
            Debug.Log($"[VersionStatus] Updating label to version: {currentVersion}");
            _label.text = $"Version: {currentVersion}";
        }

        public void Awake()
        {
            Debug.Log("[VersionStatus] Awake called");
        }

        public void OnEnable()
        {
            UpdateLabel();
        }
    }
}
