using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public List<Resource> resources;
    [SerializeField] UIManager uiManager;

    public void Start()
    {
        resources = new List<Resource>();
        InitializeResources();
    }

    public void InitializeResources()
    {
        foreach (ResourceEnum name in Enum.GetValues(typeof(ResourceEnum)))
        {
            resources.Add(new Resource(name, 0));
        }
        uiManager.UpdateResourceOptions();
        uiManager.UpdateGenerateOptions();
    }

    public Resource GetResource(ResourceEnum name)
    {
        if (resources == null)
            return null;
        return resources.Find(data => data.ResourceName == name);
    }

    public bool EnoughResource(ResourceEnum name, int quantity)
    {
        if (GetResource(name).Quantity < quantity)
            return false;
        return true;
    }

    public bool EnoughResources(List<Resource> list)
    {
        foreach (Resource resource in list)
        {
            if (resource == null || !EnoughResource(resource.ResourceName, resource.Quantity))
                return false;
        }
        return true;
    }

    public bool UseResource(ResourceEnum name, int quantity)
    {
        if (resources == null)
            return false;

        if (!EnoughResource(name, quantity))
            return false;

        GetResource(name).Quantity -= quantity;
        uiManager.UpdateResourceOptions();
        return true;
    }

    public void AddResources(ResourceEnum name, int quantity)
    {
        if (resources == null)
            return;

        GetResource(name).Quantity += quantity;
        uiManager.UpdateResourceOptions();
    }
}

[Serializable]
public class Resource
{
    public ResourceEnum ResourceName;
    public int Quantity;

    public Resource(ResourceEnum name, int quantity) 
    { 
        ResourceName = name;
        Quantity = quantity;
    }
}
