using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.jinja2
{
    internal sealed class TemplateTokenizer()
    {
        internal List<Block> Tokenize(string? text)
        {
            const char BlockStarter = '{';
            const char BlockEnder = '}';

            var blocks = new List<Block>();

            var endOfLastBlock = 0;

            var blockStartPos = 0;
            var blockStartFound = false;

            var insideTextValue = false;
            var textValueDelimiter = '\0';

            bool skipNextChar = false;
            char nextChar = text[0];
            for (int nextCharCursor = 1; nextCharCursor < text.Length; nextCharCursor++)
            {
                int currentCharPos = nextCharCursor - 1;
                int cursor = nextCharCursor;
                char currentChar = nextChar;
                nextChar = text[nextCharCursor];

                if (skipNextChar)
                {
                    skipNextChar = false;
                    continue;
                }

                // When "{{" is found outside a value
                // Note: "{{ {{x}}" => ["{{ ", "{{x}}"]
                if (!insideTextValue && currentChar == BlockStarter && nextChar == BlockStarter)
                {
                    // A block starts at the first "{"
                    blockStartPos = currentCharPos;
                    blockStartFound = true;
                }

                // After having found '{{'
                if (blockStartFound)
                {
                    if (currentChar == BlockEnder && nextChar == BlockEnder)
                    {
                        // If there is plain text between the current var/val/code block and the previous one, capture that as a TextBlock
                        if (blockStartPos > endOfLastBlock)
                        {
                            blocks.Add(new TextBlock(text.Substring(endOfLastBlock, blockStartPos - endOfLastBlock)));
                        }
                        // Extract raw block
                        var contentWithDelimiters = text.Substring(blockStartPos, cursor- blockStartPos + 1).Trim();

                            // Remove "{{" and "}}" delimiters and trim empty chars
                            var contentWithoutDelimiters = contentWithDelimiters
                                .Substring(2, contentWithDelimiters.Length- 4)
                                .Trim();

                            blocks.Add(new CodeBlock(contentWithoutDelimiters));
                            
                            endOfLastBlock = cursor + 1;
                            blockStartFound = false;
                    }
                }
            }
            if (endOfLastBlock < text.Length)
            {
                blocks.Add(new TextBlock(text.Substring( endOfLastBlock, text.Length- endOfLastBlock)));
            }
            return blocks;
        }
    }
}
