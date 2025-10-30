using System.Text;
using Gameplay.Breeding;
using Gameplay.Enemies.Enemies;
using Gameplay.Genes;
using Gameplay.Player;
using Timeline;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace GameCycle
{
    public class StatRecorder : MonoBehaviour
    {
        private static int daysSurvived;
        private static float damageDealt;
        private static int enemyKills;
        private static int respawns;
        private static int eggsLaid;
        private static int eggsLost;
        private static int timesBreed;
        private static int genesCollected;
        private static int timesMutated;

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
        }

        private void ResetStats()
        {
            daysSurvived = 0;
            damageDealt = 0;
            enemyKills = 0;
            respawns = 0;
            eggsLaid = 0;
            eggsLost = 0;
            timesBreed = 0;
            genesCollected = 0;
            timesMutated = 0;
        }

        public static string Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Days survived").Append(": ").Append(daysSurvived).Append("\n");
            sb.Append("Damage dealt").Append(": ").Append(damageDealt.ToString("n2")).Append("\n");
            sb.Append("Enemy kills").Append(": ").Append(enemyKills).Append("\n");
            sb.Append("Respawns").Append(": ").Append(respawns).Append("\n");
            sb.Append("Eggs layed").Append(": ").Append(eggsLaid).Append("\n");
            sb.Append("Eggs lost").Append(": ").Append(eggsLost).Append("\n");
            sb.Append("Times breed").Append(": ").Append(timesBreed).Append("\n");
            sb.Append("Times mutated").Append(": ").Append(timesMutated).Append("\n");
            sb.Append("Genes collected").Append(": ").Append(genesCollected);
            return sb.ToString();
        }


        private void OnGenePickup(GeneType type, int amount) => genesCollected += amount;

        private void OnBecomePregnant() => timesBreed++;

        private void OnEggsLaid(int amount) => eggsLaid += amount;

        private void OnPlayerRespawned() => respawns++;

        private void OnDayStart(int daycounter) => daysSurvived++;

        private void OnEggStolen() => eggsLost++;

        private void DamageTakenGlobal(IDamageable damageable, DamageInstance instance)
        {
            if (damageable is IDamageableEnemy) damageDealt += instance.Damage;
        }

        private void OnMutationClick() => timesMutated++;

        private void OnEggReturn() => eggsLost--;

        private void LethalBlowGlobal(IDamageable damageable) => enemyKills++;
    }
}