﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkRecovery : CreatureEffect
{

    public DarkRecovery(Player owner, CreatureLogic creature, int creatureEffectCooldown): base(owner, creature, creatureEffectCooldown)
    {
        
    }

    public override void RegisterEventEffect()
    {
        creature.e_PreAttackEvent += UseEffect; 
      
    }

    public override void UnRegisterEventEffect()
    {
         creature.e_PreAttackEvent -= UseEffect;
         
    }

    public override void UseEffect(CreatureLogic target)
    {
        if(creatureEffectCooldown <= 0)
        {
            ShowAbility();
            int damage = target.Health / 2;
            new DealDamageCommand(target.ID, damage, healthAfter: target.TakeOtherDamageVisual(damage)).AddToQueue();
            target.TakeOtherDamage (damage);

            creature.Heal(damage);
            

            base.UseEffect();
        }
    }
}
