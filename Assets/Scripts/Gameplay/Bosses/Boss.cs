using System.Collections;
using Definitions;
using Gameplay.Genes;
using Gameplay.Map;
using Timeline;
using UI.Menus;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Bosses
{
    public abstract class Boss : MonoBehaviour, IDestructionEventProvider
    {
        [SerializeField] private Scriptable.Boss scriptable;

        public Scriptable.Boss Scriptable => scriptable;

        private bool destructionInvoked;


        
        // Abstract methods
        protected abstract void Enrage();

        
        
        // Virtual methods
        protected virtual void Start()
        {
            Bossbar.Instance.SetName(Scriptable.Name);
            Bossbar.Instance.SetActive(true);
        }
        protected virtual void OnEnable() => SubToEvents();

        protected virtual void OnDisable() => UnsubFromEvents();

        protected virtual void OnDestroy() => InvokeDestructionEvent();
        public virtual void SetLocation(Vector3 pos) => transform.position = pos;
        public virtual void Flee() => InvokeDestructionEvent();
        
        protected virtual void Die()
        {
            InvokeDestructionEvent();
            OnDefeat();
        }


        // Private methods
        private void OnDayStart(int _)
        {
            switch (SettingsMenu.SelectedDifficulty.OverallDifficulty)
            {
                case OverallDifficulty.Affordable:
                    Flee();
                    break;
                case OverallDifficulty.Cruel:
                    Enrage();
                    break;
            }
        }
        
        private void OnDefeat()
        {
            TimeManager.Instance.ResetLifespan();
            Vector3 pos = MapManager.MapCenter.position;
            StartCoroutine(GeneRewardRoutine(pos));
            GlobalDefinitions.CreateMutationDrop(pos, Scriptable.MutationReward);
        }

        private IEnumerator GeneRewardRoutine(Vector3 pos)
        {
            int geneDropsAmount = Random.Range(10, 15);
            int reward = Scriptable.GenesReward;
            reward = (int) (reward * Mathf.Clamp((int) SettingsMenu.SelectedDifficulty.OverallDifficulty, 0.75f, 1.25f));
            int eachGeneAmount = reward / geneDropsAmount;
            float delay = 2f / geneDropsAmount;
            for (int i = geneDropsAmount; i > 0; i--)
            {
                GlobalDefinitions.DropGenesRandomly(pos, (GeneType)Random.Range(0, 3), eachGeneAmount);
                yield return new WaitForSeconds(delay);
            }
        }

        private void OnPlayerKilled()
        {
            if (SettingsMenu.SelectedDifficulty.OverallDifficulty == OverallDifficulty.Cruel) Flee();
        }
        
        
        // Events
        private void OnResetRequested() => Destroy(gameObject);

        private void SubToEvents()
        {
            TimeManager.OnDayStart += OnDayStart;
            Player.PlayerManager.OnPlayerKilled += OnPlayerKilled;
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void UnsubFromEvents()
        {
            TimeManager.OnDayStart -= OnDayStart;
            Player.PlayerManager.OnPlayerKilled -= OnPlayerKilled;
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        protected virtual bool InvokeDestructionEvent()
        {
            if(destructionInvoked) return false;
            UnsubFromEvents();
            Bossbar.Instance.Die();
            destructionInvoked = true;
            OnProviderDestroy?.Invoke(this);
            return true;
        }
        
        public virtual event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
    }
}