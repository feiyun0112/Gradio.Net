using System.Collections;
using System.Globalization;
using Gradio.Net.Core;
using Gradio.Net.Events;

namespace Gradio.Net.Components
{
    public class LoginButton : Button
    {
        public bool IsTemplate { get; } = true;

        public string LogoutValue { get; set; }

        private readonly object _every;
        private readonly object _inputs;

        public LoginButton(
            string value = "Sign in with Hugging Face",
            string logoutValue = "Logout ({})",
            object every = null,
            object inputs = null,
            string variant = "huggingface",
            string size = "lg",
            object icon = null,
            string link = null,
            bool visible = true,
            bool interactive = true,
            string elemId = null,
            object elemClasses = null,
            bool render = true,
            object key = null,
            object preservedByKey = null,
            int? scale = null,
            int? minWidth = null
        ) : base(
            value: value,
            variant: variant,
            size: size,
            icon: icon?.ToString() ?? Gradio.Net.Utils.Utils.GetIconPath("huggingface-logo.svg"),
            interactive: interactive,
            link: "_self",
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses as List<string>,
            key: key,
            preservedByKey: preservedByKey as string
        )
        {
            LogoutValue = logoutValue;
            _every = every;
            _inputs = inputs;

            if (elemClasses is string cls)
            {
                ElemClasses = new List<string> { cls };
            }

            if (preservedByKey is string pbk)
            {
                PreservedByKey = new List<string> { pbk };
            }
            else if (preservedByKey is List<string> pbkList)
            {
                PreservedByKey = pbkList;
            }

            if (scale.HasValue)
            {
                Scale = scale;
            }
            if (minWidth.HasValue)
            {
                MinWidth = minWidth;
            }

            if (Context.GetBlocksContext() != null && render)
            {
                Activate();
            }
        }

        public void Activate()
        {
            var page = (Page ?? string.Empty).TrimStart('/');
            var js = JsHandleRedirect
                .Replace("BUTTON_DEFAULT_VALUE", System.Text.Json.JsonSerializer.Serialize(Value ?? string.Empty), StringComparison.Ordinal)
                .Replace("REDIRECT_URL", page, StringComparison.Ordinal);

            var blocksConfig = Context.GetBlocksContext();
            if (blocksConfig != null)
            {
                blocksConfig.SetEventTrigger(
                    targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
                    fn: null,
                    inputs: new List<Block> { this },
                    outputs: null,
                    js: js,
                    preprocess: false,
                    postprocess: false,
                    apiVisibility: "undocumented"
                );
            }

            AttachLoadEvent(_checkLoginStatus, _every, ExtractInputBlocks(_inputs));
        }

        private static Block[] ExtractInputBlocks(object inputs)
        {
            if (inputs == null)
            {
                return Array.Empty<Block>();
            }

            if (inputs is Block one)
            {
                return new[] { one };
            }

            if (inputs is IEnumerable<Block> typed)
            {
                return typed.ToArray();
            }

            if (inputs is IEnumerable enumerable)
            {
                var list = new List<Block>();
                foreach (var item in enumerable)
                {
                    if (item is Block b)
                    {
                        list.Add(b);
                    }
                }
                return list.ToArray();
            }

            return Array.Empty<Block>();
        }

        private object _checkLoginStatus(Request request)
        {
            var session = GetSession(request);
            if (session == null || !session.TryGetValue("oauth_info", out var oauthInfoObj))
            {
                return UpdateButton(Value?.ToString() ?? "Sign in with Hugging Face", true);
            }

            var oauthInfo = AsStringObjectDict(oauthInfoObj);
            if (oauthInfo == null)
            {
                return UpdateButton(Value?.ToString() ?? "Sign in with Hugging Face", true);
            }

            if (oauthInfo.TryGetValue("expires_at", out var expiresAtObj))
            {
                var expiresAt = ToDouble(expiresAtObj);
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (expiresAt.HasValue && expiresAt.Value < now)
                {
                    session.Remove("oauth_info");
                    return UpdateButton(Value?.ToString() ?? "Sign in with Hugging Face", true);
                }
            }

            var userInfo = oauthInfo.TryGetValue("userinfo", out var userInfoObj)
                ? AsStringObjectDict(userInfoObj)
                : null;
            var username = userInfo != null && userInfo.TryGetValue("preferred_username", out var un)
                ? un?.ToString()
                : null;

            var logoutLabel = !string.IsNullOrWhiteSpace(username)
                ? LogoutValue.Replace("{}", username, StringComparison.Ordinal)
                : LogoutValue;

            return UpdateButton(logoutLabel, true);
        }

        private static Dictionary<string, object> UpdateButton(string value, bool interactive)
        {
            return new Dictionary<string, object>
            {
                ["value"] = value,
                ["interactive"] = interactive
            };
        }

        private static Dictionary<string, object> GetSession(Request request)
        {
            if (request?.Kwargs != null && request.Kwargs.TryGetValue("session", out var sessionObj))
            {
                return AsStringObjectDict(sessionObj);
            }
            return null;
        }

        private static Dictionary<string, object> AsStringObjectDict(object obj)
        {
            if (obj is Dictionary<string, object> dict)
            {
                return dict;
            }

            if (obj is Dictionary<object, object> objectDict)
            {
                var converted = new Dictionary<string, object>();
                foreach (var kv in objectDict)
                {
                    if (kv.Key != null)
                    {
                        converted[kv.Key.ToString()] = kv.Value;
                    }
                }
                return converted;
            }

            return null;
        }

        private static double? ToDouble(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value switch
            {
                double d => d,
                float f => f,
                int i => i,
                long l => l,
                decimal m => (double)m,
                string s when double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) => parsed,
                _ => null
            };
        }

        private const string JsHandleRedirect = @"
(buttonValue) => {
    uri = buttonValue === BUTTON_DEFAULT_VALUE ? '/login/huggingface?_target_url=/REDIRECT_URL' : '/logout?_target_url=/REDIRECT_URL';
    window.parent?.postMessage({ type: 'SET_SCROLLING', enabled: true }, '*');
    setTimeout(() => {
        window.location.assign(uri + window.location.search);
    }, 500);
}
";
    }
}
