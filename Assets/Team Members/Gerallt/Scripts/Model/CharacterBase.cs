using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class CharacterBase : MonoBehaviour
    {
        /// <summary>
        /// A move that is scheduled to be applied later on.
        /// </summary>
        public abstract class AppliedMove
        {
            
        }
        
        public class DefenceMove : AppliedMove
        {
            public float blockPercentage;
        }
        
        #region Fields

        public int maxHealth = 100;
        public int maxArcana = 100;
        public int maxResolve = 100;

        public Color representation;
        
        [SerializeField] private string characterName;
        
        [SerializeField] private int hp = 100;

        [SerializeField] private int arcana = 100;

        [SerializeField] private int resolve = 0;

        [SerializeField] private int defense = 0;

        [SerializeField] private int wisdom = 0;

        [SerializeField] private int speed = 0;

        [SerializeField] private int strength = 0;

        [SerializeField] private float movementSpeed = 1.0f;

        [SerializeField] private int level = 0;

        /// <summary>
        /// Schedule of moves the character has applied for their turn.
        /// </summary>
        [SerializeField] private List<AppliedMove> appliedMovesList = new List<AppliedMove>();

        #endregion

        #region Properties

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
            get => hp;
            set
            {
                hp = value;
                RaiseStatChanged("HP", value);
            }
        }

        public int Arcana
        {
            get => arcana;
            set
            {
                arcana = value;
                RaiseStatChanged("Arcana", value);
            }
        }

        public int Resolve
        {
            get => resolve;
            set
            {
                resolve = value;
                RaiseStatChanged("Resolve", value);
            }
        }

        public int Defense
        {
            get => defense;
            set
            {
                defense = value;
                RaiseStatChanged("Defense", value);
            }
        }

        public int Wisdom
        {
            get => wisdom;
            set
            {
                wisdom = value;
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
            get => strength;
            set
            {
                strength = value;
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

        #endregion

        // Unity can't serialise properties which is a shame. Because when properties change we could call RaiseStatChanged() internally.
        public delegate void StatChangeDelegate(CharacterBase character, string propertyName, object newValue);

        public event StatChangeDelegate OnStatChanged;

        public virtual void ApplyDamage(int damage)
        {
            int hitPoints = HP;
            
            hitPoints -= damage;

            if (hitPoints < 0)
            {
                hitPoints = 0;
            }

            HP = hitPoints;
        }

        public void UpdatePrimaryStats()
        {
            RaiseStatChanged("CharacterName", CharacterName);
            RaiseStatChanged("HP", HP);
            RaiseStatChanged("Resolve", Resolve);
            RaiseStatChanged("Arcana", Arcana);
        }

        public List<AppliedMove> GetMoves()
        {
            return appliedMovesList;
        }
        
        public void ApplyMove(AppliedMove move)
        {
            appliedMovesList.Add(move);
        }

        public void RemoveMove(AppliedMove move)
        {
            appliedMovesList.Remove(move);
        }
        
        public void ClearMoves()
        {
            appliedMovesList.Clear();
        }

        private void RaiseStatChanged(string propertyName, object newValue)
        {
            OnStatChanged?.Invoke(this, propertyName, newValue);
        }
        
        static internal Color RandomColour()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        private void Awake()
        {
            //representation = RandomColour();
        }
    }
}