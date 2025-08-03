using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public PlayerResources PlayerResource;
    public SpellManager SpellManager;

    [SerializeField] PlayerStats baseStats;
    [SerializeField] PlayerStats appliedStats;

    [SerializeField] Scrollbar mpBar;
    [SerializeField] TMP_Text mpBarText;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject selectionBox;

    public Tilemap ResourceMap;
    public ResourceTiles TileDict;
    [SerializeField] UIManager uiManager;
    [SerializeField] Grid grid;

    PlayerStats currentStats;
    public float CurrentMP;

    Vector2 playerDirection = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        StatCalculation();
        //uiManager.UpdateGenerateOptions();
        CurrentMP = currentStats.MaxMP;
    }
    // Update is called once per frame
    void Update()
    {
        StatCalculation();
        RegenMP();
        Movement();
    }

    public void OnMove(InputValue value)
    {
        playerDirection = value.Get<Vector2>();
    }

    void Movement()
    {
        if (playerDirection == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (playerDirection.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        selectionBox.transform.position = grid.WorldToCell(this.transform.position + new Vector3(playerDirection.x, playerDirection.y, 0));
        selectionBox.transform.position += new Vector3(0.5f, 0.5f, 0);
        rb.linearVelocity = currentStats.Speed * playerDirection;
    }

    void StatCalculation()
    {
        currentStats.MaxMP = baseStats.MaxMP + appliedStats.MaxMP;
        currentStats.MPRegen = baseStats.MPRegen + appliedStats.MPRegen;
        currentStats.Speed = baseStats.Speed + appliedStats.Speed;
    }

    void RegenMP()
    {
        if (CurrentMP < currentStats.MaxMP)
        {
            CurrentMP += Time.deltaTime * currentStats.MPRegen;
        }
        else if (CurrentMP > currentStats.MaxMP)
            CurrentMP = currentStats.MaxMP;

        mpBar.size = CurrentMP / currentStats.MaxMP;
        mpBarText.text = $"MP {Mathf.RoundToInt(CurrentMP)}/{currentStats.MaxMP}";
    }

    public void OnSelect(InputValue value)
    {
        Debug.Log(ResourceMap.GetTile(grid.WorldToCell(selectionBox.transform.position)));
        //return lastPosition;
    }

    public void CastSpell(string name)
    {
        Spell spell = SpellManager.SpellDictionary.Find(data => data.Name.Equals(name));
        if (spell.Cost > CurrentMP)
            return;
        CurrentMP -= spell.Cost;
        SpellManager.Cast(spell, grid.WorldToCell(selectionBox.transform.position));
    }
}

[Serializable]
public struct PlayerStats
{
    [field: SerializeField]
    public int MaxMP;
    [field: SerializeField]
    public float MPRegen;
    [field: SerializeField]
    public float Speed;
}