using Pulumi;
using static System.Text.Encoding;
using Convert = System.Convert;

namespace Ubiquitous.AutoDevOps.Stack;

public static class Extensions {
    public static InputMap<string> AsInputMap(this Dictionary<string, string>? dict) => dict ?? new Dictionary<string, string>();

    public static InputMap<string> AddPair(this InputMap<string> inputMap, string key, string? value) {
        inputMap.Add(key, value ?? "");
        return inputMap;
    }

    public static InputMap<string> AddPairIf(this InputMap<string> inputMap, bool condition, string key, string? value) {
        if (condition) inputMap.Add(key, value ?? "");
        return inputMap;
    }

    public static T When<T>(this T self, bool argument, Action<T> action) {
        if (argument) action(self);
        return self;
    }

    public static T WhenNotEmptyString<T>(this T self, string? argument, Action<T, string> action) {
        if (!string.IsNullOrEmpty(argument)) action(self, argument);
        return self;
    }

    public static T WhenNotNull<T, TArg>(this T self, TArg? argument, Action<T, TArg> action) {
        if (argument is not null) action(self, argument);
        return self;
    }

    public static string Or(this string? val, string alt) => string.IsNullOrEmpty(val) ? alt : val;

    public static int IntOr(this string? val, int alt) => !string.IsNullOrWhiteSpace(val) && int.TryParse(val, out var intVal) ? intVal : alt;
        
    public static string Base64Encode(this string plainText) => Convert.ToBase64String(UTF8.GetBytes(plainText));

    public static bool IsEmpty(this string? value) => string.IsNullOrWhiteSpace(value);
}