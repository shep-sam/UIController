using UnityEngine;

namespace SpaceCore.UIController
{
    public class UIWindow : MonoBehaviour, IWindow
    {
        [SerializeField]
        private WindowTag windowTag;
        public WindowTag WindowTag { get => windowTag; }

        public virtual void Close()
        {
            UIController.Instance.RemoveWindow(this);
        }
    }
}