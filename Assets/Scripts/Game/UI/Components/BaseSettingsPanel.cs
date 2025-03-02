using System;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Controllers.SettingsMenu
{
    public abstract class BaseSettingsPanel : MonoBehaviour
    {
        public event Action OnSettingsChanged;

        protected bool _hasChanges = false;

        public abstract void LoadSettings();

        public abstract void SaveSettings();

        public abstract void DiscardChanges();

        public abstract void ResetToDefaults();

        public virtual bool HasChanges() => _hasChanges;

        protected void NotifySettingsChanged()
        {
            OnSettingsChanged?.Invoke();
        }

        protected virtual void ResetChangeTracking()
        {
            _hasChanges = false;
            NotifySettingsChanged();
        }
    }
}