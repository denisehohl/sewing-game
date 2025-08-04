using System;
using UnityEngine;
using Object = System.Object;

namespace Ateo.UI
{
    [Serializable]
    // <summary>
    // Class to store the text and/or image of a single option in the dropdown list.
    // </summary>
    public class DropdownItemData : IComparable
    {
        public string m_Text;
        public Sprite m_Image;
        public Object m_Data;

        /// <summary>
        /// The text associated with the option.
        /// </summary>
        public string Text
        {
            get => m_Text;
            set => m_Text = value;
        }

        /// <summary>
        /// The image associated with the option.
        /// </summary>
        public Sprite Image
        {
            get => m_Image;
            set => m_Image = value;
        }
        
        /// <summary>
        /// The data associated with the option.
        /// </summary>
        public Object Data
        {
            get => m_Data;
            set => m_Data = value;
        }

        public int Index => Item.transform.GetSiblingIndex();
        public DropdownItem Item { get; set; }

        public DropdownItemData()
        {
        }

        public DropdownItemData(string text)
        {
            m_Text = text;
        }

        public DropdownItemData(Sprite image)
        {
            m_Image = image;
        }

        /// <summary>
        /// Create an object representing a single option for the dropdown list.
        /// </summary>
        /// <param name="text">Optional text for the option.</param>
        /// <param name="image">Optional image for the option.</param>
        /// <param name="data">Optional data</param>
        public DropdownItemData(string text, Sprite image, Object data = null)
        {
            m_Text = text;
            m_Image = image;
            m_Data = data;
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                case DropdownItemData other when Index > other.Index:
                    return 1;
                case DropdownItemData other when Index < other.Index:
                    return -1;
                case DropdownItemData other:
                    return 0;
                default:
                    throw new ArgumentException("Object is not a DropdownItemData");
            }
        }

        public override string ToString()
        {
            return m_Text;
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)
