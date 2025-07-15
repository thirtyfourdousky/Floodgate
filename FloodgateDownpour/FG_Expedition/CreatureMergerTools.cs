using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Floodgate.NotEnums;
using UnityEngine;

namespace Floodgate.FG_Expedition;

internal class CreatureMergerTools
{
    public static void Apply()
    {
        On.Expedition.ChallengeTools.GenerateCreatureScores += ChallengeTools_GenerateCreatureScores;

        On.Expedition.CycleScoreChallenge.CreatureKilled += CycleScoreChallenge_CreatureKilled;

        On.Expedition.GlobalScoreChallenge.CreatureKilled += GlobalScoreChallenge_CreatureKilled;
    }

    private static void GlobalScoreChallenge_CreatureKilled(On.Expedition.GlobalScoreChallenge.orig_CreatureKilled orig, Expedition.GlobalScoreChallenge self, Creature crit, int playerNumber)
    {
        if (self == null || crit == null) return;
        int currentScore = self.score;
        orig(self, crit, playerNumber);
        if (self.completed || self.game == null) return;

        if (currentScore != self.score || !Plugin.RemixOptions.LowBudgetModdedExperience.Value) return;
        CreatureTemplate.Type type = crit.abstractCreature.creatureTemplate.type;
        if (type != null && Expedition.ChallengeTools.creatureScores.ContainsKey(type.value))
        {
            int points = Mathf.FloorToInt(Expedition.ChallengeTools.creatureScores[type.value] * Mathf.Lerp(UnityEngine.Random.value, .7f, 1f));
            self.score += points;
            Expedition.ExpLog.Log("Player " + (playerNumber + 1) + " killed " + type.value + " | +" + points);
        }
        self.UpdateDescription();
        if (self.score >= self.target)
        {
            self.score = self.target;
            self.CompleteChallenge();
        }
    }

    private static void CycleScoreChallenge_CreatureKilled(On.Expedition.CycleScoreChallenge.orig_CreatureKilled orig, global::Expedition.CycleScoreChallenge self, Creature crit, int playerNumber)
    {
        if (self == null || crit == null) return;
        int currentScore = self.score;
        orig(self, crit, playerNumber);
        if (self.completed || self.game == null) return;

        if (currentScore != self.score || !Plugin.RemixOptions.LowBudgetModdedExperience.Value) return;
        CreatureTemplate.Type type = crit.abstractCreature.creatureTemplate.type;
        if (type != null && Expedition.ChallengeTools.creatureScores.ContainsKey(type.value))
        {
            int points = Mathf.FloorToInt(Expedition.ChallengeTools.creatureScores[type.value] * Mathf.Lerp(UnityEngine.Random.value, .7f, 1f));
            self.score += points;
            Expedition.ExpLog.Log("Player " + (playerNumber + 1) + " killed " + type.value + " | +" + points);
        }
        self.UpdateDescription();
        if (self.score >= self.target)
        {
            self.score = self.target;
            self.CompleteChallenge();
        }
    }

    private static void ChallengeTools_GenerateCreatureScores(On.Expedition.ChallengeTools.orig_GenerateCreatureScores orig, ref Dictionary<string, int> dict)
    {
        orig(ref dict);
        foreach (var crit in CreatureTemplateType.RegisteredTemplates)
        {
            if (dict.ContainsKey(crit.Name) && dict[crit.Name] != crit.Score)
            {
                FloodgatePatcher.CustomLog.Log("[Expedition Scores] replacing creature already in the list [" + crit.Name + "] : [" + dict[crit.Name] + "]");
            }
            dict[crit.Name] = crit.Score;
        }
    }
}
