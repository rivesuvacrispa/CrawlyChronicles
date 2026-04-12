using System;
using System.Text;
using Gameplay.Breeding;
using Gameplay.Enemies.Enemies;
using Gameplay.Food;
using Gameplay.Genes;
using Gameplay.Interaction;
using Gameplay.Player;
using Hitboxes;
using Timeline;
using UI.Menus;
using UnityEngine;
using Util.Enums;
using Util.Interfaces;

namespace GameCycle
{
    public class StatRecorder : MonoBehaviour
    {
        public static int DaysSurvived { get; private set; }
        public static float DamageDealt { get; private set; }
        public static int EnemyKills { get; private set; }
        public static int Respawns { get; private set; }
        public static int EggsLaid { get; private set; }
        public static int EggsLost { get; private set; }
        public static int TimesBreed { get; private set; }
        public static int GenesCollected { get; private set; }
        public static int TimesMutated { get; private set; }
        public static int ProteinEaten { get; private set; }
        public static int FungiEaten { get; private set; }
        public static int PlantsEaten { get; private set; }

        private void OnEnable()
        {
            MainMenu.OnResetRequested += ResetStats;
            
            GeneDrop.OnPickedUp += OnGenePickup;
            BreedingManager.OnBecomePregnant += OnBecomePregnant;
            BreedingManager.OnEggsLaid += OnEggsLaid;
            PlayerManager.OnPlayerRespawned += OnPlayerRespawned;
            TimeManager.OnDayStart += OnDayStart;
            AntEggStealer.OnEggStolen += OnEggStolen;
            IDamageable.OnDamageTakenGlobal += DamageTakenGlobal;
            IDamageable.OnLethalBlowGlobal += LethalBlowGlobal;
            MutationMenu.OnMutationClick += OnMutationClick;
            EggBed.OnEggReturned += OnEggReturn;
            Interactor.OnInteract += OnInteract;
        }
        
        private void OnDisable()
        {
            MainMenu.OnResetRequested -= ResetStats;
            
            GeneDrop.OnPickedUp -= OnGenePickup;
            BreedingManager.OnBecomePregnant -= OnBecomePregnant;
            BreedingManager.OnEggsLaid -= OnEggsLaid;
            PlayerManager.OnPlayerRespawned -= OnPlayerRespawned;
            TimeManager.OnDayStart -= OnDayStart;
            AntEggStealer.OnEggStolen -= OnEggStolen;
            IDamageable.OnDamageTakenGlobal -= DamageTakenGlobal;
            IDamageable.OnLethalBlowGlobal -= LethalBlowGlobal;
            MutationMenu.OnMutationClick -= OnMutationClick;
            EggBed.OnEggReturned -= OnEggReturn;
            Interactor.OnInteract -= OnInteract;
        }

        private void ResetStats()
        {
            DaysSurvived = 0;
            DamageDealt = 0;
            EnemyKills = 0;
            Respawns = 0;
            EggsLaid = 0;
            EggsLost = 0;
            TimesBreed = 0;
            GenesCollected = 0;
            TimesMutated = 0;
            ProteinEaten = 0;
            FungiEaten = 0;
            PlantsEaten = 0;
        }

        public static string Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Days survived").Append(": ").Append(DaysSurvived).Append("\n");
            sb.Append("Damage dealt").Append(": ").Append(DamageDealt.ToString("n2")).Append("\n");
            sb.Append("Enemy kills").Append(": ").Append(EnemyKills).Append("\n");
            sb.Append("Respawns").Append(": ").Append(Respawns).Append("\n");
            sb.Append("Eggs layed").Append(": ").Append(EggsLaid).Append("\n");
            sb.Append("Eggs lost").Append(": ").Append(EggsLost).Append("\n");
            sb.Append("Times breed").Append(": ").Append(TimesBreed).Append("\n");
            sb.Append("Times mutated").Append(": ").Append(TimesMutated).Append("\n");
            sb.Append("Genes collected").Append(": ").Append(GenesCollected);
            sb.Append("Food eaten").Append(": ").Append(ProteinEaten + FungiEaten + PlantsEaten);
            return sb.ToString();
        }


        private void OnGenePickup(GeneType type, int amount) => GenesCollected += amount;

        private void OnBecomePregnant() => TimesBreed++;

        private void OnEggsLaid(int amount) => EggsLaid += amount;

        private void OnPlayerRespawned() => Respawns++;

        private void OnDayStart(int daycounter) => DaysSurvived++;

        private void OnEggStolen() => EggsLost++;

        private void DamageTakenGlobal(IDamageable damageable, DamageInstance instance)
        {
            if (damageable is IDamageableEnemy) DamageDealt += instance.Damage;
        }

        private void OnMutationClick() => TimesMutated++;

        private void OnEggReturn() => EggsLost--;

        private void LethalBlowGlobal(IDamageable damageable) => EnemyKills++;
        
        private void OnInteract(IInteractable interactable)
        {
            if (interactable is not FoodObject food) return;
            
            switch (food.FoodType)
            {
                case FoodType.Protein:
                    ProteinEaten++;
                    break;
                case FoodType.Fungi:
                    FungiEaten++;
                    break;
                case FoodType.Plant:
                    PlantsEaten++;
                    break;
            }
        }
    }
}