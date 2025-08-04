using UnityEngine;
using Ateo.Events;

namespace Ateo.Misc
{
    public class OpenExternalURL : MonoBehaviour
    {
        public string URL;
        public event DelegateVoid OnOpen;

        public void Open()
        {
            OnOpen?.Invoke();
            Application.OpenURL(URL);
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)
