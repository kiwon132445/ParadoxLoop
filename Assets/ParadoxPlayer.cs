using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ParadoxPlayer : MonoBehaviour
{
    public Loop command;
    [SerializeField] SpellManager spellManager;

    float loopCountdown = 0;
    bool startCountdown = false;
    int loopIndex;

    public void StartLoop(Loop loop)
    {
        command = loop;
        loopCountdown = command.LoopDuration;
        loopIndex = 0;
        startCountdown = true;

        //Debug.Log("Paradox Start");
    }

    private void Update()
    {   
        if (startCountdown)
        {
            loopCountdown -= Time.deltaTime;
            if (loopCountdown <= 0)
            {
                //Debug.Log("Paradox Destroyed");
                Destroy(gameObject);
            }
            if (!spellManager.Casting)
            {
                NextTask();
            }
        }
    }

    public void NextTask()
    {
        //Debug.Log("Task Check");
        if (loopIndex >= command.LoopedSpells.Count)
            return;
        //Debug.Log("Task Start");
        transform.position = command.LoopedSpells[loopIndex].Position;
        spellManager.SelectedResource = command.LoopedSpells[loopIndex].SelectedResource;
        spellManager.Cast(command.LoopedSpells[loopIndex].CastSpell, command.LoopedSpells[loopIndex].CastLocation);
        loopIndex++;
    }
}
