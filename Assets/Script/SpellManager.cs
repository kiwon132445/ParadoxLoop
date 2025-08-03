using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    public List<Spell> SpellDictionary {  get; private set; }
    

    public List<Loop> loops = new();
    [SerializeField] Scrollbar spellbar;
    [SerializeField] GameObject ParadoxPrefab;
    [SerializeField] TMP_Dropdown generateResourceDropdown;
    [SerializeField] Button loopButton;
    [SerializeField] TMP_Text loopButtonText;
    [SerializeField] TMP_Dropdown loopDropdown;
    [SerializeField] UIManager uiManager;
    [SerializeField] int loopLimit;

    //Spell currentSpell;
    float castDuration = 0;
    float remainingCastTime = 0;

    [SerializeField] float maxLoopDuration;
    float remainingLoopTime;
    Loop currentLoop;

    public bool Casting = false;
    bool looping = false;
    int loopName = 0;
    public ResourceEnum SelectedResource;

    public Vector3Int lastCastLocation;

    Action CastSpell;

    private void Start()
    {
        SpellDictionary = new(){
            { new Spell("Loop", 2, 10) },
            { new Spell("Destroy Loop", 0, 0) },
            { new Spell("Generate Resource", 2, 10) },
            { new Spell("Gather Resource", 2, 10) },
        };
    }

    public void Cast(Spell spell, Vector3Int castLocation)
    {
        if (Casting || spell == null)
            return;

        switch(spell.Name)
        {
            case "Loop":
                CastSpell += LoopCast;
                if (looping)
                {
                    LoopCast();
                    return;
                }
                break;
            case "Destroy Loop":
                CastSpell += DestroyLoop;
                break;
            case "Generate Resource":
                CastSpell += GenerateResource;
                break;
            case "Gather Resource":
                CastSpell += GatherResources;
                break;
        }

        Casting = true;
        castDuration = spell.Duration;
        remainingCastTime = spell.Duration;
        lastCastLocation = castLocation;

        if (looping)
        {
            //Debug.Log("Spell added to loop");
            currentLoop.LoopedSpells.Add(new LoopData(this.transform.position, castLocation, spell, (ResourceEnum)generateResourceDropdown.value));
        }
    }

    private void Update()
    {
        foreach (Loop loop in loops)
        {
            if (loop.ParadoxPlayer == null)
                StartLoop(loop);
        }

        if (Casting)
        {
            if (!spellbar.gameObject.activeSelf) spellbar.gameObject.SetActive(true);
            remainingCastTime -= Time.deltaTime;
            spellbar.size = remainingCastTime / castDuration;
            if (remainingCastTime <= 0)
            {
                Casting = false;
                spellbar.gameObject.SetActive(false);
                CastSpell?.Invoke();
            }
        }

        if (looping)
        {
            remainingLoopTime -= Time.deltaTime;
            if (remainingLoopTime <= 0)
            {
                EndLoopCast();
            }
        }
    }

    public void StartLoop(Loop loop)
    {
        GameObject paradox = Instantiate(ParadoxPrefab);
        loop.ParadoxPlayer = paradox;
        ParadoxPlayer script = paradox.GetComponent<ParadoxPlayer>();
        script.StartLoop(loop);
    }

    public void DestroyLoop()
    {
        if (loopDropdown.value > loops.Count)
            return;
        CastSpell -= DestroyLoop;
        Loop selectedLoop = loops[loopDropdown.value - 1];
        Destroy(selectedLoop.ParadoxPlayer);
        loops.Remove(selectedLoop);
    }

    public void LoopCast()
    {
        CastSpell -= LoopCast;
        if (loops.Count >= loopLimit)
            return;

        
        if (looping)
        {
            EndLoopCast();
        }
        else
        {
            //Debug.Log("Loop Recording Start");
            currentLoop = new();
            currentLoop.Name = loopName;
            loopName++;
            remainingLoopTime = maxLoopDuration;
            loopButtonText.text = "Stop";
            looping = true;
        }
    }

    public void EndLoopCast()
    {
        //Debug.Log("Loop Recording End");
        loopButtonText.text = "Loop";
        looping = false;
        currentLoop.LoopDuration = maxLoopDuration - remainingLoopTime;
        loops.Add(currentLoop);
        StartLoop(currentLoop);
        currentLoop = null;
        uiManager.UpdateLoopOptions(loops);
    }

    public void GenerateResource()
    {
        CastSpell -= GenerateResource;
        ResourceEnum resource;
        if (generateResourceDropdown != null)
        {
            resource = (ResourceEnum)generateResourceDropdown.value;
        }
        else
        {
            resource = SelectedResource;
        }
        PlayerManager.Instance.ResourceMap.SetTile(lastCastLocation, PlayerManager.Instance.TileDict.ResourceList.Find(data=>data.ResourceName == resource).Tiles[0]);
    }

    public void GatherResources()
    {
        CastSpell -= GatherResources;
        TileBase tile = PlayerManager.Instance.ResourceMap.GetTile(lastCastLocation);
        if (tile == null)
            return;

        foreach (TilesofResources t in PlayerManager.Instance.TileDict.ResourceList)
        {
            if (t.Tiles.Find(data => data == tile) != null)
            {
                PlayerManager.Instance.PlayerResource.AddResources(t.ResourceName, 100);
                break;
            }
        }
        PlayerManager.Instance.ResourceMap.SetTile(lastCastLocation, null);
        //uiManager.UpdateResourceOptions();
    }
}

[Serializable]
public class Spell
{ 
    public string Name;
    public float Duration;
    public int Cost;

    public Spell(string name, int duration, int cost) 
    {
        Name = name;
        Duration = duration;
        Cost = cost;
    }
}

[Serializable]
public class Loop
{
    public int Name;
    public GameObject ParadoxPlayer;
    public float LoopDuration;
    public List<LoopData> LoopedSpells = new();
}

public struct LoopData
{
    public Vector3 Position;
    public Vector3Int CastLocation;
    public Spell CastSpell;
    public ResourceEnum SelectedResource;

    public LoopData(Vector3 position, Vector3Int castLocation, Spell castSpell, ResourceEnum resource)
    {
        Position = position;
        CastLocation = castLocation;
        CastSpell = castSpell;
        SelectedResource = resource;
    }
}