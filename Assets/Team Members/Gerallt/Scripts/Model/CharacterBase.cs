using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainsOfFate.Gerallt
{
    public abstract class CharacterBase : MonoBehaviour, IDescriptive
    {
        public States currentState = States.NotSet;
        
        // HACK: To get player and enemies to line up in correct position when camera mode is in top down
        [SerializeField] private Vector3 topDownCameraOffset = new Vector3(-12, -12, -14);
        
        public enum States
        {
            NotSet = 0,
            Defending = 1,
            AttackingWeapon = 2,
            AttackingSpell = 3,
            Fleeing = 4,
            UsingItem = 5,
            Encouraging = 6,
            Taunting = 7
        }
        
        // /// <summary>
        // /// A move that is scheduled to be applied later on.
        // /// </summary>
        // public abstract class AppliedMove
        // {
        //     
        // }

        // public class AttackMove : AppliedMove
        // {
        //     public float totalDamage;
        // }
        
        // public class DefenceMove : AppliedMove
        // {
        //     public float blockPercentage;
        // }
        
        #region Fields
        
        /// <summary>
        /// The ID of the character. Internally generated by a GUID.
        /// No need to manually set this.
        /// </summary>
        private string id;

        private Rigidbody2D rb;
        private Transform trans;
        
        public int encouragePercent = 35;
        public int tauntPercent = 35;

        public Color representation;
        
        public List<WeaponBase> availableWeapons;
        public List<SpellBase> availableSpells;
        public List<ItemBase> availableItems;

        public HealthStat healthStat;
        public ArcanaStat arcanaStat;
        public ResolveStat resolveStat;
        public WisdomStat wisdomStat;
        public DefenseStat defenseStat;
        public StrengthStat strengthStat;
        
        [SerializeField] private string characterName;
        
        //[SerializeField] private int hp = 100;

        //[SerializeField] private int arcana = 100;

        //[SerializeField] private int resolve = 100;

        //[SerializeField] private int defense = 0;

        //[SerializeField] private int wisdom = 0;

        [SerializeField] private int speed = 0;

        //[SerializeField] private int strength = 0;

        [SerializeField] private float movementSpeed = 1.0f;

        [SerializeField] private int level = 0;

        [SerializeField] private int xp = 0;

        // /// <summary>
        // /// Schedule of moves the character has applied for their turn.
        // /// </summary>
        // [SerializeField] private List<AppliedMove> appliedMovesList = new List<AppliedMove>();

        #endregion

        #region Properties

        public string ID => id;
        
        public int MaxHealth => healthStat.maxValue;
        public int MaxArcana => arcanaStat.maxValue;
        public int MaxResolve => resolveStat.maxValue;
        public int MaxWisdom => wisdomStat.maxValue;
        public int MaxDefense => defenseStat.maxValue;
        public int MaxStrength => strengthStat.maxValue;
        
        public string CharacterName
        {
            get => characterName;
            set
            {
                characterName = value;
                RaiseStatChanged("CharacterName", value);
            }
        }
        
        public int HP
        {
            get => healthStat.Value;
            set
            {
                healthStat.Value = value;
                RaiseStatChanged("HP", value);
            }
        }

        public int Arcana
        {
            get => arcanaStat.Value;
            set
            {
                arcanaStat.Value = value;
                RaiseStatChanged("Arcana", value);
            }
        }

        public int Resolve
        {
            get => resolveStat.Value;
            set
            {
                resolveStat.Value = value;
                RaiseStatChanged("Resolve", value);
            }
        }

        public int Defense
        {
            get => defenseStat.Value;
            set
            {
                defenseStat.Value = value;
                RaiseStatChanged("Defense", value);
            }
        }

        public int Wisdom
        {
            get => wisdomStat.Value;
            set
            {
                wisdomStat.Value = value;
                RaiseStatChanged("Wisdom", value);
            }
        }

        public int Speed
        {
            get => speed;
            set
            {
                speed = value;
                RaiseStatChanged("Speed", value);
            }
        }

        public int Strength
        {
            get => strengthStat.Value;
            set
            {
                strengthStat.Value = value;
                RaiseStatChanged("Strength", value);
            }
        }

        public float MovementSpeed
        {
            get => movementSpeed;
            set
            {
                movementSpeed = value;
                RaiseStatChanged("MovementSpeed", value);
            }
        }

        public int Level
        {
            get => level;
            set
            {
                level = value;
                RaiseStatChanged("Level", value);
            }
        }
        
        public int XP
        {
            get => xp;
            set
            {
                xp = value;
                RaiseStatChanged("XP", value);
            }
        }

        #endregion

        // Unity can't serialise properties which is a shame. Because when properties change we could call RaiseStatChanged() internally.
        public delegate void StatChangeDelegate(CharacterBase character, string propertyName, object newValue);

        public event StatChangeDelegate OnStatChanged;

        public bool LevelUp(int newLevel, int maxLevel, bool debugOutput = false, bool raiseEvent = true)
        {
            int oldLevel = Level;

            List<IStat> toLevelUp = GetStatComponents(); // A generic list of stat components to level up.
            List<IStat> statsAffected = new List<IStat>(); // The stats that have had actual changes.


            string debug = "";
            if (debugOutput)
            {
                debug = "Level " + newLevel;
            }

            int s = 0;
            bool showDebug = false;
            foreach (IStat stat in toLevelUp)
            {
                if (stat.LevelUp(newLevel, maxLevel))
                {
                    // If the stat value has changed given the level up, add it to the list.
                    statsAffected.Add(stat);

                    if (debugOutput)
                    {
                        object newMaximum = stat.GetMaximum();
                        object absoluteMax = stat.GetAbsoluteMaximum();

                        debug += ((s == 0) ? " " : ", ") + "[" + stat.StatName + "] " + newMaximum + "/" + absoluteMax;
                        
                        showDebug = true;
                    }

                    s++;
                }
            }

            if (debugOutput && showDebug)
            {
                Debug.Log(debug);
            }
            
            Level = newLevel;

            if (raiseEvent)
            {
                LevelingManager.Instance.RaiseLevelUp(this, oldLevel, newLevel, maxLevel, statsAffected);
            }

            return statsAffected.Count > 0;
        }
        
        /// <summary>
        /// Apply the damage to health later when the player processed moves applied to them.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void AddDamage(int damage, CharacterBase attacker)
        {
            // Individual characters implement this differently.
        }
        
        /// <summary>
        /// Actually apply the damage to health right now.
        /// </summary>
        public virtual void ApplyDamage(int damage)
        {
            int hitPoints = HP;
            
            hitPoints -= damage;
            
            if (hitPoints < 0)
            {
                hitPoints = 0;
            }

            if (hitPoints > MaxHealth)
            {
                hitPoints = MaxHealth;
            }
            
            HP = hitPoints;
        }
        
        /// <summary>
        /// Actually apply the damage to health right now.
        /// </summary>
        public virtual void ApplyHealth(int healthChange)
        {
            int hitPoints = HP;
            
            hitPoints += healthChange;
            
            if (hitPoints < 0)
            {
                hitPoints = 0;
            }

            if (hitPoints > MaxHealth)
            {
                hitPoints = MaxHealth;
            }
            
            HP = hitPoints;
        }
        
        /// <summary>
        /// Actually apply reducing the arcana by the specified cost right now.
        /// </summary>
        public virtual void ReduceArcana(int spellCost)
        {
            int arc = Arcana;
            
            arc -= spellCost;
            
            if (arc < 0)
            {
                arc = 0;
            }

            if (arc > MaxArcana)
            {
                arc = MaxArcana;
            }
            
            Arcana = arc;
        }
        
        /// <summary>
        /// Actually apply change to the arcana right now.
        /// </summary>
        public virtual void ApplyArcana(int arcanaChange)
        {
            int arc = Arcana;
            
            arc += arcanaChange;
            
            if (arc < 0)
            {
                arc = 0;
            }

            if (arc > MaxArcana)
            {
                arc = MaxArcana;
            }
            
            Arcana = arc;
        }

        /// <summary>
        /// Actually apply change to the resolve right now.
        /// </summary>
        public virtual void ApplyResolve(int resolveChange)
        {
            int res = Resolve;
            
            res += resolveChange;
            
            if (res < 0)
            {
                res = 0;
            }

            if (res > MaxResolve)
            {
                res = MaxResolve;
            }
            
            Resolve = res;
        }
        
        /// <summary>
        /// Actually apply change to the wisdom right now.
        /// </summary>
        public virtual void ApplyWisdom(int wisdomChange)
        {
            int wis = Wisdom;
            
            wis += wisdomChange;
            
            if (wis < 0)
            {
                wis = 0;
            }

            if (wis > MaxWisdom)
            {
                wis = MaxWisdom;
            }
            
            Wisdom = wis;
        }
        
        /// <summary>
        /// Actually apply change to the strength right now.
        /// </summary>
        public virtual void ApplyStrength(int strengthChange)
        {
            int strength = Strength;
            
            strength += strengthChange;
            
            if (strength < 0)
            {
                strength = 0;
            }

            if (strength > MaxStrength)
            {
                strength = MaxStrength;
            }
            
            Strength = strength;
        }
        
        /// <summary>
        /// Actually apply change to the strength right now.
        /// </summary>
        public virtual void ApplyDefense(int defenseChange)
        {
            int defense = Defense;
            
            defense += defenseChange;
            
            if (defense < 0)
            {
                defense = 0;
            }

            if (defense > MaxDefense)
            {
                defense = MaxDefense;
            }
            
            Defense = defense;
        }
        
        public virtual void UpdatePrimaryStats()
        {
            RaiseStatChanged("ID", ID);
            RaiseStatChanged("CharacterName", CharacterName);
            RaiseStatChanged("HP", HP);
            RaiseStatChanged("Resolve", Resolve);
            RaiseStatChanged("Arcana", Arcana);
            RaiseStatChanged("Wisdom", Wisdom);
            RaiseStatChanged("XP", XP);
        }

        // public List<AppliedMove> GetMoves()
        // {
        //     return appliedMovesList;
        // }
        //
        // public void ApplyMove(AppliedMove move)
        // {
        //     appliedMovesList.Add(move);
        // }
        //
        // public void RemoveMove(AppliedMove move)
        // {
        //     appliedMovesList.Remove(move);
        // }
        //
        // public void ClearMoves()
        // {
        //     appliedMovesList.Clear();
        // }

        public string GetId()
        {
            return ID;
        }
        
        public string GetName()
        {
            return CharacterName;
        }

        public string GetDescription()
        {
            return string.Empty;
        }

        public Color GetTint()
        {
            return representation;
        }

        public List<IDescriptive> GetInventory()
        {
            List<IDescriptive> allItems = new List<IDescriptive>();
            allItems.AddRange(availableWeapons);
            allItems.AddRange(availableSpells);
            allItems.AddRange(availableItems);
            return allItems;
        }

        public void RemoveItem(IDescriptive item)
        {
            int i = 0;
            if (item is WeaponBase) // TODO: Refactor this generalising item lists into one IDescriptive list
            {
                WeaponBase weapon = item as WeaponBase;
                foreach (WeaponBase w in availableWeapons)
                {
                    if (w.ID == weapon.ID)
                    {
                        availableWeapons.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }
            else if (item is SpellBase)
            {
                SpellBase spell = item as SpellBase;
                foreach (SpellBase s in availableSpells)
                {
                    if (s.ID == spell.ID)
                    {
                        availableSpells.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }
            else if (item is ItemBase)
            {
                ItemBase itemBase = item as ItemBase;
                foreach (ItemBase it in availableItems)
                {
                    if (it.ID == itemBase.ID)
                    {
                        availableItems.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }
        }
        
        public void ResetState()
        {
            currentState = States.NotSet;
        }
        
        protected void RaiseStatChanged(string propertyName, object newValue)
        {
            OnStatChanged?.Invoke(this, propertyName, newValue);
        }

        public List<IStat> GetStatComponents()
        {
            return GetComponentsInTree<IStat>();
        }
        
        public void ResetStats()
        {
            List<IStat> stats = GetStatComponents();
            foreach (IStat stat in stats)
            {
                stat.Reset();
            }
        }
        
        public void ReplenishStats()
        {
            List<IStat> stats = GetStatComponents();
            foreach (IStat stat in stats)
            {
                stat.Replenish();
            }
        }

        public static Vector2 FlockSeparation<T>(List<T> neighbours, float minSeparationDistance, Vector2 leaderPosition) where T: MonoBehaviour
        {
            Vector2 separation = Vector2.zero;
            int neighboursCount = neighbours.Count;
            if (neighboursCount > 0)
            {
                foreach (T neighbour in neighbours)
                {
                    Vector2 nPos = neighbour.transform.position;

                    float distToNeighbour = Vector2.Distance(leaderPosition, nPos);

                    if (distToNeighbour < minSeparationDistance)
                    {
                        separation.x += nPos.x - leaderPosition.x;
                        separation.y += nPos.y - leaderPosition.y;
                    }
                }

                separation /= neighboursCount;
                    
                separation.x *= -1;
                separation.y *= -1;
            }

            return separation;
        }
        
        private void UpdateStatComponents()
        {
            GetComponent(ref healthStat);
            GetComponent(ref arcanaStat);
            GetComponent(ref resolveStat);
            GetComponent(ref wisdomStat);
            GetComponent(ref defenseStat);
            GetComponent(ref strengthStat);
        }

        /// <summary>
        /// Always gets a component either directly within the current GameObject
        /// or if that fails, gets the component that may be a child of the current game object.
        /// </summary>
        public void GetComponent<T>(ref T componentReferenceVariable)
        {
            if (componentReferenceVariable == null)
            {
                componentReferenceVariable = GetComponent<T>();

                if (componentReferenceVariable == null)
                {
                    componentReferenceVariable = GetComponentInChildren<T>();
                }
            }
        }
        
        public List<T> GetComponentsInTree<T>()
        {
            List<T> componentsList = new List<T>();

            componentsList.AddRange(GetComponents<T>());
            componentsList.AddRange(GetComponentsInChildren<T>());
            
            return componentsList;
        }
        
        public virtual void Awake()
        {
            Guid newId = Guid.NewGuid(); //TODO: Check for collisions with characters that by pure unluck might have the same GUID.
            id = newId.ToString();

            // Get stat components and store them as variables on the current character.
            UpdateStatComponents();
            
            // Automatically add items, weapons, spells to their respective lists.
            availableItems.Clear();
            availableSpells.Clear();
            availableWeapons.Clear();
            
            availableItems.AddRange(GetComponentsInTree<ItemBase>());
            availableSpells.AddRange(GetComponentsInTree<SpellBase>());
            availableWeapons.AddRange(GetComponentsInTree<WeaponBase>());
            
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            trans = GetComponent<Transform>();
        }

        private Vector3 spawn;
        
        void Start()
        {
            spawn = transform.position;
        }
        
        public void FixedUpdate()
        {
            gameObject.SetActive(false);
            Vector3 oldPos = transform.position;
            oldPos.z = GameManager.Instance.spawnZ; // HACK: Changed by DebugUI when camera mode changes

            
            
            Vector3 pos = Camera.main.transform.position;
            
            Vector3 sp = GetComponent<Transform>().position;
            
            pos.x = sp.x; //test.x;
            pos.y = 0;
            pos.z = -15;

            Camera.main.transform.position = pos;
            
            
            if (GameManager.Instance.cameraMode == GameManager.CameraMode.TopDown)
            {
                // CHECK: Test if top down camera mode works better with this offset 

                //oldPos.z = GameManager.Instance.spawnPerspectiveZ;
                
                //oldPos += topDownCameraOffset; // HACK: To get player and enemies to line up in correct position when camera mode is in top down
            }
            else
            {

            }
            

            rb.velocity = Vector2.zero; // HACK: Cancel any unwanted velocities!
        }
    }
}