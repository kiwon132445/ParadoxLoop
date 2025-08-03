using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resourceDropdown;
    [SerializeField] TMP_Dropdown generateDropdown;
    [SerializeField] TMP_Dropdown loopDropdown;

    public void UpdateResourceOptions()
    {
        resourceDropdown.ClearOptions();

        List<string> optionList = new();
        foreach (Resource r in  PlayerManager.Instance.PlayerResource.resources)
        {
            optionList.Add($"{r.ResourceName.ToString()}: {r.Quantity}");
        }
        resourceDropdown.AddOptions(optionList);
    }

    public void UpdateGenerateOptions()
    {
        generateDropdown.ClearOptions();

        List<string> generateList = new();
        foreach (Resource r in PlayerManager.Instance.PlayerResource.resources)
        {
            generateList.Add($"{r.ResourceName.ToString()}");
        }
        generateDropdown.AddOptions(generateList);
    }

    public void UpdateLoopOptions(List<Loop> loopList)
    {
        loopDropdown.ClearOptions();

        List<string> optionList = new();
        optionList.Add("Remove Loop");
        foreach (Loop l in loopList)
        {
            optionList.Add(l.Name.ToString());
        }
        loopDropdown.AddOptions(optionList);
    }
}
