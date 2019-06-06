﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffEffect
{

    public string Name; 
    
    [HideInInspector]
    public CreatureLogic source;
    [HideInInspector]
    public CreatureLogic target;

    //public int buffCooldown;


    private int buffCooldownValue;
    public int buffCooldown
    {
        get
        {
            return buffCooldownValue;
        }
        set
        {
            buffCooldownValue = value;
            
            if (isActive && buffCooldownValue > 0)
            {
                Debug.Log ("Calling Property update cooldown");
                new UpdateBuffCommand(this).AddToQueue();
                    
            }

            else
            {

            }

            
        }
    }

    public bool isBuff;
    public bool isDebuff;

    public Sprite buffIcon;

    //DS
    //Added buffID for Logic and Visual link
    public int buffID;

    //DS
    //Use this flag to ensure that UpdateBUffCommand in buffCooldown prop is only called when the Buff Visual already exists
    public bool isActive = false;
    
    public BuffEffect(CreatureLogic source, CreatureLogic target, int buffCooldown)
    {
        this.source = source;
        this.target = target;
        this.buffCooldown = buffCooldown;

    //DS
    //Added buffID for Logic and Visual link
        this.buffID = IDFactory.GetUniqueID();

        Name = this.GetType().Name.ToString();
        

    }

    ~BuffEffect()
    {
        Debug.Log("Buffeffect destroyed: " +this.GetType().Name);
    }

    public virtual void RegisterCooldown()
    {
        target.e_CreatureOnTurnEnd += ReduceCreatureEffectCooldown;
    }

    public virtual void UnregisterCooldown()
    {
        target.e_CreatureOnTurnEnd -= ReduceCreatureEffectCooldown;
        
    }

    public virtual void CauseBuffEffect(){}

    public virtual void UndoBuffEffect(){}

    public void ReduceCreatureEffectCooldown()
    {       
        if(buffCooldown > 0)
        {
            buffCooldown--;
            //insert UpdateBuffCommand to update the cooldown text
            new UpdateBuffCommand(this).AddToQueue();

        

        }
        if(buffCooldown <= 0)

            RemoveBuff();
            
                        
    }

    public virtual void RemoveBuff()
    {
        Debug.Log("Remove Buff " +this.GetType().Name);
        UndoBuffEffect();
        UnregisterCooldown();        
        target.buffEffects.Remove(this);        
        new DestroyBuffCommand(this, target.UniqueCreatureID).AddToQueue();

        
    }

}
