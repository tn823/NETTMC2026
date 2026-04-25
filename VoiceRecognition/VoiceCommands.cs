using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable

namespace NETTMC.VoiceRecognition
{
    public enum VoiceCommandKind
    {
        Unknown,
        Part,
        Error,
        Action,
        Composite
    }

    public class VoiceCommandDefinition
    {
        public VoiceCommandKind Kind { get; set; }
        public string Code { get; set; }
        public string DisplayText { get; set; }
        public string ActionType { get; set; }
        public IReadOnlyCollection<string> Aliases { get; set; } = Array.Empty<string>();

        public IEnumerable<string> AllPhrases()
        {
            if (!string.IsNullOrWhiteSpace(DisplayText))
            {
                yield return DisplayText;
            }

            if (!string.IsNullOrWhiteSpace(Code))
            {
                yield return Code;
            }

            foreach (string alias in Aliases ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    yield return alias;
                }
            }
        }
    }

    public class VoiceCommandMatch
    {
        public string RecognizedText { get; set; }
        public VoiceCommandKind Kind { get; set; }
        public string PartCode { get; set; }
        public string ErrorCode { get; set; }
        public string ActionType { get; set; }
        public string MatchedCommand { get; set; }
        public double ConfidenceScore { get; set; }
        public bool IsSuccess => Kind != VoiceCommandKind.Unknown && ConfidenceScore > 0;

        public string ToDisplayText()
        {
            if (Kind == VoiceCommandKind.Composite)
            {
                return $"part:{PartCode} error:{ErrorCode}";
            }

            if (Kind == VoiceCommandKind.Part)
            {
                return $"part:{PartCode}";
            }

            if (Kind == VoiceCommandKind.Error)
            {
                return $"error:{ErrorCode}";
            }

            if (Kind == VoiceCommandKind.Action)
            {
                return ActionType;
            }

            return MatchedCommand;
        }
    }

    public interface IVoiceEnabledForm
    {
        IReadOnlyCollection<VoiceCommandDefinition> BuildVoiceCommands();
        void SelectPart(string partCode);
        void SelectError(string errorCode);
        void ConfirmAction(string actionType);
    }

    public static class VoiceCommandParser
    {
        public static VoiceCommandMatch Parse(
            string input,
            IEnumerable<VoiceCommandDefinition> definitions,
            double threshold = 0.80)
        {
            var commandDefinitions = definitions?.Where(d => d != null).ToList() ?? new List<VoiceCommandDefinition>();
            if (string.IsNullOrWhiteSpace(input) || commandDefinitions.Count == 0)
            {
                return Fail(input);
            }

            var parts = commandDefinitions.Where(d => d.Kind == VoiceCommandKind.Part).ToList();
            var errors = commandDefinitions.Where(d => d.Kind == VoiceCommandKind.Error).ToList();
            var actions = commandDefinitions.Where(d => d.Kind == VoiceCommandKind.Action).ToList();

            var part = FindBest(input, parts);
            var error = FindBest(input, errors);
            bool canUseComposite = VoiceTextNormalizer.Normalize(input)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Length >= 2;

            if (canUseComposite &&
                part.definition != null &&
                error.definition != null &&
                part.score >= threshold &&
                error.score >= threshold)
            {
                return new VoiceCommandMatch
                {
                    RecognizedText = input,
                    Kind = VoiceCommandKind.Composite,
                    PartCode = part.definition.Code,
                    ErrorCode = error.definition.Code,
                    MatchedCommand = $"{part.phrase} {error.phrase}",
                    ConfidenceScore = Math.Min(part.score, error.score)
                };
            }

            var action = FindBest(input, actions);
            var bestSingle = new[] { part, error, action }
                .Where(m => m.definition != null)
                .OrderByDescending(m => m.score)
                .FirstOrDefault();

            if (bestSingle.definition == null || bestSingle.score < threshold)
            {
                return Fail(input, bestSingle.phrase, bestSingle.score);
            }

            VoiceCommandDefinition definition = bestSingle.definition;
            return new VoiceCommandMatch
            {
                RecognizedText = input,
                Kind = definition.Kind,
                PartCode = definition.Kind == VoiceCommandKind.Part ? definition.Code : null,
                ErrorCode = definition.Kind == VoiceCommandKind.Error ? definition.Code : null,
                ActionType = definition.Kind == VoiceCommandKind.Action ? definition.ActionType : null,
                MatchedCommand = bestSingle.phrase,
                ConfidenceScore = bestSingle.score
            };
        }

        private static (VoiceCommandDefinition definition, string phrase, double score) FindBest(
            string input,
            IEnumerable<VoiceCommandDefinition> definitions)
        {
            string normalizedInput = VoiceTextNormalizer.Normalize(input);
            double bestScore = 0;
            string bestPhrase = null;
            VoiceCommandDefinition bestDefinition = null;

            foreach (VoiceCommandDefinition definition in definitions)
            {
                foreach (string phrase in definition.AllPhrases())
                {
                    string normalizedPhrase = VoiceTextNormalizer.Normalize(phrase);
                    double score = VoiceTextNormalizer.JaroWinkler(normalizedInput, normalizedPhrase);

                    if (normalizedInput.Contains(normalizedPhrase))
                    {
                        score = Math.Min(score + 0.18, 1.0);
                    }

                    if (normalizedInput.Split(' ').Contains(normalizedPhrase))
                    {
                        score = Math.Min(score + 0.10, 1.0);
                    }

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPhrase = phrase;
                        bestDefinition = definition;
                    }
                }
            }

            return (bestDefinition, bestPhrase, bestScore);
        }

        private static VoiceCommandMatch Fail(string input, string nearest = null, double score = 0)
        {
            return new VoiceCommandMatch
            {
                RecognizedText = input,
                Kind = VoiceCommandKind.Unknown,
                MatchedCommand = nearest,
                ConfidenceScore = score
            };
        }
    }

    public static class VoiceTextNormalizer
    {
        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string text = RemoveDiacritics(value).ToLowerInvariant();
            var builder = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                builder.Append(char.IsLetterOrDigit(ch) ? ch : ' ');
            }

            text = " " + string.Join(" ", builder.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) + " ";

            var replacements = new Dictionary<string, string>
            {
                { " loi lai ", " re fail " },
                { " lam lai ", " re fail " },
                { " dat ", " pass " },
                { " dat hang ", " pass " },
                { " qua ", " pass " },
                { " hong ", " fail " },
                { " hu ", " fail " },
                { " xoa ", " clear " },
                { " huy ", " clear " },
                { " mot ", " 1 " },
                { " môt ", " 1 " },
                { " hai ", " 2 " },
                { " ba ", " 3 " },
                { " bon ", " 4 " },
                { " nam ", " 5 " },
                { " sau ", " 6 " },
                { " bay ", " 7 " },
                { " tam ", " 8 " },
                { " chin ", " 9 " },
                { " muoi ", " 10 " }
            };

            foreach (var replacement in replacements)
            {
                text = text.Replace(replacement.Key, replacement.Value);
            }

            return string.Join(" ", text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static double JaroWinkler(string s1, string s2)
        {
            if (s1 == s2)
            {
                return 1.0;
            }

            if (s1.Length == 0 || s2.Length == 0)
            {
                return 0.0;
            }

            int distance = Math.Max(s1.Length, s2.Length) / 2 - 1;
            if (distance < 0)
            {
                distance = 0;
            }

            bool[] s1Matches = new bool[s1.Length];
            bool[] s2Matches = new bool[s2.Length];
            int matches = 0;
            int transpositions = 0;

            for (int i = 0; i < s1.Length; i++)
            {
                int start = Math.Max(0, i - distance);
                int end = Math.Min(i + distance + 1, s2.Length);

                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j] || s1[i] != s2[j])
                    {
                        continue;
                    }

                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0)
            {
                return 0.0;
            }

            int k = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (!s1Matches[i])
                {
                    continue;
                }

                while (!s2Matches[k])
                {
                    k++;
                }

                if (s1[i] != s2[k])
                {
                    transpositions++;
                }

                k++;
            }

            double jaro = (matches / (double)s1.Length +
                           matches / (double)s2.Length +
                           (matches - transpositions / 2.0) / matches) / 3.0;

            int prefix = 0;
            for (int i = 0; i < Math.Min(Math.Min(s1.Length, s2.Length), 4); i++)
            {
                if (s1[i] == s2[i])
                {
                    prefix++;
                }
                else
                {
                    break;
                }
            }

            return jaro + prefix * 0.1 * (1 - jaro);
        }

        private static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (char ch in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(ch == 'đ' || ch == 'Đ' ? 'd' : ch);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
