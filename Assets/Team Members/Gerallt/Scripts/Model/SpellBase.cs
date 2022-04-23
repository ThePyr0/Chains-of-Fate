using System;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class SpellBase : MonoBehaviour, IDescriptive
    {
        #region Fields
        
        /// <summary>
        /// The ID of the spell. Internally generated by a GUID.
        /// No need to manually set this.
        /// </summary>
        private string id;

        [SerializeField] private string spellName;
        [SerializeField] private string spellDescription;
        [SerializeField] private int baseDamage;
        [SerializeField] private float spellCost; // Acana cost

        #endregion

        #region Properties

        public string ID => id;

        public string SpellName
        {
            get => spellName;
            set
            {
                spellName = value;
                RaiseStatChanged("SpellName", value);
            }
        }
        
        public int BaseDamage
        {
            get => baseDamage;
            set
            {
                baseDamage = value;
                RaiseStatChanged("BaseDamage", value);
            }
        }
        
        public float SpellCost
        {
            get => spellCost;
            set
            {
                spellCost = value;
                RaiseStatChanged("SpellCost", value);
            }
        }
        
        public string SpellDescription
        {
            get => spellDescription;
            set
            {
                spellDescription = value;
                RaiseStatChanged("SpellDescription", value);
            }
        }

        #endregion

        // Unity can't serialise properties which is a shame. Because when properties change we could call RaiseStatChanged() internally.
        public delegate void StatChangeDelegate(SpellBase weapon, string propertyName, object newValue);

        public event StatChangeDelegate OnStatChanged;

        public virtual void UpdatePrimaryStats()
        {
            RaiseStatChanged("ID", ID);
            RaiseStatChanged("SpellName", SpellName);
            RaiseStatChanged("BaseDamage", BaseDamage);
            RaiseStatChanged("SpellCost", SpellCost);
            RaiseStatChanged("SpellDescription", SpellDescription);
        }

        protected void RaiseStatChanged(string propertyName, object newValue)
        {
            OnStatChanged?.Invoke(this, propertyName, newValue);
        }

        public string GetId()
        {
            return ID;
        }
        
        public string GetName()
        {
            return SpellName;
        }

        public string GetDescription()
        {
            return SpellDescription;
        }
        
        public virtual void Awake()
        {
            Guid newId = Guid.NewGuid(); //TODO: Check for collisions with spells that by pure unluck might have the same GUID.
            id = newId.ToString();
        }
    }
}