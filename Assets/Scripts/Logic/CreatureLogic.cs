﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureLogic: ICharacter
{
    // PUBLIC FIELDS
    public Player owner;
    public string Name;
    public CardAsset ca;
    public CreatureEffect effect;
    public int UniqueCreatureID;
    public bool Frozen = false;
    public bool isDead;

    public bool isActive;

    [HideInInspector]
    public BuffEffect testBuff;
/*    
    {
        get{ return isActiveValue; }

        set
        {
            isActiveValue = value;
            GameObject g = IDHolder.GetGameObjectWithID(UniqueCreatureID);
            if (g!= null)
            {
                g.GetComponent<OneCreatureManager>().IsActive = isActiveValue;
            }
        }
    }
*/


    public int chance;

    public bool isEquip = false;

    //DS
    //Adding Abilities
    public List<AbilityAsset> abilities; 

    public AbilityAsset equipAbility;   
    
    public List<CreatureEffect> creatureEffects = new List<CreatureEffect>(); 

    public CreatureEffect equipEffect;

    public List<BuffEffect> buffEffects = new List<BuffEffect>(); 

    //public List<CreatureEffect> creatureEffects; 

    //public List<BuffEffect> buffEffects;

    public delegate void CreatureOnTurnStart();    
    public event CreatureOnTurnStart e_CreatureOnTurnStart;

    public delegate void PreAttackEvent(CreatureLogic target);    
    public event PreAttackEvent e_PreAttackEvent;

    public delegate void CreatureOnTurnEnd();    
    public event CreatureOnTurnEnd e_CreatureOnTurnEnd;   

    public delegate void ThisCreatureDies(CreatureLogic creature);    
    public event ThisCreatureDies e_ThisCreatureDies;

    public delegate void IsAttacked(CreatureLogic creature);    
    public event IsAttacked e_IsAttacked;

    public delegate void IsAttackedBy(CreatureLogic source);    
    public event IsAttackedBy e_IsAttackedBy;

    public delegate void IsDamagedByAttack(CreatureLogic source, CreatureLogic target, int damage);    
    public event IsDamagedByAttack e_IsDamagedByAttack;


    public delegate void IsComputeDamage();    
    public event IsComputeDamage e_IsComputeDamage;     

    public delegate void BeforeAttacking(CreatureLogic target);    
    public event BeforeAttacking e_BeforeAttacking; 
    
    public delegate void AfterAttacking(CreatureLogic target);    
    public event AfterAttacking e_AfterAttacking; 

    public delegate void SecondAttack(CreatureLogic target);    
    public event SecondAttack e_SecondAttack; 
    
    public delegate void BuffApplied(CreatureLogic Target, BuffEffect buff);    
    public event BuffApplied e_buffApplied;     
    //Debuff flags
    public bool isAttacked;   
    
    
    // PROPERTIES
    // property from ICharacter interface
    public int ID
    {
        get{ return UniqueCreatureID; }
    }
        
    // the basic health that we have in CardAsset
    private int baseHealth;
    // health with all the current buffs taken into account
    public int MaxHealth
    {
        get{ return baseHealth;}
    }

    // current health of this creature
    private int health;
    public int Health
    {
        get{ return health; }

        set
        {                 
            if (value > MaxHealth)
                health = MaxHealth;
            else if (value <= 0)
                Die();
            else
                health = value;
        }
    }

    private int armor;
    public int Armor
    {
        get{ return armor; }

        set
        {
            if (value <= 0)
            {
                armor = 0;
                value = armor;

                new UpdateArmorCommand(ID, value).AddToQueue();                   
                //this.DestroyArmor();
            }                
                
            else
            {
                armor = value;
                new UpdateArmorCommand(ID, value).AddToQueue();
            }
                

            
        }
    }

    // returns true if we can attack with this creature now
    public bool CanAttack
    {

        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !Frozen);
        }
        set{}
        
        
    }

    
    // property for Attack
    private int baseAttack;
    public int Attack
    {
        get{ return baseAttack; }

        set{ baseAttack = value;}
    }
     
    // number of attacks for one turn if (attacksForOneTurn==2) => Windfury
    public int attacksForOneTurn = 1;

    private int attacksLeftThisTurn;
    public int AttacksLeftThisTurn
    {
        get{
            return attacksLeftThisTurn;
        }

        set{
            if(value < 0)
                attacksLeftThisTurn = 0;
            else
                attacksLeftThisTurn = value;
        }
    }

    private int attackDamage;
    public int AttackDamage
    {
        get {return attackDamage;}
        set{
            if(value < 0)
                attackDamage = 0;
            else
                attackDamage = value;
        }
    }


    private int criticalFactor = 1;
    public float CriticalChance
    {
        get{return criticalFactor;}
        set{
            if(value < 1)
                criticalFactor = 1;
            else
                criticalFactor = 2;
        }
    }

    private int otherFactor = 1;
    public int OtherFactor
    {
        get{return otherFactor;}
        set{otherFactor = value;}
    }

    private int damageReduction = 1;
    public int DamageReduction
    {
        get{return damageReduction;}
        set
        {
            damageReduction = value;
        }
    }

    private int damageReductionAdditive = 0;
    public int DamageReductionAdditive
    {
        get{return damageReductionAdditive;}
        set
        {
            damageReductionAdditive = value;
        }
    }

    private int otherDamageReduction = 1;
    public int OtherDamageReduction
    {
        get{return otherDamageReduction;}
        set
        {
            otherDamageReduction = value;
        }
    }

    
    //Flags
   
    [HideInInspector]
    public int healFactor = 1;
    [HideInInspector]
    public bool canDebuff = true;
    [HideInInspector]
    public bool canBuff = true;
    [HideInInspector]
    public bool canBeDebuffed = true;
    [HideInInspector]
    public bool canBeBuffed = true;
    [HideInInspector]
    public int targetAttackDamage = 0;
    [HideInInspector]
    public bool canUseAbility = true;
    [HideInInspector]
    public int attackTurnModifier;

   

    [HideInInspector]
    public bool canBeAttacked = true;
    public bool hasTaunt = false;

    public bool hasBless = false;

    public bool hasEndure = false;

    public float chanceTakeDamageFromAttack = 100;

    public int lastDamageValue;

    public bool hasHorror = false;

    public bool isPrimaryForm = true;

    public bool hasCurse = false;

    public bool pauseAttack = false;
    public bool hasStun = false;


    // CONSTRUCTOR
    public CreatureLogic(Player owner, CardAsset ca)
    {

        
        
        this.ca = ca;

        //DS
        abilities = ca.Abilities;
        equipAbility = ca.equipAbility;
        chance = ca.Chance;


        baseHealth = ca.MaxHealth;
        Health = ca.MaxHealth;
        baseAttack = ca.Attack;
        
        attacksForOneTurn = ca.AttacksForOneTurn;

        // Remove Charge
        // if (ca.Charge)
        //     AttacksLeftThisTurn = attacksForOneTurn;

        this.owner = owner;
        UniqueCreatureID = IDFactory.GetUniqueID(); 
        //Name = this.GetType().Name.ToString();
        Name = ca.cardName;

       
        //DS
        //Add activator for abilities

        /* Replace with Abilities SO
        if (ca.abilityEffect != null)
        {
            foreach (AbilityEffect ae in ca.abilityEffect)
            {
                if (ae.CreatureScriptName != null && ae.CreatureScriptName != "")
                {
                    effect = System.Activator.CreateInstance(System.Type.GetType(ae.CreatureScriptName), new System.Object[]{owner, this, ae.coolDown}) as CreatureEffect;
                    effect.RegisterCooldown();
                    effect.RegisterEventEffect();

                    if (ae.abilityImage != null)
                    effect.abilityPreviewSprite = ae.abilityImage;
                    effect.abilityDescription = ae.abilityDescription;

                    creatureEffects.Add(effect);

                   
                   
                    
                }
            }
        }
        */

        if (ca.Abilities != null)
        {
            foreach (AbilityAsset ae in ca.Abilities)
            {
                if (ae.abilityEffect != null && ae.abilityEffect != "")
                {
                    
                    effect = System.Activator.CreateInstance(System.Type.GetType(ae.abilityEffect), new System.Object[]{owner, this, ae.abilityCoolDown}) as CreatureEffect;
                    
                    //effect.RegisterCooldown();
                    //effect.RegisterEventEffect();

                    if (ae.icon != null)
                    effect.abilityPreviewSprite = ae.icon;
                    effect.abilityDescription = ae.description;

                    creatureEffects.Add(effect);
 
                }
            }
        }


//for Equip
/*
        if (ca.equipAbility != null)
        {
            if (ca.equipAbility.abilityEffect != null && ca.equipAbility.abilityEffect != "")
            {
                effect = System.Activator.CreateInstance(System.Type.GetType(ca.equipAbility.abilityEffect), new System.Object[]{owner,this}) as CreatureEffect;
                //effect.RegisterCooldown();
                //effect.RegisterEventEffect();

                if (ca.equipAbility.icon != null)
                effect.abilityPreviewSprite = ca.equipAbility.icon;
                effect.abilityDescription = ca.equipAbility.description;

                equipEffect = effect;

            }
        }
        */



        //DS

        CreaturesCreatedThisGame.Add(UniqueCreatureID, this);

    }

    
    // METHODS
    public void OnTurnStart()
    {
        
       
        // will be granted by Player

        isActive = true;

        AttacksLeftThisTurn = attacksForOneTurn + attackTurnModifier; 
        //TurnOrder:  Check Stun, Ability Cooldown Reduction, Effects       

        //TODO: Buff/Debuff effects (like Poison, Heal)   

         //TODO:  Ability Effects (BattleCry, etc.)
        foreach(CreatureEffect ce in creatureEffects)
        {
            //Debug.Log ("CreatureEffect: " + ce.ToString() + ", CD: " + ce.remainingCooldown);
        }

//         if(e_CreatureOnTurnStart != null)
//            e_CreatureOnTurnStart.Invoke();

        
                
    }

    public void OnTurnEnd(){
        isActive = false;
        AttacksLeftThisTurn = 0;


        //DS "color" Creature that already attacked
            new CreatureColorCommand(this,true).AddToQueue();

        //if (owner.ExtraCreatureTurn != 0)
        //    owner.ExtraCreatureTurn = 0;

        //TODO:  End of Turn Effects


        if(e_CreatureOnTurnEnd != null)
            e_CreatureOnTurnEnd.Invoke();               
        
        

        //TODO:  Buff Duration Reduction                       
    }

    public void DestroyArmor()
    {
         if(Armor <=0)
            {
                for(int i = this.buffEffects.Count-1; i>=0; i--)
                {
                    Debug.Log("Hero Name : " +this.Name);
                    if(this.buffEffects[i].Name == "Armor")
                    {
                         Debug.Log("Command Buff: " +this.buffEffects[i].Name);
                         this.RemoveBuff(this.buffEffects[i]);
                    }
                    
                }
            }

    }

    public void Die()
    {   
        //ORIGINAL SCRIPT
        //owner.table.CreaturesOnTable.Remove(this);        
        
        //New SCRIPT
      
        this.isDead = true;

        this.isActive = false;

        
        //Remove all buffs/debuffs        
        RemoveAllBuffs(); 

        
        foreach(CreatureEffect ce in creatureEffects)
        {
            if(effect !=null)
            {
                ce.remainingCooldown = ce.creatureEffectCooldown;
                ce.WhenACreatureDies();
                ce.UnRegisterEventEffect();
                ce.UnregisterCooldown();                                   
            }
        }        
            
        //if(!hasResurrect)
        
        //REMOVE INSTANTIATED EFFECTS
        //creatureEffects.Clear();
        

        new CreatureDieCommand(UniqueCreatureID, owner).AddToQueue();         
        

        if(e_ThisCreatureDies != null)
        e_ThisCreatureDies.Invoke(this);
        
        owner.CheckIfGameOver();           
    }

    public void ExtraTurn()
    {
        isActive = true; 
        AttacksLeftThisTurn++;
        new CreatureColorCommand(this,false).AddToQueue();

    }

    public void Revive()
    {
        if (!hasCurse)
        {
            isDead = false;
            OnTurnStart();        

            foreach(CreatureEffect ce in creatureEffects)
            {
                if(effect !=null)
                {
                    ce.remainingCooldown = ce.creatureEffectCooldown;
                    ce.RegisterCooldown();
                    ce.RegisterEventEffect();                                
                }
            }

            
            new CreatureResurrectCommand(UniqueCreatureID, owner).AddToQueue(); 
            new UpdateAttackCommand(ID, Attack).AddToQueue();
            new UpdateHealthCommand(ID, MaxHealth).AddToQueue();
            new UpdateArmorCommand(ID, Armor).AddToQueue();
            new CreatureColorCommand(this,false).AddToQueue();
        }

    }

    public void GoFace()
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = owner.otherPlayer.Health - Attack;
        //new CreatureAttackCommand(owner.otherPlayer.PlayerID, UniqueCreatureID, 0, Attack, Health, targetHealthAfter, CanAttack).AddToQueue();
        owner.otherPlayer.Health -= Attack;
    }

    public void AttackCreature (CreatureLogic target)
    {
        lastDamageValue = 0;
        
        AttacksLeftThisTurn--;         


        if(!target.isDead)
        {
            AttackDamage = DealDamage(Attack);

            if (Random.Range(0,100) <= chanceTakeDamageFromAttack)
            {          
                target.TakeDamage(this, AttackDamage);
                
            }

            int targetHealthAfter = target.Health;
            int attackerHealthAfter = Health;
            int targetArmorAfter = target.Armor;
            new CreatureAttackCommand(target.UniqueCreatureID, UniqueCreatureID, target.targetAttackDamage, AttackDamage, attackerHealthAfter, targetHealthAfter, targetArmorAfter, CanAttack).AddToQueue();
            
            if(this.e_AfterAttacking != null)
            this.e_AfterAttacking.Invoke(target);

            if (target.lastDamageValue > 0)
            {
                if(e_IsDamagedByAttack != null)
                    e_IsDamagedByAttack.Invoke(this, target, target.lastDamageValue); 
                
                target.TriggerThisWhenAttacked(this);

            }
        
            if(!CanAttack)
            {
                new EndTurnCommand().AddToQueue();
                OnTurnEnd();
            }
            else
            {            
                if(e_SecondAttack != null)
                e_SecondAttack.Invoke(target);    
            }   

        } else {
             if(!CanAttack)
            {
                new EndTurnCommand().AddToQueue();
                OnTurnEnd();
            }
            else
            {            
                if(e_SecondAttack != null)
                e_SecondAttack.Invoke(target);    
            }   
        }
    } 

    public void TriggerThisWhenAttacked (CreatureLogic attacker)
    {

        if(e_IsAttackedBy != null)
            e_IsAttackedBy.Invoke(attacker);

    }

    public int DealDamage(int amount)
    {
        int damage = amount*criticalFactor;

        //if(e_IsComputeDamage!=null)
        //e_IsComputeDamage();

        return damage;
    }

    //used by non-attack based damage
    public int DealOtherDamage(int amount)
    {
        int damage = amount*OtherFactor;

        //if(e_IsComputeDamage!=null)
        //e_IsComputeDamage();

        return damage;
    }

    //input: DealDamage
    //for attack damage
    public void TakeDamage(CreatureLogic source, int damage)
    {
  
        if(!hasBless)
        {
            if(hasHorror && source.CriticalChance == 1)
                damage = damage * 2;
            
            int finalDamage = DamageReduction*damage - DamageReductionAdditive;
            int healthAfter = Health;
            int spillDamage = 0;
            
            if (Armor > 0)
            {
                Debug.Log("Armor: " + Armor);
                Debug.Log("Final Damage: " + finalDamage);
                if (Armor > finalDamage)
                {
                    Armor -= finalDamage;
                }
                else if (Armor == finalDamage)
                {
                    Armor = 0;
                    //source.DestroyArmor();
                    this.DestroyArmor();
                }
                else if (Armor < finalDamage)
                {
                    spillDamage = Armor- finalDamage;                    
                    Armor = 0;
                    //source.DestroyArmor();
                    this.DestroyArmor();
                    healthAfter += spillDamage;
                }
            }

            else
            {
                healthAfter-=finalDamage;
            }

            lastDamageValue = finalDamage;

            if (healthAfter <= 0 && hasEndure)
            {
                Health = 1;
            }
            else
                Health = healthAfter;

            if(e_IsAttacked != null)
            e_IsAttacked.Invoke(this); 

            if(e_IsComputeDamage!=null && (damage - DamageReductionAdditive) > 0)
            e_IsComputeDamage();



        }
    }

    public void TakeSplashDamage(CreatureLogic source, int damage)
    {
  
        if(!hasBless)
        {
            if(hasHorror && source.CriticalChance == 1)
                damage = damage * 2;
            
            int finalDamage = DamageReduction*damage - DamageReductionAdditive;
            int healthAfter = Health;
            int spillDamage = 0;
            
            if (Armor > 0)
            {
                Debug.Log("Armor: " + Armor);
                Debug.Log("Final Damage: " + finalDamage);
                if (Armor > finalDamage)
                {
                    Armor -= finalDamage;
                }
                else if (Armor == finalDamage)
                {
                    Armor = 0;
                }
                else if (Armor < finalDamage)
                {
                    spillDamage = Armor - finalDamage;                    
                    Armor = 0;
                    healthAfter += spillDamage;
                }
            }

            else
            {
                healthAfter-=finalDamage;
            }

            lastDamageValue = finalDamage;

            if (healthAfter <= 0 && hasEndure)
            {
                Health = 1;
            }
            else
                Health = healthAfter;

            if(e_IsComputeDamage!=null && (damage - DamageReductionAdditive) > 0)
            e_IsComputeDamage();



        }
    }

    // for non-attack damage
    public void TakeOtherDamage(int damage)
    {

        int finalOtherDamage = OtherDamageReduction*damage - DamageReductionAdditive;
        int healthAfter = Health;
        int spillDamage = 0;
        
        if (Armor > 0)
        {
            if (Armor > finalOtherDamage)
            {
                Armor -= finalOtherDamage;
            }
            else if (Armor == finalOtherDamage)
            {
                Armor = 0;
            }
            else if (Armor < finalOtherDamage)
            {
                spillDamage = Armor - finalOtherDamage;
                Armor = 0;
                healthAfter += spillDamage; 
            }
        }

        else
        {
            healthAfter-=finalOtherDamage;
        }

        lastDamageValue = finalOtherDamage;

        if (healthAfter <= 0 && hasEndure)
            {
                Health = 1;
            }
        else
            Health = healthAfter;

        if(e_IsComputeDamage!=null && (damage - DamageReductionAdditive) > 0)
            e_IsComputeDamage();

    }


    //DS: TO BE REVIEWED
    public int TakeOtherDamageVisual(int damage)
    {
        int finalOtherDamage = OtherDamageReduction*damage - DamageReductionAdditive;
        int healthAfter = Health;
        int spillDamage = 0;
        int armorValue = Armor;
        
        if (armorValue > 0)
        {
            if (armorValue > finalOtherDamage)
            {
                armorValue -= finalOtherDamage;
            }
            else if (armorValue == damage)
            {
                armorValue = 0;
            }
            else
            {
                spillDamage = armorValue - finalOtherDamage;
                armorValue = 0;
                healthAfter -= spillDamage; 
            }
        }

        else
        {
            healthAfter-=finalOtherDamage;
        }

        if (healthAfter <= 0 && hasEndure)
            {
                return 1;
            }
        else
            return healthAfter;
    }

    public int TakeArmorDamageVisual(int damage)
    {
        int finalOtherDamage = OtherDamageReduction*damage - DamageReductionAdditive;
        int healthAfter = Health;
        int spillDamage = 0;
        int armorValue = Armor;
        
        if (armorValue > 0)
        {
            if (armorValue > finalOtherDamage)
            {
                armorValue -= finalOtherDamage;
            }
            else if (armorValue == damage)
            {
                armorValue = 0;
            }
            else
            {
                spillDamage = armorValue - finalOtherDamage;
                armorValue = 0;
                healthAfter -= spillDamage; 
            }

            return armorValue;
        }

        else
            return 0;
    }

    public void AttackCreatureWithID(int uniqueCreatureID)
    {
        
        CreatureLogic target = CreatureLogic.CreaturesCreatedThisGame[uniqueCreatureID];
        new StartPreAttackSequenceCommand(this, target).AddToQueue();
        

        //PreAttack(target);
        //AttackCreature(target);        
        
    }


    public void PreAttack(CreatureLogic target)
    {
        //Subscribe all creature pre-attack abilities here
        if(e_PreAttackEvent != null)
           this.e_PreAttackEvent.Invoke(target);
           new DelayCommand(0.5f).AddToQueue();

        if(!pauseAttack)
            new StartAttackSequenceCommand(this, target).AddToQueue();
        else
        {
            pauseAttack = false;
        }


    }

    public void SplashAttackDamage(CreatureLogic target, int splashDamage)
    {
        List<CreatureLogic> enemies = owner.EnemyList();

            foreach(CreatureLogic enemy in enemies)
            {
                
                if(enemy != target && !enemy.isDead)
                {
                     //new DelayCommand(0.5f).AddToQueue();    
                
                    

                        int damageTakenByTarget = splashDamage;
                        // GameObject targetEnemy = IDHolder.GetGameObjectWithID(enemy.UniqueCreatureID);    

                    	// if(damageTakenByTarget>0) 
	                    // DamageEffect.CreateDamageEffect(targetEnemy.transform.position, damageTakenByTarget);

                        new SplashDamageCommand(enemy.UniqueCreatureID, damageTakenByTarget).AddToQueue();
	                    

                    //enemy.TakeDamage(creature.AttackDamage);      
                    enemy.TakeSplashDamage(this, splashDamage);      
                   
                   new UpdateHealthCommand(enemy.UniqueCreatureID, enemy.Health).AddToQueue();
                }                
                               
            }         

            enemies.Clear();
    }





    public void AddBuff(BuffEffect buff)
    {
        bool buffExists = false;

        //check if same buff already exists, just update the buff
        foreach (BuffEffect be in buffEffects)
        {
            if (be.GetType().Name == buff.GetType().Name)
            {
                be.source = buff.source;
                be.target = buff.target;
                
                if (be.buffCooldown < buff.buffCooldown)
                    be.buffCooldown = buff.buffCooldown;

            
            //Set Armor to Original Value when re-applied
            if (be.GetType().Name == "Armor")
            {
                           
                if(be.target.Armor < be.defaultArmor)
                be.target.Armor = be.defaultArmor;
            }
            
                

                buffExists = true;

                new UpdateBuffCommand(be).AddToQueue();

                new ShowBuffPreviewCommand(buff, this.ID, buff.GetType().Name).AddToQueue();

                if(e_buffApplied!=null)
                    e_buffApplied(this, buff);
                
            }
        }

        //if buff is new, add it to the CL

        if (!buffExists)
        {
            buff.RegisterCooldown();
            
            buff.CauseBuffEffect();
            buffEffects.Add(buff);

           new AddBuffCommand(buff, UniqueCreatureID).AddToQueue();

           new ShowBuffPreviewCommand(buff, this.ID, buff.GetType().Name).AddToQueue();

                if(e_buffApplied!=null)
                    e_buffApplied(this, buff);
        }
        

    }   

    

   //Remove Buff is in BuffEffect Script

    public BuffEffect RandomBuff()
    {
        var randList = new List<BuffEffect>();

        foreach(BuffEffect be in buffEffects)
        {
            if(be.isBuff)
            randList.Add(be);
        }

        if(randList.Count>=1)
        {
            BuffEffect buff = randList[Random.Range(0,randList.Count)];
            return buff;
        }

        else return null;

        
    }

    public BuffEffect RandomDebuff()
    {
        var randList = new List<BuffEffect>();

        foreach(BuffEffect be in buffEffects)
        {
            if(be.isDebuff)
            randList.Add(be);
        }

        if(randList.Count>=1)
        {
            BuffEffect buff = randList[Random.Range(0,randList.Count)];
            return buff;
        }
        else return null;

        
    }



   public void RemoveRandomBuff()
    {
        BuffEffect buff = RandomBuff();
        if (buff != null)
        {
            RemoveBuff(buff);
        }
       
    }



   public void RemoveRandomDebuff()
    {
        BuffEffect buff = RandomDebuff();
        if (buff != null)
        {
            RemoveBuff(buff);
        }
       
    }

    public void RemoveBuff(BuffEffect buff)
    {

            buff.UndoBuffEffect();
            buff.UnregisterCooldown();
            buffEffects.Remove(buff);

            new DestroyBuffCommand(buff, this.UniqueCreatureID).AddToQueue();
       
    }

    public void RemoveAllBuffs()
    {
        if(buffEffects != null)
        foreach (BuffEffect be in buffEffects)
        {          
            be.UndoBuffEffect();                
            be.UnregisterCooldown();
            new DestroyBuffCommand(be, this.UniqueCreatureID).AddToQueue();                         
        }
        buffEffects.Clear();
    }//RemoveAllBuffs

    public void RemoveDeBuffsAll()
    {
        int i = this.buffEffects.Count;
        int buffCounter = 0;
        for(int x = i-1; x>=0; x--)
        {
            if(this.buffEffects[x].isDebuff)
            {
                this.buffEffects[x].RemoveBuff();
                buffCounter++;
            }            
        }
    }

    public void RemoveBuffsAll()
    {
        int i = this.buffEffects.Count;
        int buffCounter = 0;
        for(int x = i-1; x>=0; x--)
        {
            if(this.buffEffects[x].isBuff)
            {
                this.buffEffects[x].RemoveBuff();
                buffCounter++;
            }            
        }
    }

    

    
    public void Heal(int amount)
    {
        ChangeHealth(amount, healFactor);
    }

    
    // //Used to compute Damage this creature takes
    // public void TakeDamage(int amount, CreatureLogic source)
    // {
    //     int damage = amount*DamageReduction;

    //     new DelayCommand(0.5f).AddToQueue();
    //     new DealDamageCommand(target.ID, poisonDamage, healthAfter: target.Health - target.ComputeDamage(poisonDamage, target)).AddToQueue();

    //     target.Health -= target.ComputeDamage(poisonDamage, target);    
    //     Debug.Log("Poison Activated" +target.UniqueCreatureID);

        
    // }  
    
    
    
    // //Used to compute Damage this creature deals
    // public int ComputeDamage(int amount, CreatureLogic target)
    // {
    //     int damage = amount*criticalFactor;

    //     if(e_IsComputeDamage!=null)
    //     e_IsComputeDamage();

    //     return damage;
    // }

    //used by attack based damage

  
    
    
    public void ChangeHealth(int changeAmount, int factor=1)
    {    
       int amount = changeAmount*factor; 

       int healthAfter = Health + amount;
       if(healthAfter > MaxHealth)
            healthAfter = MaxHealth;
            
       
      
       new DelayCommand(0.5f).AddToQueue();
       if(amount>=0)
        {
            Debug.Log("healthAfter: " + healthAfter);          
            new DealHealingCommand(this.ID, amount, healthAfter).AddToQueue();
            

        }
        else if(amount < 0)
        {           
            new DealDamageCommand(this.ID, -amount, healthAfter, Armor).AddToQueue();   
            
        }

        new UpdateHealthCommand(ID, healthAfter).AddToQueue();        
        Health += amount;
    }

    
    // STATIC For managing IDs
    public static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();

}
