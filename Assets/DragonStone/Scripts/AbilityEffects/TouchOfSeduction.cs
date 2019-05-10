﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class TouchOfSeduction : CreatureEffect {

    public int buffCooldown = 2;

    public TouchOfSeduction(Player owner, CreatureLogic creature, int creatureEffectCooldown): base(owner, creature, creatureEffectCooldown)
    {}


   public override void RegisterEventEffect()
    {
       creature.e_CreatureOnTurnStart += CauseEventEffect;
        
    }

    public override void UnRegisterEventEffect()
    {
         creature.e_CreatureOnTurnStart += CauseEventEffect;
    }

    public override void CauseEventEffect()
    {
       if(remainingCooldown <=0)
        Debug.Log("Activate Effect: " +this.ToString());
    }

    public override void UseEffect(CreatureLogic target)
    {
        //AddBuff will be called from parent CreatureEffect
        AddBuff(target,"DecreaseAttack",buffCooldown);
       
    }

}