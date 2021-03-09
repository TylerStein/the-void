using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LexiconRule
{
    public static string leftSideDelim = "<";
    public static string rightSideDelim = ">";
    public static string handleOrDelim = "|";
    public static string anySymbol = "*";

    [SerializeField] public string targetHandle = default;
    [SerializeField] public string[] leftSide = new string[0];
    [SerializeField] public string[] rightSide = new string[0];
    [SerializeField] public bool isValid;

    public static LexiconRule Parse(string rule) {
        int leftSideIndex = rule.IndexOf(leftSideDelim);
        string leftSide = anySymbol;
        if (leftSideIndex != -1) {
            leftSide = rule.Substring(0, leftSideIndex).Trim();
        } else {
            leftSideIndex = 0;
        }

        string[] leftSideHandles = leftSide.Split(' ');

        int rightSideIndex = rule.IndexOf(rightSideDelim);
        string rightSide = rule.Substring(rightSideIndex + 1).Trim();
        string[] rightSideHandles = rightSide.Split(' ');

        string middle = rule.Substring(leftSideIndex, rule.Length - rightSideIndex - 1).Trim();

        return new LexiconRule() {
            targetHandle = middle,
            leftSide = leftSideHandles,
            rightSide = rightSideHandles,
            isValid = middle.Length > 0,
        };
    }
}

[CreateAssetMenu(fileName = "New Generator Lexicon", menuName = "Level Generation/Lexicon")]
public class GeneratorLexicon : ScriptableObject
{

    [Tooltip("List of lexicon rules, see documentation")]
    [SerializeField] public string[] ruleStrings = new string[0];

    public Dictionary<string, List<LexiconRule>> beatRules;

    public void Initialize() {
        beatRules = new Dictionary<string, List<LexiconRule>>();
        foreach (string rule in ruleStrings) {
            LexiconRule lexRule = LexiconRule.Parse(rule);
            if (lexRule.isValid) {
                if (beatRules.ContainsKey(lexRule.targetHandle)) {
                    beatRules[lexRule.targetHandle].Add(lexRule);
                } else {
                    beatRules.Add(lexRule.targetHandle, new List<LexiconRule>() { lexRule });
                }
            }
        }
    }

    public bool CheckRule(string[] beatHistory, string nextBeat) {
        // allow any on empty history
        if (beatHistory.Length == 0) return true;
        string lastBeat = beatHistory[beatHistory.Length - 1];

        // check if this beat is in the lexicon
        if (beatRules.ContainsKey(nextBeat)) {
            List<LexiconRule> rules = beatRules[nextBeat];
            for (int i = 0; i < rules.Count; i++) {
                if (rules[i].leftSide.Length > 0 && rules[i].leftSide[0] != LexiconRule.anySymbol) {
                    // check left side
                    for (int j = 0; j < rules[i].leftSide.Length; j++) {
                        if (rules[i].leftSide[j] == lastBeat) return true;
                    }
                }
            }

            return true;
        } else {
            return true;
        }
    }
}
