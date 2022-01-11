using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMaterialManager : MonoBehaviour
{
    public static CarMaterialManager Instance { get; private set; }

    [System.Serializable]
    public class CarMaterial
    {
        [SerializeField] private Material m_Material;
        [SerializeField] private Color m_Color;

        public Material Material => m_Material;
        public Color Color => m_Color;
    }

    [SerializeField] private List<CarMaterial> m_PrimaryMaterials = new List<CarMaterial>();
    [SerializeField] private List<CarMaterial> m_SecondaryMaterials = new List<CarMaterial>();

    private int m_MaxPrimaryIndex;
    public int MaxPrimaryIndex => m_MaxPrimaryIndex;

    private int m_MaxSecondaryIndex;
    public int MaxSecondaryIndex => m_MaxSecondaryIndex;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (m_PrimaryMaterials.Count >= 1)
        {
            m_MaxPrimaryIndex = m_PrimaryMaterials.Count - 1;
        }

        if (m_SecondaryMaterials.Count >= 1)
        {
            m_MaxSecondaryIndex = m_SecondaryMaterials.Count - 1;
        }
    }

    public Material GetSelectedPrimaryMaterial(int index)
    {
        index = Mathf.Clamp(index, 0, m_MaxPrimaryIndex);

        return m_PrimaryMaterials[index].Material;
    }

    public Material GetSelectedSecondaryMaterial(int index)
    {
        index = Mathf.Clamp(index, 0, m_MaxSecondaryIndex);

        return m_SecondaryMaterials[index].Material;
    }

    public Color GetSelectedPrimaryColor(int index)
    {
        index = Mathf.Clamp(index, 0, m_MaxPrimaryIndex);

        return m_PrimaryMaterials[index].Color;
    }

    public Color GetSelectedSecondaryColor(int index)
    {
        index = Mathf.Clamp(index, 0, m_MaxSecondaryIndex);

        return m_SecondaryMaterials[index].Color;
    }
}
