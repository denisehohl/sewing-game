using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.UI
{
    public class PaginationToggles : Pagination
    {
        [Required]
        public UnityEngine.UI.Toggle Prefab;

        private readonly List<UnityEngine.UI.Toggle> _toggles = new List<UnityEngine.UI.Toggle>();
        private UnityEngine.UI.Toggle _current;
        
        public override void Initialize(int count, int index = 0)
        {
            for (var i = 0; i < count; i++)
            {
                var toggle = Instantiate(Prefab.gameObject).GetComponent<UnityEngine.UI.Toggle>();
                toggle.transform.SetParent(transform, false);
                toggle.transform.localScale = Vector3.one;
                toggle.isOn = false;
                _toggles.Add(toggle);
            }
        }

        public override void SelectElement(int index, bool instant = false)
        {
            if (_current != null)
                _current.isOn = false;

            _current = _toggles[index];
            _current.isOn = true;
        }
    }
}