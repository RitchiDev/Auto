using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tommy
{
    public class ColorManager : MonoBehaviour
    {
        public static ColorManager m_instance;

        public Color m_Prim;
        public Color m_Second;
        public bool m_ChosenStance; //False = Primairy True = Side
        private void Awake()
        {
            m_instance = this;
        }
    
    }

}
