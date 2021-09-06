

using UnityEngine;

namespace UINavigation
{
    public class GoToKeyComponent : BaseGoToComponent
    {
        public string nextKey;

        protected override void Awake()
        {
            if (string.IsNullOrEmpty(nextKey))
                nextKey = gameObject.name;
        }

        public override void GoToNext()
        {
            if (manager == null)
                return;

            manager.ShowPanel(nextKey);
        }
    }
}
