using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Color
    {
        public string C50 { get; set; }
        public string C100 { get; set; }
        public string C200 { get; set; }
        public string C300 { get; set; }
        public string C400 { get; set; }
        public string C500 { get; set; }
        public string C600 { get; set; }
        public string C700 { get; set; }
        public string C800 { get; set; }
        public string C900 { get; set; }
        public string C950 { get; set; }
        public string Name { get; set; }

        public Color(string c50, string c100, string c200, string c300, string c400, string c500, string c600, string c700, string c800, string c900, string c950, string name = null)
        {
            C50 = c50;
            C100 = c100;
            C200 = c200;
            C300 = c300;
            C400 = c400;
            C500 = c500;
            C600 = c600;
            C700 = c700;
            C800 = c800;
            C900 = c900;
            C950 = c950;
            Name = name;

        }

        public List<string> Expand()
        {
            return new List<string> { C50, C100, C200, C300, C400, C500, C600, C700, C800, C900, C950 };
        }
    }

    public static class Colors
    {
        public static Color Slate { get; } = new Color(
                 name: "slate",
                 c50: "#f8fafc",
                 c100: "#f1f5f9",
                 c200: "#e2e8f0",
                 c300: "#cbd5e1",
                 c400: "#94a3b8",
                 c500: "#64748b",
                 c600: "#475569",
                 c700: "#334155",
                 c800: "#1e293b",
                 c900: "#0f172a",
                 c950: "#0a0f1e"
             );

        public static Color Gray { get; } = new Color(
            name: "gray",
            c50: "#f9fafb",
            c100: "#f3f4f6",
            c200: "#e5e7eb",
            c300: "#d1d5db",
            c400: "#9ca3af",
            c500: "#6b7280",
            c600: "#4b5563",
            c700: "#374151",
            c800: "#1f2937",
            c900: "#111827",
            c950: "#0b0f19"
        );
        public static Color Zinc { get; } = new Color(
            name: "zinc",
            c50: "#fafafa",
            c100: "#f4f4f5",
            c200: "#e4e4e7",
            c300: "#d4d4d8",
            c400: "#bbbbc2",
            c500: "#71717a",
            c600: "#52525b",
            c700: "#3f3f46",
            c800: "#27272a",
            c900: "#18181b",
            c950: "#0f0f11"
        );
        public static Color Neutral { get; } = new Color(
            name: "neutral",
            c50: "#fafafa",
            c100: "#f5f5f5",
            c200: "#e5e5e5",
            c300: "#d4d4d4",
            c400: "#a3a3a3",
            c500: "#737373",
            c600: "#525252",
            c700: "#404040",
            c800: "#262626",
            c900: "#171717",
            c950: "#0f0f0f"
        );
        public static Color Stone { get; } = new Color(
            name: "stone",
            c50: "#fafaf9",
            c100: "#f5f5f4",
            c200: "#e7e5e4",
            c300: "#d6d3d1",
            c400: "#a8a29e",
            c500: "#78716c",
            c600: "#57534e",
            c700: "#44403c",
            c800: "#292524",
            c900: "#1c1917",
            c950: "#0f0e0d"
        );
        public static Color Red { get; } = new Color(
            name: "red",
            c50: "#fef2f2",
            c100: "#fee2e2",
            c200: "#fecaca",
            c300: "#fca5a5",
            c400: "#f87171",
            c500: "#ef4444",
            c600: "#dc2626",
            c700: "#b91c1c",
            c800: "#991b1b",
            c900: "#7f1d1d",
            c950: "#6c1e1e"
        );
        public static Color Orange { get; } = new Color(
            name: "orange",
            c50: "#fff7ed",
            c100: "#ffedd5",
            c200: "#ffddb3",
            c300: "#fdba74",
            c400: "#fb923c",
            c500: "#f97316",
            c600: "#ea580c",
            c700: "#c2410c",
            c800: "#9a3412",
            c900: "#7c2d12",
            c950: "#6c2e12"
        );
        public static Color Amber { get; } = new Color(
            name: "amber",
            c50: "#fffbeb",
            c100: "#fef3c7",
            c200: "#fde68a",
            c300: "#fcd34d",
            c400: "#fbbf24",
            c500: "#f59e0b",
            c600: "#d97706",
            c700: "#b45309",
            c800: "#92400e",
            c900: "#78350f",
            c950: "#6c370f"
        );
        public static Color Yellow { get; } = new Color(
            name: "yellow",
            c50: "#fefce8",
            c100: "#fef9c3",
            c200: "#fef08a",
            c300: "#fde047",
            c400: "#facc15",
            c500: "#eab308",
            c600: "#ca8a04",
            c700: "#a16207",
            c800: "#854d0e",
            c900: "#713f12",
            c950: "#653b12"
        );
        public static Color Lime { get; } = new Color(
            name: "lime",
            c50: "#f7fee7",
            c100: "#ecfccb",
            c200: "#d9f99d",
            c300: "#bef264",
            c400: "#a3e635",
            c500: "#84cc16",
            c600: "#65a30d",
            c700: "#4d7c0f",
            c800: "#3f6212",
            c900: "#365314",
            c950: "#2f4e14"
        );
        public static Color Green { get; } = new Color(
            name: "green",
            c50: "#f0fdf4",
            c100: "#dcfce7",
            c200: "#bbf7d0",
            c300: "#86efac",
            c400: "#4ade80",
            c500: "#22c55e",
            c600: "#16a34a",
            c700: "#15803d",
            c800: "#166534",
            c900: "#14532d",
            c950: "#134e28"
        );
        public static Color Emerald { get; } = new Color(
            name: "emerald",
            c50: "#ecfdf5",
            c100: "#d1fae5",
            c200: "#a7f3d0",
            c300: "#6ee7b7",
            c400: "#34d399",
            c500: "#10b981",
            c600: "#059669",
            c700: "#047857",
            c800: "#065f46",
            c900: "#064e3b",
            c950: "#054436"
        );
        public static Color Teal { get; } = new Color(
            name: "teal",
            c50: "#f0fdfa",
            c100: "#ccfbf1",
            c200: "#99f6e4",
            c300: "#5eead4",
            c400: "#2dd4bf",
            c500: "#14b8a6",
            c600: "#0d9488",
            c700: "#0f766e",
            c800: "#115e59",
            c900: "#134e4a",
            c950: "#12443e"
        );
        public static Color Cyan { get; } = new Color(
            name: "cyan",
            c50: "#ecfeff",
            c100: "#cffafe",
            c200: "#a5f3fc",
            c300: "#67e8f9",
            c400: "#22d3ee",
            c500: "#06b6d4",
            c600: "#0891b2",
            c700: "#0e7490",
            c800: "#155e75",
            c900: "#164e63",
            c950: "#14455c"
        );
        public static Color Sky { get; } = new Color(
            name: "sky",
            c50: "#f0f9ff",
            c100: "#e0f2fe",
            c200: "#bae6fd",
            c300: "#7dd3fc",
            c400: "#38bdf8",
            c500: "#0ea5e9",
            c600: "#0284c7",
            c700: "#0369a1",
            c800: "#075985",
            c900: "#0c4a6e",
            c950: "#0b4165"
        );
        public static Color Blue { get; } = new Color(
            name: "blue",
            c50: "#eff6ff",
            c100: "#dbeafe",
            c200: "#bfdbfe",
            c300: "#93c5fd",
            c400: "#60a5fa",
            c500: "#3b82f6",
            c600: "#2563eb",
            c700: "#1d4ed8",
            c800: "#1e40af",
            c900: "#1e3a8a",
            c950: "#1d3660"
        );
        public static Color Indigo { get; } = new Color(
            name: "indigo",
            c50: "#eef2ff",
            c100: "#e0e7ff",
            c200: "#c7d2fe",
            c300: "#a5b4fc",
            c400: "#818cf8",
            c500: "#6366f1",
            c600: "#4f46e5",
            c700: "#4338ca",
            c800: "#3730a3",
            c900: "#312e81",
            c950: "#2b2c5e"
        );
        public static Color Violet { get; } = new Color(
            name: "violet",
            c50: "#f5f3ff",
            c100: "#ede9fe",
            c200: "#ddd6fe",
            c300: "#c4b5fd",
            c400: "#a78bfa",
            c500: "#8b5cf6",
            c600: "#7c3aed",
            c700: "#6d28d9",
            c800: "#5b21b6",
            c900: "#4c1d95",
            c950: "#431d7f"
        );
        public static Color Purple { get; } = new Color(
            name: "purple",
            c50: "#faf5ff",
            c100: "#f3e8ff",
            c200: "#e9d5ff",
            c300: "#d8b4fe",
            c400: "#c084fc",
            c500: "#a855f7",
            c600: "#9333ea",
            c700: "#7e22ce",
            c800: "#6b21a8",
            c900: "#581c87",
            c950: "#4c1a73"
        );
        public static Color Fuchsia { get; } = new Color(
            name: "fuchsia",
            c50: "#fdf4ff",
            c100: "#fae8ff",
            c200: "#f5d0fe",
            c300: "#f0abfc",
            c400: "#e879f9",
            c500: "#d946ef",
            c600: "#c026d3",
            c700: "#a21caf",
            c800: "#86198f",
            c900: "#701a75",
            c950: "#5e1a66"
        );
        public static Color Pink { get; } = new Color(
            name: "pink",
            c50: "#fdf2f8",
            c100: "#fce7f3",
            c200: "#fbcfe8",
            c300: "#f9a8d4",
            c400: "#f472b6",
            c500: "#ec4899",
            c600: "#db2777",
            c700: "#be185d",
            c800: "#9d174d",
            c900: "#831843",
            c950: "#6e1a3d"
        );
        public static Color Rose { get; } = new Color(
            name: "rose",
            c50: "#fff1f2",
            c100: "#ffe4e6",
            c200: "#fecdd3",
            c300: "#fda4af",
            c400: "#fb7185",
            c500: "#f43f5e",
            c600: "#e11d48",
            c700: "#be123c",
            c800: "#9f1239",
            c900: "#881337",
            c950: "#771d3a"
        );
    }
}
