﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : BuffEffect {

    public Defense(CreatureLogic source, CreatureLogic target, int buffCooldown, int specialValue = 2) : base (source, target, buffCooldown, specialValue)
    {
        buffIcon = Resources.Load<Sprite>("BuffIcons/Defense");
        isBuff = true;
    }

    public override void CauseBuffEffect()
    {        
        target.DamageReductionAdditive += specialValue;      
    }

    public override void UndoBuffEffect()
    {
        target.DamageReductionAdditive -= specialValue;
    }


    
}