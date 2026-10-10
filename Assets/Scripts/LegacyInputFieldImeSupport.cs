using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LegacyInputFieldImeSupport : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private bool forceDubeolsikFallback;

    public bool ForceDubeolsikFallback
    {
        get { return forceDubeolsikFallback; }
        set
        {
            if (forceDubeolsikFallback == value)
            {
                return;
            }

            forceDubeolsikFallback = value;
            if (!forceDubeolsikFallback)
            {
                ResetRawFallbackState();
            }
        }
    }

    private InputField inputField;
    private bool composing;
    private bool rawFallbackComposing;
    private string compositionBaseText = string.Empty;
    private string lastCompositionText = string.Empty;
    private string rawFallbackBaseText = string.Empty;
    private string rawFallbackKeys = string.Empty;
    private int compositionStart;
    private int compositionEnd;
    private int rawFallbackStart;
    private int rawFallbackEnd;
    private int lastValueChangedFrame = -1;
    private static readonly Dictionary<char, KeyLetter> Consonants = new Dictionary<char, KeyLetter>
    {
        { 'r', new KeyLetter(0, 1, 'ㄱ') },
        { 'R', new KeyLetter(1, 2, 'ㄲ') },
        { 's', new KeyLetter(2, 4, 'ㄴ') },
        { 'e', new KeyLetter(3, 7, 'ㄷ') },
        { 'E', new KeyLetter(4, -1, 'ㄸ') },
        { 'f', new KeyLetter(5, 8, 'ㄹ') },
        { 'a', new KeyLetter(6, 16, 'ㅁ') },
        { 'q', new KeyLetter(7, 17, 'ㅂ') },
        { 'Q', new KeyLetter(8, -1, 'ㅃ') },
        { 't', new KeyLetter(9, 19, 'ㅅ') },
        { 'T', new KeyLetter(10, 20, 'ㅆ') },
        { 'd', new KeyLetter(11, 21, 'ㅇ') },
        { 'w', new KeyLetter(12, 22, 'ㅈ') },
        { 'W', new KeyLetter(13, -1, 'ㅉ') },
        { 'c', new KeyLetter(14, 23, 'ㅊ') },
        { 'z', new KeyLetter(15, 24, 'ㅋ') },
        { 'x', new KeyLetter(16, 25, 'ㅌ') },
        { 'v', new KeyLetter(17, 26, 'ㅍ') },
        { 'g', new KeyLetter(18, 27, 'ㅎ') },
    };
    private static readonly Dictionary<char, KeyLetter> JamoConsonants = new Dictionary<char, KeyLetter>
    {
        { 'ㄱ', new KeyLetter(0, 1, 'ㄱ') },
        { 'ㄲ', new KeyLetter(1, 2, 'ㄲ') },
        { 'ㄴ', new KeyLetter(2, 4, 'ㄴ') },
        { 'ㄷ', new KeyLetter(3, 7, 'ㄷ') },
        { 'ㄸ', new KeyLetter(4, -1, 'ㄸ') },
        { 'ㄹ', new KeyLetter(5, 8, 'ㄹ') },
        { 'ㅁ', new KeyLetter(6, 16, 'ㅁ') },
        { 'ㅂ', new KeyLetter(7, 17, 'ㅂ') },
        { 'ㅃ', new KeyLetter(8, -1, 'ㅃ') },
        { 'ㅅ', new KeyLetter(9, 19, 'ㅅ') },
        { 'ㅆ', new KeyLetter(10, 20, 'ㅆ') },
        { 'ㅇ', new KeyLetter(11, 21, 'ㅇ') },
        { 'ㅈ', new KeyLetter(12, 22, 'ㅈ') },
        { 'ㅉ', new KeyLetter(13, -1, 'ㅉ') },
        { 'ㅊ', new KeyLetter(14, 23, 'ㅊ') },
        { 'ㅋ', new KeyLetter(15, 24, 'ㅋ') },
        { 'ㅌ', new KeyLetter(16, 25, 'ㅌ') },
        { 'ㅍ', new KeyLetter(17, 26, 'ㅍ') },
        { 'ㅎ', new KeyLetter(18, 27, 'ㅎ') },
    };
    private static readonly Dictionary<char, int> Vowels = new Dictionary<char, int>
    {
        { 'k', 0 }, { 'o', 1 }, { 'i', 2 }, { 'O', 3 },
        { 'j', 4 }, { 'p', 5 }, { 'u', 6 }, { 'P', 7 },
        { 'h', 8 }, { 'y', 12 }, { 'n', 13 }, { 'b', 17 },
        { 'm', 18 }, { 'l', 20 },
    };
    private static readonly Dictionary<char, int> JamoVowels = new Dictionary<char, int>
    {
        { 'ㅏ', 0 }, { 'ㅐ', 1 }, { 'ㅑ', 2 }, { 'ㅒ', 3 },
        { 'ㅓ', 4 }, { 'ㅔ', 5 }, { 'ㅕ', 6 }, { 'ㅖ', 7 },
        { 'ㅗ', 8 }, { 'ㅘ', 9 }, { 'ㅙ', 10 }, { 'ㅚ', 11 },
        { 'ㅛ', 12 }, { 'ㅜ', 13 }, { 'ㅝ', 14 }, { 'ㅞ', 15 },
        { 'ㅟ', 16 }, { 'ㅠ', 17 }, { 'ㅡ', 18 }, { 'ㅢ', 19 },
        { 'ㅣ', 20 },
    };
    private static readonly Dictionary<int, int> CompoundVowels = new Dictionary<int, int>
    {
        { PairKey(8, 0), 9 },
        { PairKey(8, 1), 10 },
        { PairKey(8, 20), 11 },
        { PairKey(13, 4), 14 },
        { PairKey(13, 5), 15 },
        { PairKey(13, 20), 16 },
        { PairKey(18, 20), 19 },
    };
    private static readonly Dictionary<int, int> CompoundFinals = new Dictionary<int, int>
    {
        { PairKey(1, 19), 3 },
        { PairKey(4, 22), 5 },
        { PairKey(4, 27), 6 },
        { PairKey(8, 1), 9 },
        { PairKey(8, 16), 10 },
        { PairKey(8, 17), 11 },
        { PairKey(8, 19), 12 },
        { PairKey(8, 25), 13 },
        { PairKey(8, 26), 14 },
        { PairKey(8, 27), 15 },
        { PairKey(17, 19), 18 },
    };
    private static readonly Dictionary<int, SplitFinal> SplitFinals = new Dictionary<int, SplitFinal>
    {
        { 3, new SplitFinal(1, 9) },
        { 5, new SplitFinal(4, 12) },
        { 6, new SplitFinal(4, 18) },
        { 9, new SplitFinal(8, 0) },
        { 10, new SplitFinal(8, 6) },
        { 11, new SplitFinal(8, 7) },
        { 12, new SplitFinal(8, 9) },
        { 13, new SplitFinal(8, 16) },
        { 14, new SplitFinal(8, 17) },
        { 15, new SplitFinal(8, 18) },
        { 18, new SplitFinal(17, 9) },
    };
    private static readonly Dictionary<int, int> FinalToInitial = new Dictionary<int, int>
    {
        { 1, 0 }, { 2, 1 }, { 4, 2 }, { 7, 3 }, { 8, 5 },
        { 16, 6 }, { 17, 7 }, { 19, 9 }, { 20, 10 }, { 21, 11 },
        { 22, 12 }, { 23, 14 }, { 24, 15 }, { 25, 16 },
        { 26, 17 }, { 27, 18 },
    };
    private static readonly char[] InitialJamo =
    {
        'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ',
        'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
    };
    private static readonly char[] VowelJamo =
    {
        'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ',
        'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'
    };
    private static readonly char[] FinalJamo =
    {
        '\0', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ',
        'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ',
        'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
    };

    private void Awake()
    {
        inputField = GetComponent<InputField>();
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void LateUpdate()
    {
        if (inputField == null || !IsSelected())
        {
            if (composing)
            {
                composing = false;
            }
            rawFallbackComposing = false;
            rawFallbackKeys = string.Empty;
            return;
        }

        Input.imeCompositionMode = IMECompositionMode.On;
        Input.compositionCursorPos = Input.mousePosition;

        string composition = Input.compositionString;
        string printableInput = PrintableInput(Input.inputString);
        bool printableHasKorean = ContainsHangul(printableInput) || ContainsCompatibilityJamo(printableInput);

        if (!string.IsNullOrEmpty(composition))
        {
            rawFallbackComposing = false;
            rawFallbackKeys = string.Empty;
            if (ForceDubeolsikFallback && (ContainsHangul(composition) || ContainsCompatibilityJamo(composition) || printableHasKorean))
            {
                composing = false;
                lastCompositionText = string.Empty;
                return;
            }

            if (!composing)
            {
                BeginComposition(printableInput);
            }
            lastCompositionText = NormalizeKoreanComposition(composition);
            PreviewComposition(lastCompositionText);
            return;
        }

        if (ForceDubeolsikFallback && !string.IsNullOrEmpty(printableInput))
        {
            if (printableHasKorean)
            {
                rawFallbackComposing = false;
                rawFallbackKeys = string.Empty;
                return;
            }

            if (LooksLikeDubeolsik(printableInput))
            {
                if (!rawFallbackComposing)
                {
                    BeginRawFallback(printableInput);
                }
                rawFallbackKeys += printableInput;
                PreviewRawFallback();
                return;
            }

            rawFallbackComposing = false;
            rawFallbackKeys = string.Empty;
        }

        if (rawFallbackComposing)
        {
            return;
        }

        if (composing)
        {
            CommitComposition(string.IsNullOrEmpty(printableInput) ? lastCompositionText : NormalizeKoreanComposition(printableInput));
            composing = false;
            lastCompositionText = string.Empty;
            return;
        }

        if (!string.IsNullOrEmpty(printableInput) && lastValueChangedFrame != Time.frameCount)
        {
            InsertText(ForceDubeolsikFallback ? NormalizeKoreanComposition(printableInput) : printableInput);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        composing = false;
        rawFallbackComposing = false;
        rawFallbackKeys = string.Empty;
        lastCompositionText = string.Empty;
        Input.imeCompositionMode = IMECompositionMode.Auto;
    }

    private void OnValueChanged(string value)
    {
        lastValueChangedFrame = Time.frameCount;
        if (!ForceDubeolsikFallback || inputField == null || string.IsNullOrEmpty(value) || !ContainsCompatibilityJamo(value))
        {
            return;
        }

        string normalized = ComposeDubeolsik(value);
        if (normalized == value)
        {
            return;
        }

        int caret = Mathf.Clamp(inputField.caretPosition + normalized.Length - value.Length, 0, normalized.Length);
        inputField.SetTextWithoutNotify(normalized);
        SetCaret(caret);
    }

    public void ResetRawFallbackState()
    {
        rawFallbackComposing = false;
        rawFallbackKeys = string.Empty;
        rawFallbackBaseText = string.Empty;
        rawFallbackStart = 0;
        rawFallbackEnd = 0;
    }

    private bool IsSelected()
    {
        return inputField.isFocused;
    }

    private void BeginComposition(string printableInput)
    {
        composing = true;
        lastCompositionText = string.Empty;
        compositionBaseText = inputField.text ?? string.Empty;
        compositionStart = Mathf.Min(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        compositionEnd = Mathf.Max(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);

        if (!string.IsNullOrEmpty(printableInput)
            && lastValueChangedFrame == Time.frameCount
            && TryRemoveJustInsertedText(printableInput))
        {
            compositionStart = Mathf.Clamp(compositionStart - printableInput.Length, 0, compositionBaseText.Length);
            compositionEnd = compositionStart;
        }
    }

    private bool TryRemoveJustInsertedText(string insertedText)
    {
        int caret = Mathf.Clamp(inputField.caretPosition, 0, compositionBaseText.Length);
        int start = caret - insertedText.Length;
        if (start < 0 || start + insertedText.Length > compositionBaseText.Length)
        {
            return false;
        }
        if (compositionBaseText.Substring(start, insertedText.Length) != insertedText)
        {
            return false;
        }

        compositionBaseText = compositionBaseText.Remove(start, insertedText.Length);
        return true;
    }

    private void BeginRawFallback(string printableInput)
    {
        rawFallbackComposing = true;
        rawFallbackKeys = string.Empty;
        rawFallbackBaseText = inputField.text ?? string.Empty;
        rawFallbackStart = Mathf.Min(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        rawFallbackEnd = Mathf.Max(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);

        int caret = Mathf.Clamp(inputField.caretPosition, 0, rawFallbackBaseText.Length);
        int start = caret - printableInput.Length;
        if (lastValueChangedFrame == Time.frameCount
            && start >= 0
            && start + printableInput.Length <= rawFallbackBaseText.Length
            && rawFallbackBaseText.Substring(start, printableInput.Length) == printableInput)
        {
            rawFallbackBaseText = rawFallbackBaseText.Remove(start, printableInput.Length);
            rawFallbackStart = start;
            rawFallbackEnd = start;
        }
    }

    private void PreviewRawFallback()
    {
        string composed = ComposeDubeolsik(rawFallbackKeys);
        string preview = ReplaceRange(rawFallbackBaseText, rawFallbackStart, rawFallbackEnd, composed);
        inputField.text = preview;
        SetCaret(rawFallbackStart + composed.Length);
    }

    private void PreviewComposition(string composition)
    {
        string preview = ReplaceRange(compositionBaseText, compositionStart, compositionEnd, composition);
        inputField.SetTextWithoutNotify(preview);
        SetCaret(compositionStart + composition.Length);
    }

    private void CommitComposition(string committedText)
    {
        string nextText = ReplaceRange(compositionBaseText, compositionStart, compositionEnd, committedText);
        inputField.text = nextText;
        SetCaret(compositionStart + committedText.Length);
    }

    private void InsertText(string text)
    {
        string current = inputField.text ?? string.Empty;
        int start = Mathf.Min(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        int end = Mathf.Max(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
        string nextText = ReplaceRange(current, start, end, text);
        inputField.text = nextText;
        SetCaret(start + text.Length);
    }

    private void SetCaret(int caret)
    {
        caret = Mathf.Clamp(caret, 0, (inputField.text ?? string.Empty).Length);
        inputField.caretPosition = caret;
        inputField.selectionAnchorPosition = caret;
        inputField.selectionFocusPosition = caret;
    }

    private string ReplaceRange(string source, int start, int end, string replacement)
    {
        source = source ?? string.Empty;
        replacement = replacement ?? string.Empty;
        start = Mathf.Clamp(start, 0, source.Length);
        end = Mathf.Clamp(end, start, source.Length);
        return source.Substring(0, start) + replacement + source.Substring(end);
    }

    private string PrintableInput(string rawInput)
    {
        if (string.IsNullOrEmpty(rawInput))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(rawInput.Length);
        for (int i = 0; i < rawInput.Length; i++)
        {
            char c = rawInput[i];
            if (c != '\b' && c != '\n' && c != '\r' && !char.IsControl(c))
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }

    private string NormalizeKoreanComposition(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        if (ContainsCompatibilityJamo(text))
        {
            return ComposeDubeolsik(text);
        }

        if (ContainsHangul(text) || !LooksLikeDubeolsik(text))
        {
            return text;
        }

        return ComposeDubeolsik(text);
    }

    private bool ContainsHangul(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if ((c >= 0xAC00 && c <= 0xD7A3) || (c >= 0x3130 && c <= 0x318F))
            {
                return true;
            }
        }
        return false;
    }

    private bool LooksLikeDubeolsik(string text)
    {
        bool hasKoreanKey = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (Consonants.ContainsKey(c) || Vowels.ContainsKey(c))
            {
                hasKoreanKey = true;
                continue;
            }
            if (!char.IsWhiteSpace(c))
            {
                return false;
            }
        }
        return hasKoreanKey;
    }

    private bool ContainsCompatibilityJamo(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (JamoConsonants.ContainsKey(c) || JamoVowels.ContainsKey(c))
            {
                return true;
            }
        }
        return false;
    }

    public static string ComposeDubeolsikForTest(string text)
    {
        return ComposeDubeolsik(text ?? string.Empty);
    }

    public static string NormalizePrintableInputForTest(string text, bool forceDubeolsikFallback)
    {
        return forceDubeolsikFallback ? ComposeDubeolsik(text ?? string.Empty) : text ?? string.Empty;
    }

    private static string ComposeDubeolsik(string text)
    {
        text = ExpandHangulSyllables(text ?? string.Empty);
        StringBuilder output = new StringBuilder(text.Length);
        int initial = -1;
        int vowel = -1;
        int final = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            KeyLetter consonant;
            int nextVowel;
            if (TryGetKoreanConsonant(c, out consonant))
            {
                if (initial < 0)
                {
                    initial = consonant.initialIndex;
                }
                else if (vowel < 0)
                {
                    FlushHangul(output, initial, vowel, final);
                    initial = consonant.initialIndex;
                    final = 0;
                }
                else if (final == 0 && consonant.finalIndex > 0)
                {
                    final = consonant.finalIndex;
                }
                else if (final > 0 && consonant.finalIndex > 0 && CompoundFinals.TryGetValue(PairKey(final, consonant.finalIndex), out int compoundFinal))
                {
                    final = compoundFinal;
                }
                else
                {
                    FlushHangul(output, initial, vowel, final);
                    initial = consonant.initialIndex;
                    vowel = -1;
                    final = 0;
                }
                continue;
            }

            if (TryGetKoreanVowel(c, out nextVowel))
            {
                if (initial < 0)
                {
                    if (vowel >= 0 && CompoundVowels.TryGetValue(PairKey(vowel, nextVowel), out int compoundVowel))
                    {
                        vowel = compoundVowel;
                    }
                    else
                    {
                        FlushHangul(output, initial, vowel, final);
                        vowel = nextVowel;
                    }
                }
                else if (vowel < 0)
                {
                    vowel = nextVowel;
                }
                else if (final == 0 && CompoundVowels.TryGetValue(PairKey(vowel, nextVowel), out int compoundVowel))
                {
                    vowel = compoundVowel;
                }
                else if (final > 0)
                {
                    int carriedInitial = -1;
                    if (SplitFinals.TryGetValue(final, out SplitFinal split))
                    {
                        final = split.remainingFinal;
                        carriedInitial = split.nextInitial;
                    }
                    else if (FinalToInitial.TryGetValue(final, out int singleInitial))
                    {
                        final = 0;
                        carriedInitial = singleInitial;
                    }

                    FlushHangul(output, initial, vowel, final);
                    initial = carriedInitial;
                    vowel = nextVowel;
                    final = 0;
                }
                else
                {
                    FlushHangul(output, initial, vowel, final);
                    initial = -1;
                    vowel = nextVowel;
                    final = 0;
                }
                continue;
            }

            FlushHangul(output, initial, vowel, final);
            initial = -1;
            vowel = -1;
            final = 0;
            output.Append(c);
        }

        FlushHangul(output, initial, vowel, final);
        return output.ToString();
    }

    private static string ExpandHangulSyllables(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        StringBuilder builder = null;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c < 0xAC00 || c > 0xD7A3)
            {
                if (builder != null)
                {
                    builder.Append(c);
                }
                continue;
            }

            if (builder == null)
            {
                builder = new StringBuilder(text.Length * 2);
                if (i > 0)
                {
                    builder.Append(text, 0, i);
                }
            }

            int syllable = c - 0xAC00;
            int final = syllable % 28;
            int vowel = (syllable / 28) % 21;
            int initial = syllable / (28 * 21);
            builder.Append(InitialJamo[initial]);
            builder.Append(VowelJamo[vowel]);
            AppendFinalJamo(builder, final);
        }

        return builder != null ? builder.ToString() : text;
    }

    private static void AppendFinalJamo(StringBuilder builder, int final)
    {
        if (final <= 0)
        {
            return;
        }

        SplitFinal split;
        if (SplitFinals.TryGetValue(final, out split))
        {
            builder.Append(FinalJamo[split.remainingFinal]);
            builder.Append(InitialJamo[split.nextInitial]);
            return;
        }

        builder.Append(FinalJamo[final]);
    }

    private static bool TryGetKoreanConsonant(char c, out KeyLetter consonant)
    {
        return Consonants.TryGetValue(c, out consonant) || JamoConsonants.TryGetValue(c, out consonant);
    }

    private static bool TryGetKoreanVowel(char c, out int vowel)
    {
        return Vowels.TryGetValue(c, out vowel) || JamoVowels.TryGetValue(c, out vowel);
    }

    private static void FlushHangul(StringBuilder output, int initial, int vowel, int final)
    {
        if (initial >= 0 && vowel >= 0)
        {
            output.Append((char)(0xAC00 + ((initial * 21) + vowel) * 28 + Mathf.Clamp(final, 0, 27)));
        }
        else if (initial >= 0)
        {
            output.Append(InitialJamo[initial]);
        }
        else if (vowel >= 0)
        {
            output.Append(VowelJamo[vowel]);
        }
    }

    private static int PairKey(int left, int right)
    {
        return (left << 8) | right;
    }

    private struct KeyLetter
    {
        public readonly int initialIndex;
        public readonly int finalIndex;
        public readonly char jamo;

        public KeyLetter(int initialIndex, int finalIndex, char jamo)
        {
            this.initialIndex = initialIndex;
            this.finalIndex = finalIndex;
            this.jamo = jamo;
        }
    }

    private struct SplitFinal
    {
        public readonly int remainingFinal;
        public readonly int nextInitial;

        public SplitFinal(int remainingFinal, int nextInitial)
        {
            this.remainingFinal = remainingFinal;
            this.nextInitial = nextInitial;
        }
    }
}
