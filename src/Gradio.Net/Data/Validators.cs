namespace Gradio.Net.Data;

using Gradio.Net.Core;

public static class Validators
{
    public static Dictionary<string, object> IsAudioCorrectLength(
        Tuple<int, Array> audio,
        double? minLength,
        double? maxLength)
    {
        if (audio != null && (minLength != null || maxLength != null))
        {
            var sampleRate = audio.Item1;
            var data = audio.Item2;
            var duration = sampleRate > 0 ? (double)data.Length / sampleRate : 0;

            if (minLength != null && duration < minLength)
            {
                return new Dictionary<string, object>
                {
                    ["__type__"] = "validate",
                    ["is_valid"] = false,
                    ["message"] = $"Audio is too short. It must be at least {minLength} seconds"
                };
            }

            if (maxLength != null && duration > maxLength)
            {
                return new Dictionary<string, object>
                {
                    ["__type__"] = "validate",
                    ["is_valid"] = false,
                    ["message"] = $"Audio is too long. It must be at most {maxLength} seconds"
                };
            }
        }

        return new Dictionary<string, object>
        {
            ["__type__"] = "validate",
            ["is_valid"] = true
        };
    }

    public static Dictionary<string, object> IsAudioCorrectLength(
        Tuple<int, float[]> audio,
        double? minLength,
        double? maxLength)
    {
        if (audio == null)
        {
            return new Dictionary<string, object>
            {
                ["__type__"] = "validate",
                ["is_valid"] = true
            };
        }

        return IsAudioCorrectLength(
            new Tuple<int, Array>(audio.Item1, audio.Item2),
            minLength,
            maxLength);
    }

    public static Dictionary<string, object> IsVideoCorrectLength(
        string video,
        double? minLength,
        double? maxLength)
    {
        if (!string.IsNullOrWhiteSpace(video) && (minLength != null || maxLength != null))
        {
            var duration = ProcessingUtils.GetVideoLength(video);

            if (minLength != null && duration < minLength)
            {
                return new Dictionary<string, object>
                {
                    ["__type__"] = "validate",
                    ["is_valid"] = false,
                    ["message"] = $"Video is too short. It must be at least {minLength} seconds"
                };
            }

            if (maxLength != null && duration > maxLength)
            {
                return new Dictionary<string, object>
                {
                    ["__type__"] = "validate",
                    ["is_valid"] = false,
                    ["message"] = $"Video is too long. It must be at most {maxLength} seconds"
                };
            }
        }

        return new Dictionary<string, object>
        {
            ["__type__"] = "validate",
            ["is_valid"] = true
        };
    }

    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static bool IsValidFilePath(string path)
    {
        try
        {
            Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidInteger(string value)
    {
        return int.TryParse(value, out _);
    }

    public static bool IsValidFloat(string value)
    {
        return float.TryParse(value, out _);
    }

    public static bool IsValidBoolean(string value)
    {
        return bool.TryParse(value, out _);
    }

    public static bool IsNonEmptyString(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValidComponentId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && int.TryParse(id, out _);
    }

    public static T ValidateRequired<T>(T? value, string fieldName = "Value") where T : struct
    {
        if (!value.HasValue)
        {
            throw new ArgumentNullException(fieldName, $"{fieldName} is required.");
        }
        return value.Value;
    }

    public static string ValidateRequired(string? value, string fieldName = "Value")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(fieldName, $"{fieldName} is required.");
        }
        return value;
    }

    public static T ValidateMinLength<T>(T value, int minLength, string fieldName = "Value") where T : ICollection<object>
    {
        if (value.Count < minLength)
        {
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} must have at least {minLength} items.");
        }
        return value;
    }

    public static T ValidateMaxLength<T>(T value, int maxLength, string fieldName = "Value") where T : ICollection<object>
    {
        if (value.Count > maxLength)
        {
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} must have at most {maxLength} items.");
        }
        return value;
    }
}
