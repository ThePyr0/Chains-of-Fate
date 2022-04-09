using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtons : MonoBehaviour
    {
        public PlayableCharacter playableCharacter;
        public CombatGameManager combatGameManager;
        public GameObject view;
        
        public PlayerButtonsAttackSet AttackButtonSet;
        public PlayerButtonsResolveSet ResolveButtonsSet;
        public PlayerButtonsInventorySet InventoryButtonsSet;
        public PlayerButtonsDefensiveSet DefensiveButtonsSet;
        
        public void AttackButton_OnClick()
        {
            view.SetActive(false);
            AttackButtonSet.gameObject.SetActive(true);
        }

        public void InventoryButton_OnClick()
        {
            view.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(true);
        }
        
        public void DefendButton_OnClick()
        {
            view.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(true);
        }
        
        public void ResolveButton_OnClick()
        {
            view.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(true);
        }
        
        public void FleeButton_OnClick()
        {
            Debug.Log("Flee");
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IFleeAction fleeAction = (IFleeAction)currentCharacter;
            bool canFlee;
            
            if (fleeAction != null)
            {
                canFlee = fleeAction.Flee();
                
                // If can't flee, the next challenger/enemy in the queue takes a turn via skipToNextChallenger.
                combatGameManager.FinishedTurn(currentCharacter, !canFlee); 
                combatGameManager.RaiseFleeEvent(currentCharacter, null);
            }
            else
            {
                canFlee = true;
            }
            
            if (canFlee)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
        }
        
        private void OnEnable()
        {
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }

        private void OnDisable()
        {

        }

        private void Awake()
        {
            combatGameManager.OnEnemyHavingTurn += CombatGameManager_OnEnemyHavingTurn;
            combatGameManager.OnEnemyCompletedTurn += CombatGameManager_OnEnemyCompletedTurn;
        }

        private void OnDestroy()
        {
            combatGameManager.OnEnemyHavingTurn -= CombatGameManager_OnEnemyHavingTurn;
            combatGameManager.OnEnemyCompletedTurn -= CombatGameManager_OnEnemyCompletedTurn;
        }

        private void CombatGameManager_OnEnemyHavingTurn(EnemyNPC currentAgent)
        {
            view.SetActive(false);
            
            // Also hide children views:
            AttackButtonSet.gameObject.SetActive(false);
            ResolveButtonsSet.gameObject.SetActive(false);
            InventoryButtonsSet.gameObject.SetActive(false);
            DefensiveButtonsSet.gameObject.SetActive(false);
        }
        
        private void CombatGameManager_OnEnemyCompletedTurn(EnemyNPC currentAgent)
        {
            view.SetActive(true);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}