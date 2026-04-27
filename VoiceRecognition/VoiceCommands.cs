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

    [Flags]
    public enum VoiceActionSupport
    {
        None   = 0,
        Pass   = 1 << 0,
        Fail   = 1 << 1,
        RePass = 1 << 2,
        ReFail = 1 << 3,
        Clear  = 1 << 4,

        FullEOL  = Pass | Fail | RePass | ReFail | Clear,
        FailOnly = Fail | ReFail | Clear,
    }

    public interface IVoiceEnabledForm
    {
        VoiceActionSupport SupportedActions { get; }
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

        /// <summary>
        /// Phân tích một câu thành NHIỀU lệnh (token-by-token).
        /// VD: "A 11 21 82" → [Part:A, Error:11, Error:21, Error:82]
        /// </summary>
        public static IReadOnlyList<VoiceCommandMatch> ParseAll(
            string input,
            IEnumerable<VoiceCommandDefinition> definitions,
            double threshold = 0.80)
        {
            var results = new List<VoiceCommandMatch>();
            var defs = definitions?.Where(d => d != null).ToList() ?? new List<VoiceCommandDefinition>();
            if (string.IsNullOrWhiteSpace(input) || defs.Count == 0) return results;

            var parts   = defs.Where(d => d.Kind == VoiceCommandKind.Part).ToList();
            var errors  = defs.Where(d => d.Kind == VoiceCommandKind.Error).ToList();

            string normalizedFull = VoiceTextNormalizer.Normalize(input);
            string[] tokens = SplitNormalizedTokens(normalizedFull);

            var usedIdx = new HashSet<int>();

            // Bước 1: Tìm part từ từng token riêng lẻ (ưu tiên token khớp nhất)
            int bestPartIdx   = -1;
            double bestPartSc = 0;
            VoiceCommandMatch bestPartMatch = null;

            for (int i = 0; i < tokens.Length; i++)
            {
                var (def, phrase, score) = FindBestFromToken(tokens[i], parts);
                if (def != null && score >= threshold && score > bestPartSc)
                {
                    bestPartSc    = score;
                    bestPartIdx   = i;
                    bestPartMatch = new VoiceCommandMatch
                    {
                        RecognizedText = input,
                        Kind           = VoiceCommandKind.Part,
                        PartCode       = def.Code,
                        MatchedCommand = phrase,
                        ConfidenceScore = score
                    };
                }
            }

            if (bestPartMatch != null) { results.Add(bestPartMatch); usedIdx.Add(bestPartIdx); }

            // Match multi-token error aliases before single-token codes.
            for (int i = 0; i < tokens.Length; i++)
            {
                if (usedIdx.Contains(i)) continue;
                var (windowDef, windowPhrase, windowScore, windowLength) = FindBestFromTokenWindow(tokens, i, errors, usedIdx);
                if (windowDef != null && windowScore >= threshold)
                {
                    results.Add(new VoiceCommandMatch
                    {
                        RecognizedText  = input,
                        Kind            = VoiceCommandKind.Error,
                        ErrorCode       = windowDef.Code,
                        MatchedCommand  = windowPhrase,
                        ConfidenceScore = windowScore
                    });

                    for (int j = i; j < i + windowLength; j++)
                    {
                        usedIdx.Add(j);
                    }

                    i += windowLength - 1;
                }
            }

            // Bước 2: Tìm tất cả error từ các token còn lại
            for (int i = 0; i < tokens.Length; i++)
            {
                if (usedIdx.Contains(i)) continue;
                var (def, phrase, score) = FindBestFromToken(tokens[i], errors);
                if (def != null && score >= threshold)
                {
                    results.Add(new VoiceCommandMatch
                    {
                        RecognizedText  = input,
                        Kind            = VoiceCommandKind.Error,
                        ErrorCode       = def.Code,
                        MatchedCommand  = phrase,
                        ConfidenceScore = score
                    });
                    usedIdx.Add(i);
                }
            }

            // Bước 3: Không có kết quả → fallback Parse() đơn (action, composite từ cụm từ)
            if (results.Count == 0)
            {
                var single = Parse(input, defs, threshold);
                if (single.IsSuccess) results.Add(single);
            }

            return results;
        }

        private static string[] SplitNormalizedTokens(string normalizedText)
        {
            if (string.IsNullOrWhiteSpace(normalizedText))
            {
                return Array.Empty<string>();
            }

            var tokens = new List<string>();
            foreach (string rawToken in normalizedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (rawToken.Length <= 1)
                {
                    tokens.Add(rawToken);
                    continue;
                }

                var current = new StringBuilder();
                bool? currentIsDigit = null;

                foreach (char ch in rawToken)
                {
                    bool isDigit = char.IsDigit(ch);
                    if (current.Length > 0 && currentIsDigit.HasValue && currentIsDigit.Value != isDigit)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }

                    current.Append(ch);
                    currentIsDigit = isDigit;
                }

                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                }
            }

            return tokens.ToArray();
        }

        private static (VoiceCommandDefinition definition, string phrase, double score, int length) FindBestFromTokenWindow(
            string[] tokens,
            int startIndex,
            IEnumerable<VoiceCommandDefinition> definitions,
            ISet<int> usedIdx)
        {
            string bestPhrase = null;
            VoiceCommandDefinition bestDef = null;
            int bestLength = 0;

            foreach (var def in definitions)
            {
                foreach (string phrase in def.AllPhrases())
                {
                    string norm = VoiceTextNormalizer.Normalize(phrase);
                    string[] phraseTokens = SplitNormalizedTokens(norm);
                    if (phraseTokens.Length <= 1 || startIndex + phraseTokens.Length > tokens.Length)
                    {
                        continue;
                    }

                    bool overlapsUsedToken = false;
                    for (int i = startIndex; i < startIndex + phraseTokens.Length; i++)
                    {
                        if (usedIdx.Contains(i))
                        {
                            overlapsUsedToken = true;
                            break;
                        }
                    }

                    if (overlapsUsedToken)
                    {
                        continue;
                    }

                    bool exactMatch = true;
                    for (int i = 0; i < phraseTokens.Length; i++)
                    {
                        if (!string.Equals(tokens[startIndex + i], phraseTokens[i], StringComparison.Ordinal))
                        {
                            exactMatch = false;
                            break;
                        }
                    }

                    if (!exactMatch)
                    {
                        continue;
                    }

                    if (phraseTokens.Length > bestLength)
                    {
                        bestPhrase = phrase;
                        bestDef = def;
                        bestLength = phraseTokens.Length;
                    }
                }
            }

            return (bestDef, bestPhrase, bestDef == null ? 0 : 1.0, bestLength);
        }

        // Match một token đơn lẻ (đã normalize) với danh sách lệnh.
        // Chỉ xét phrase 1 từ để tránh "hai mươi tám" match với token "hai".
        private static (VoiceCommandDefinition definition, string phrase, double score) FindBestFromToken(
            string token,
            IEnumerable<VoiceCommandDefinition> definitions)
        {
            double bestScore = 0;
            string bestPhrase = null;
            VoiceCommandDefinition bestDef = null;

            foreach (var def in definitions)
            {
                foreach (string phrase in def.AllPhrases())
                {
                    string norm = VoiceTextNormalizer.Normalize(phrase);
                    if (norm.Contains(' ')) continue; // bỏ qua phrase nhiều từ

                    double score = token == norm ? 1.0 : VoiceTextNormalizer.JaroWinkler(token, norm);

                    if (score > bestScore)
                    {
                        bestScore  = score;
                        bestPhrase = phrase;
                        bestDef    = def;
                    }
                }
            }

            return (bestDef, bestPhrase, bestScore);
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

            var replacements = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(" loi lai ", " re fail "),
                new KeyValuePair<string, string>(" lam lai ", " re fail "),
                new KeyValuePair<string, string>(" dat ", " pass "),
                new KeyValuePair<string, string>(" dat hang ", " pass "),
                new KeyValuePair<string, string>(" qua ", " pass "),
                new KeyValuePair<string, string>(" hong ", " fail "),
                new KeyValuePair<string, string>(" hu ", " fail "),
                new KeyValuePair<string, string>(" xoa ", " clear "),
                new KeyValuePair<string, string>(" huy ", " clear "),
                new KeyValuePair<string, string>(" mot ", " 1 "),
                new KeyValuePair<string, string>(" hai ", " 2 "),
                new KeyValuePair<string, string>(" ba ", " 3 "),
                new KeyValuePair<string, string>(" bon ", " 4 "),
                new KeyValuePair<string, string>(" nam ", " 5 "),
                new KeyValuePair<string, string>(" sau ", " 6 "),
                new KeyValuePair<string, string>(" bay ", " 7 "),
                new KeyValuePair<string, string>(" tam ", " 8 "),
                new KeyValuePair<string, string>(" chin ", " 9 "),
                // "mươi" đầy đủ
                // "mùi" — Whisper hay nhầm "mười" thành "mùi" trong môi trường ồn
                new KeyValuePair<string, string>(" mui ", " 10 "),
                // "muời", "mưoi" — các biến thể unicode khác
                new KeyValuePair<string, string>(" muoi ", " 10 "),
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

    public static class VoiceAliasHelper
    {
        // Số → tiếng Việt (1–82, bao phủ toàn bộ REASON_ID trong hệ thống)
        public static readonly IReadOnlyDictionary<string, string> NumberWords =
            new Dictionary<string, string>
            {
                {"1","một"},   {"2","hai"},   {"3","ba"},    {"4","bốn"},  {"5","năm"},
                {"6","sáu"},   {"7","bảy"},   {"8","tám"},   {"9","chín"}, {"10","mười"},
                {"11","mười một"},   {"12","mười hai"},  {"13","mười ba"},
                {"14","mười bốn"},   {"15","mười lăm"},  {"16","mười sáu"},
                {"17","mười bảy"},   {"18","mười tám"},  {"19","mười chín"},
                {"20","hai mươi"},
                {"21","hai mươi mốt"}, {"22","hai mươi hai"}, {"23","hai mươi ba"},
                {"24","hai mươi bốn"}, {"25","hai mươi lăm"}, {"26","hai mươi sáu"},
                {"27","hai mươi bảy"}, {"28","hai mươi tám"}, {"29","hai mươi chín"},
                {"30","ba mươi"},
                {"31","ba mươi mốt"}, {"32","ba mươi hai"}, {"33","ba mươi ba"},
                {"34","ba mươi bốn"}, {"35","ba mươi lăm"}, {"36","ba mươi sáu"},
                {"37","ba mươi bảy"}, {"38","ba mươi tám"}, {"39","ba mươi chín"},
                {"40","bốn mươi"},
                {"41","bốn mươi mốt"}, {"42","bốn mươi hai"}, {"43","bốn mươi ba"},
                {"44","bốn mươi bốn"}, {"45","bốn mươi lăm"}, {"46","bốn mươi sáu"},
                {"47","bốn mươi bảy"}, {"48","bốn mươi tám"}, {"49","bốn mươi chín"},
                {"50","năm mươi"},
                {"51","năm mươi mốt"}, {"52","năm mươi hai"}, {"53","năm mươi ba"},
                {"54","năm mươi bốn"}, {"55","năm mươi lăm"}, {"56","năm mươi sáu"},
                {"57","năm mươi bảy"}, {"58","năm mươi tám"}, {"59","năm mươi chín"},
                {"60","sáu mươi"},
                {"61","sáu mươi mốt"}, {"62","sáu mươi hai"}, {"63","sáu mươi ba"},
                {"64","sáu mươi bốn"}, {"65","sáu mươi lăm"}, {"66","sáu mươi sáu"},
                {"67","sáu mươi bảy"}, {"68","sáu mươi tám"}, {"69","sáu mươi chín"},
                {"70","bảy mươi"},
                {"71","bảy mươi mốt"}, {"72","bảy mươi hai"}, {"73","bảy mươi ba"},
                {"74","bảy mươi bốn"}, {"75","bảy mươi lăm"}, {"76","bảy mươi sáu"},
                {"77","bảy mươi bảy"}, {"78","bảy mươi tám"}, {"79","bảy mươi chín"},
                {"80","tám mươi"},
                {"81","tám mươi mốt"}, {"82","tám mươi hai"},
            };

        // Cách phát âm chữ cái theo kiểu người Việt trong xưởng
        // Dùng làm ExtraAliases trong GetPartLabels() của từng form
        public static readonly IReadOnlyDictionary<string, string[]> LetterViAliases =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "A", new[] { "a", "à", "ay", "ei" } },
                { "B", new[] { "bê", "bờ", "bi" } },
                { "C", new[] { "xê", "cờ", "xi", "xê" } },
                { "D", new[] { "dê", "dờ", "đi", "đê", "đờ" } },
                { "E", new[] { "ê", "e" } },
                { "F", new[] { "ép phờ", "ép", "ep" } },
                { "G", new[] { "giê", "gờ" } },
                { "H", new[] { "hắt", "hờ" } },
                { "I", new[] { "i", "i ngắn" } },
                { "J", new[] { "gi", "jê" } },
                { "K", new[] { "ca", "ká" } },
                { "L", new[] { "lờ", "elờ" } },
                { "M", new[] { "mờ", "em mờ" } },
                { "N", new[] { "nờ", "en nờ" } },
                { "O", new[] { "o", "ô" } },
                { "P", new[] { "pê", "pờ" } },
                { "Q", new[] { "quê", "cu" } },
                { "R", new[] { "rờ", "ờờ" } },
                { "S", new[] { "sờ", "ét xì" } },
                { "T", new[] { "tê", "tờ" } },
                { "U", new[] { "u" } },
                { "V", new[] { "vê", "vờ" } },
                { "W", new[] { "vê kép", "w" } },
                { "X", new[] { "xờ", "ích xì" } },
                { "Y", new[] { "y", "i dài" } },
                { "Z", new[] { "dét", "zét" } },
            };

        // Alias tiếng Việt cho các lỗi phổ biến (theo REASON_ID)
        // Thêm dần khi test thực tế
        private static readonly IReadOnlyDictionary<string, string[]> ErrorVnAliases =
            new Dictionary<string, string[]>
            {
                { "5",  new[] { "khác màu" } },
                { "10", new[] { "chỉ thừa", "đứt chỉ" } },
                { "12", new[] { "nhăn", "cộm" } },
                { "17", new[] { "lem keo", "lem nước" } },
                { "18", new[] { "hở keo" } },
                { "21", new[] { "vệ sinh", "dơ" } },
                { "25", new[] { "kim loại" } },
                { "34", new[] { "hở keo nhỏ" } },
                { "35", new[] { "hở keo lớn" } },
                { "40", new[] { "chỉ thừa" } },
                { "41", new[] { "may sụp mí", "đứt chỉ" } },
                { "42", new[] { "nhăn upper", "nhăn lót" } },
            };


        /// <summary>
        /// Tạo danh sách alias cho một mã lỗi. Dùng chung cho tất cả form EOL.
        /// </summary>
        public static IReadOnlyCollection<string> BuildReasonAliases(string reasonCode, string reasonText)
        {
            var aliases = new List<string>
            {
                reasonCode,
                "lỗi " + reasonCode,
                "mã lỗi " + reasonCode,
                reasonText,
                "lỗi " + reasonText,
            };

            // Dạng đầy đủ: "hai mươi tám"
            if (NumberWords.TryGetValue(reasonCode, out string word))
            {
                aliases.Add(word);
                aliases.Add("lỗi " + word);

                // Dạng nuốt chữ (bỏ "mươi"): "hai tám"
                string shortForm = GetShortNumberWord(reasonCode);
                if (shortForm != null)
                {
                    aliases.Add(shortForm);
                    aliases.Add("lỗi " + shortForm);
                }
            }

            if (ErrorVnAliases.TryGetValue(reasonCode, out string[] vnAliases))
            {
                aliases.AddRange(vnAliases);
                foreach (string alias in vnAliases)
                    aliases.Add("lỗi " + alias);
            }

            return aliases;
        }

        // Tạo dạng nuốt chữ "mươi" cho số 2 chữ số (21–82)
        // VD: 28 → "hai tám", 35 → "ba lăm"
        private static string GetShortNumberWord(string code)
        {
            if (!int.TryParse(code, out int n) || n < 21 || n > 82) return null;
            int tens = n / 10;
            int units = n % 10;
            if (units == 0) return null; // 20, 30... không nuốt

            string[] tensW  = { "", "", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám" };
            // Vị trí đơn vị trong số ghép: 1→mốt, 5→lăm
            string[] unitsW = { "", "mốt", "hai", "ba", "bốn", "lăm", "sáu", "bảy", "tám", "chín" };

            if (tens < 2 || tens > 8 || units < 1 || units > 9) return null;
            return tensW[tens] + " " + unitsW[units];
        }

        /// <summary>
        /// Tạo action commands theo cờ SupportedActions của mỗi form.
        /// </summary>
        public static IEnumerable<VoiceCommandDefinition> BuildActionCommands(VoiceActionSupport supported)
        {
            if (supported.HasFlag(VoiceActionSupport.Pass))
            {
                yield return new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Action,
                    ActionType = "pass",
                    DisplayText = "đạt",
                    Aliases = new[] { "pass", "đạt", "qua", "ok", "ô kê", "okay" }
                };
            }

            if (supported.HasFlag(VoiceActionSupport.Fail))
            {
                yield return new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Action,
                    ActionType = "fail",
                    DisplayText = "lỗi",
                    Aliases = new[] { "fail", "lỗi", "không đạt", "ng" }
                };
            }

            if (supported.HasFlag(VoiceActionSupport.RePass))
            {
                yield return new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Action,
                    ActionType = "re-pass",
                    DisplayText = "đạt lại",
                    Aliases = new[] { "re pass", "đạt lại", "kiểm lại đạt" }
                };
            }

            if (supported.HasFlag(VoiceActionSupport.ReFail))
            {
                yield return new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Action,
                    ActionType = "re-fail",
                    DisplayText = "lỗi lại",
                    Aliases = new[] { "re fail", "lỗi lại", "kiểm lại lỗi" }
                };
            }

            if (supported.HasFlag(VoiceActionSupport.Clear))
            {
                yield return new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Action,
                    ActionType = "clear",
                    DisplayText = "xóa",
                    Aliases = new[] { "xóa", "hủy", "clear", "bỏ chọn" }
                };
            }
        }
    }
}
