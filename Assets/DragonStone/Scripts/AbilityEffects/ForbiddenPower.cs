﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForbiddenPower : CreatureEffect
{
    public int buffCooldown = 1;

    bool effectChance = false;
    public ForbiddenPower(Player owner, CreatureLogic creature, int creatureEffectCooldown): base(owner, creature, creatureEffectCooldown)
    {
        
    }

    public override void RegisterEventEffect()
    {
       creature.e_PreAttackEvent += CheckChance;      
       creature.e_AfterAttacking += UseEffect;      
    }

    public override void UnRegisterEventEffect()
    {
        creature.e_PreAttackEvent -= CheckChance;  
        creature.e_AfterAttacking -= UseEffect;          
    }

    public void CheckChance(CreatureLogic target)
    {
        if(creatureEffectCooldown <= 0)
        {
            effectChance = false;
            
            if(Random.Range(0,100)<=creature.chance)
            {
                ShowAbility();
                
                effectChance = true;
                creature.CriticalChance += 1;

            }
        }

    }
    public override void UseEffect(CreatureLogic target)
    {
        if(creatureEffectCooldown <= 0)
        {

            if(effectChance)
            {            
                AddBuff(target, "Poison", buffCooldown);
                creature.CriticalChance -= 1;
            }

            base.UseEffect();
        }
    }
}
