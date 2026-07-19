using System.Reflection;
using Gradio.Net.Components;

namespace Gradio.Net
{
    public static class PipelinesUtils
    {
        private delegate object PostprocessDelegate(object result, object[] originalInputs);

        public static Dictionary<string, object> HandleTransformersPipeline(object pipeline)
        {
            var name = pipeline?.GetType().Name ?? string.Empty;

            if (IsTransformersPipelineType(name, "AudioClassificationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: AudioInput("Input"),
                    outputs: TextOutput("Class"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (IsTransformersPipelineType(name, "AutomaticSpeechRecognitionPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: AudioInput("Input"),
                    outputs: TextOutput("Output"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => GetValue(r, "text")
                );
            }
            if (IsTransformersPipelineType(name, "FeatureExtractionPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Output"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => First(r)
                );
            }
            if (IsTransformersPipelineType(name, "FillMaskPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Classification"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => ToScoreDict(r, "token_str", "score")
                );
            }
            if (IsTransformersPipelineType(name, "ImageClassificationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: ImageInput("Input Image"),
                    outputs: TextOutput("Classification"),
                    preprocess: args => new Dictionary<string, object> { ["images"] = Arg(args, 0) },
                    postprocess: (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (IsTransformersPipelineType(name, "QuestionAnsweringPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component> { new Textbox(lines: 7, label: "Context"), TextInput("Question") },
                    outputs: new List<Component> { TextOutput("Answer"), TextOutput("Score") },
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["context"] = Arg(args, 0),
                        ["question"] = Arg(args, 1)
                    },
                    postprocess: (r, _) => new object[] { GetValue(r, "answer"), GetValue(r, "score") }
                );
            }
            if (IsTransformersPipelineType(name, "SummarizationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new Textbox(lines: 7, label: "Input"),
                    outputs: TextOutput("Summary"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => GetValue(First(r), "summary_text")
                );
            }
            if (IsTransformersPipelineType(name, "TextClassificationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Classification"),
                    preprocess: args => new object[] { Arg(args, 0) },
                    postprocess: (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (IsTransformersPipelineType(name, "TokenClassificationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Entities"),
                    preprocess: args => new object[] { Arg(args, 0) },
                    postprocess: (r, inputArgs) => new Dictionary<string, object>
                    {
                        ["text"] = Arg(inputArgs, 0),
                        ["entities"] = ToList(r)
                    }
                );
            }
            if (IsTransformersPipelineType(name, "TextGenerationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Output"),
                    preprocess: args => new Dictionary<string, object> { ["text_inputs"] = Arg(args, 0) },
                    postprocess: (r, _) => GetValue(First(r), "generated_text")
                );
            }
            if (IsTransformersPipelineType(name, "TranslationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Translation"),
                    preprocess: args => new object[] { Arg(args, 0) },
                    postprocess: (r, _) => GetValue(First(r), "translation_text")
                );
            }
            if (IsTransformersPipelineType(name, "Text2TextGenerationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: TextInput("Input"),
                    outputs: TextOutput("Generated Text"),
                    preprocess: args => new object[] { Arg(args, 0) },
                    postprocess: (r, _) => GetValue(First(r), "generated_text")
                );
            }
            if (IsTransformersPipelineType(name, "ZeroShotClassificationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component>
                    {
                        TextInput("Input"),
                        TextInput("Possible class names (comma-separated)"),
                        new Checkbox(label: "Allow multiple true classes")
                    },
                    outputs: TextOutput("Classification"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["sequences"] = Arg(args, 0),
                        ["candidate_labels"] = Arg(args, 1),
                        ["multi_label"] = Arg(args, 2)
                    },
                    postprocess: (r, _) => ZipLabelsScores(r)
                );
            }
            if (IsTransformersPipelineType(name, "DocumentQuestionAnsweringPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component> { ImageInput("Input Document"), TextInput("Question") },
                    outputs: TextOutput("Label"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["image"] = Arg(args, 0),
                        ["question"] = Arg(args, 1)
                    },
                    postprocess: (r, _) => ToScoreDict(r, "answer", "score")
                );
            }
            if (IsTransformersPipelineType(name, "VisualQuestionAnsweringPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component> { ImageInput("Input Image"), TextInput("Question") },
                    outputs: TextOutput("Score"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["image"] = Arg(args, 0),
                        ["question"] = Arg(args, 1)
                    },
                    postprocess: (r, _) => ToScoreDict(r, "answer", "score")
                );
            }
            if (IsTransformersPipelineType(name, "ImageToTextPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: ImageInput("Input Image"),
                    outputs: TextOutput("Text"),
                    preprocess: args => new Dictionary<string, object> { ["images"] = Arg(args, 0) },
                    postprocess: (r, _) => GetValue(First(r), "generated_text")
                );
            }
            if (IsTransformersPipelineType(name, "ObjectDetectionPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: ImageInput("Input Image"),
                    outputs: TextOutput("Objects Detected"),
                    preprocess: args => new Dictionary<string, object> { ["inputs"] = Arg(args, 0) },
                    postprocess: (r, inputArgs) => new object[] { Arg(inputArgs, 0), ToObjectDetections(r) }
                );
            }

            throw new Exception($"Unsupported transformers pipeline type: {pipeline?.GetType()}");
        }

        public static Dictionary<string, object> HandleDiffusersPipeline(object pipeline)
        {
            var name = pipeline?.GetType().Name ?? string.Empty;

            if (IsDiffusersPipelineType(name, "StableDiffusionPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: SdBaseInputs(),
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["prompt"] = Arg(args, 0),
                        ["negative_prompt"] = Arg(args, 1),
                        ["num_inference_steps"] = Arg(args, 2),
                        ["guidance_scale"] = Arg(args, 3)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            if (IsDiffusersPipelineType(name, "StableDiffusionImg2ImgPipeline") ||
                IsDiffusersPipelineType(name, "StableDiffusionDepth2ImgPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: SdImageToImageInputs(),
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["prompt"] = Arg(args, 0),
                        ["negative_prompt"] = Arg(args, 1),
                        ["image"] = Arg(args, 2),
                        ["strength"] = Arg(args, 3),
                        ["num_inference_steps"] = Arg(args, 4),
                        ["guidance_scale"] = Arg(args, 5)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            if (IsDiffusersPipelineType(name, "StableDiffusionInpaintPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: SdInpaintInputs(),
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["prompt"] = Arg(args, 0),
                        ["negative_prompt"] = Arg(args, 1),
                        ["image"] = Arg(args, 2),
                        ["mask_image"] = Arg(args, 3),
                        ["strength"] = Arg(args, 4),
                        ["num_inference_steps"] = Arg(args, 5),
                        ["guidance_scale"] = Arg(args, 6)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            if (IsDiffusersPipelineType(name, "StableDiffusionImageVariationPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component>
                    {
                        ImageInput("Image"),
                        new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                        new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5)
                    },
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["image"] = Arg(args, 0),
                        ["num_inference_steps"] = Arg(args, 1),
                        ["guidance_scale"] = Arg(args, 2)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            if (IsDiffusersPipelineType(name, "StableDiffusionInstructPix2PixPipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component>
                    {
                        TextInput("Prompt"),
                        TextInput("Negative prompt"),
                        ImageInput("Image"),
                        new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                        new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5),
                        new Slider(label: "Image Guidance scale", min: 1, max: 5, value: 1.5, step: 0.5)
                    },
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["prompt"] = Arg(args, 0),
                        ["negative_prompt"] = Arg(args, 1),
                        ["image"] = Arg(args, 2),
                        ["num_inference_steps"] = Arg(args, 3),
                        ["guidance_scale"] = Arg(args, 4),
                        ["image_guidance_scale"] = Arg(args, 5)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            if (IsDiffusersPipelineType(name, "StableDiffusionUpscalePipeline"))
            {
                return BuildPipelineInfo(
                    inputs: new List<Component>
                    {
                        TextInput("Prompt"),
                        TextInput("Negative prompt"),
                        ImageInput("Image"),
                        new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                        new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5),
                        new Slider(label: "Noise level", min: 1, max: 100, value: 20, step: 1)
                    },
                    outputs: ImageOutput("Generated Image"),
                    preprocess: args => new Dictionary<string, object>
                    {
                        ["prompt"] = Arg(args, 0),
                        ["negative_prompt"] = Arg(args, 1),
                        ["image"] = Arg(args, 2),
                        ["num_inference_steps"] = Arg(args, 3),
                        ["guidance_scale"] = Arg(args, 4),
                        ["noise_level"] = Arg(args, 5)
                    },
                    postprocess: (r, _) => First(GetValue(r, "images"))
                );
            }

            throw new Exception($"Unsupported diffusers pipeline type: {pipeline?.GetType()}");
        }

        public static Dictionary<string, object> HandleTransformersJsPipeline(object pipeline)
        {
            var task = GetStringProperty(pipeline, "task") ?? string.Empty;

            // NLP
            if (task == "fill-mask")
            {
                return BuildPipelineInfo(TextInput("Input"), TextOutput("Classification"), null, (r, _) => ToScoreDict(r, "token_str", "score"));
            }
            if (task == "question-answering")
            {
                return BuildPipelineInfo(
                    new List<Component> { new Textbox(lines: 7, label: "Context"), TextInput("Question") },
                    new List<Component> { TextOutput("Answer"), TextOutput("Score") },
                    args => new object[] { Arg(args, 1), Arg(args, 0) },
                    (r, _) => new object[] { GetValue(r, "answer"), GetValue(r, "score") }
                );
            }
            if (task == "summarization")
            {
                return BuildPipelineInfo(
                    new List<Component>
                    {
                        new Textbox(lines: 7, label: "Input"),
                        new Slider(label: "The maximum numbers of tokens to generate", min: 1, max: 500, value: 100, step: 1)
                    },
                    TextOutput("Summary"),
                    args => new object[] { Arg(args, 0), new Dictionary<string, object> { ["max_new_tokens"] = Arg(args, 1) } },
                    (r, _) => GetValue(First(r), "summary_text")
                );
            }
            if (task == "text-classification")
            {
                return BuildPipelineInfo(
                    new List<Component> { TextInput("Input"), TextInput("Top k") },
                    TextOutput("Classification"),
                    args => new object[] { Arg(args, 0), new Dictionary<string, object> { ["topk"] = Arg(args, 1) } },
                    (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (task == "text-generation")
            {
                return BuildPipelineInfo(TextInput("Input"), TextOutput("Output"), null, (r, _) => GetValue(First(r), "generated_text"));
            }
            if (task == "text2text-generation")
            {
                return BuildPipelineInfo(
                    new List<Component>
                    {
                        TextInput("Input"),
                        new Slider(label: "The maximum numbers of tokens to generate", min: 1, max: 500, value: 100, step: 1)
                    },
                    TextOutput("Generated Text"),
                    args => new object[] { Arg(args, 0), new Dictionary<string, object> { ["max_new_tokens"] = Arg(args, 1) } },
                    (r, _) => GetValue(First(r), "generated_text")
                );
            }
            if (task == "token-classification")
            {
                var info = BuildPipelineInfo(TextInput("Input"), TextOutput("Output"), null, (r, _) => r);
                info["postprocess_takes_inputs"] = true;
                return info;
            }
            if (task == "translation" || task == "translation_xx_to_yy")
            {
                return BuildPipelineInfo(
                    new List<Component> { TextInput("Input"), TextInput("Source Language"), TextInput("Target Language") },
                    TextOutput("Translation"),
                    args => new object[]
                    {
                        Arg(args, 0),
                        new Dictionary<string, object>
                        {
                            ["src_lang"] = Arg(args, 1),
                            ["tgt_lang"] = Arg(args, 2)
                        }
                    },
                    (r, _) => GetValue(First(r), "translation_text")
                );
            }
            if (task == "zero-shot-classification")
            {
                return BuildPipelineInfo(
                    new List<Component> { TextInput("Input"), TextInput("Possible class names (comma-separated)") },
                    TextOutput("Classification"),
                    args => new object[] { Arg(args, 0), SplitCsv(Arg(args, 1)) },
                    (r, _) => ZipLabelsScores(r)
                );
            }
            if (task == "feature-extraction")
            {
                return BuildPipelineInfo(TextInput("Input"), TextOutput("Output"), null, (r, _) => First(r));
            }

            // Vision
            if (task == "depth-estimation")
            {
                return BuildPipelineInfo(ImageInput("Input Image"), ImageOutput("Depth"), args => new object[] { Arg(args, 0) }, (r, _) => GetValue(r, "depth") ?? r);
            }
            if (task == "image-classification")
            {
                return BuildPipelineInfo(
                    new List<Component> { ImageInput("Input Image"), TextInput("Top k") },
                    TextOutput("Classification"),
                    args => new object[] { Arg(args, 0), new Dictionary<string, object> { ["topk"] = Arg(args, 1) } },
                    (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (task == "image-segmentation")
            {
                var info = BuildPipelineInfo(ImageInput("Input Image"), TextOutput("Segmentation"), args => new object[] { Arg(args, 0) }, (r, i) => new object[] { Arg(i, 0), r });
                info["postprocess_takes_inputs"] = true;
                return info;
            }
            if (task == "image-to-image")
            {
                return BuildPipelineInfo(ImageInput("Input Image"), ImageOutput("Output Image"), args => new object[] { Arg(args, 0) }, (r, _) => r);
            }
            if (task == "object-detection")
            {
                var info = BuildPipelineInfo(ImageInput("Input Image"), TextOutput("Objects Detected"), args => new object[] { Arg(args, 0) }, (r, i) => new object[] { Arg(i, 0), ToObjectDetections(r) });
                info["postprocess_takes_inputs"] = true;
                return info;
            }
            if (task == "image-feature-extraction")
            {
                return BuildPipelineInfo(ImageInput("Input Image"), TextOutput("Output"), args => new object[] { Arg(args, 0) }, (r, _) => r);
            }

            // Audio
            if (task == "audio-classification")
            {
                return BuildPipelineInfo(AudioInput("Input"), TextOutput("Class"), args => new object[] { Arg(args, 0) }, (r, _) => ToScoreDict(r, "label", "score"));
            }
            if (task == "automatic-speech-recognition")
            {
                return BuildPipelineInfo(AudioInput("Input"), TextOutput("Output"), args => new object[] { Arg(args, 0) }, (r, _) => GetValue(r, "text"));
            }
            if (task == "text-to-audio")
            {
                return BuildPipelineInfo(
                    new List<Component> { TextInput("Input"), TextInput("Speaker Embeddings") },
                    new Audio(label: "Output"),
                    args => new object[] { Arg(args, 0), new Dictionary<string, object> { ["speaker_embeddings"] = Arg(args, 1) } },
                    (r, _) => r
                );
            }

            // Multimodal
            if (task == "document-question-answering")
            {
                return BuildPipelineInfo(
                    new List<Component> { ImageInput("Input Document"), TextInput("Question") },
                    TextOutput("Label"),
                    args => new object[] { Arg(args, 0), Arg(args, 1) },
                    (r, _) => GetValue(First(r), "answer")
                );
            }
            if (task == "image-to-text")
            {
                return BuildPipelineInfo(ImageInput("Input Image"), TextOutput("Output"), args => new object[] { Arg(args, 0) }, (r, _) => GetValue(First(r), "generated_text"));
            }
            if (task == "zero-shot-audio-classification")
            {
                return BuildPipelineInfo(
                    new List<Component> { AudioInput("Input"), TextInput("Possible class names (comma-separated)") },
                    TextOutput("Classification"),
                    args => new object[] { Arg(args, 0), SplitCsv(Arg(args, 1)) },
                    (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (task == "zero-shot-image-classification")
            {
                return BuildPipelineInfo(
                    new List<Component> { ImageInput("Input Image"), TextInput("Possible class names (comma-separated)") },
                    TextOutput("Classification"),
                    args => new object[] { Arg(args, 0), SplitCsv(Arg(args, 1)) },
                    (r, _) => ToScoreDict(r, "label", "score")
                );
            }
            if (task == "zero-shot-object-detection")
            {
                var info = BuildPipelineInfo(
                    new List<Component> { ImageInput("Input Image"), TextInput("Possible class names (comma-separated)") },
                    TextOutput("Objects Detected"),
                    args => new object[] { Arg(args, 0), SplitCsv(Arg(args, 1)) },
                    (r, i) => new object[] { Arg(i, 0), ToObjectDetections(r) }
                );
                info["postprocess_takes_inputs"] = true;
                return info;
            }

            throw new Exception($"Unsupported transformers_js_py pipeline type: {task}");
        }

        private static bool IsTransformersPipelineType(string pipelineTypeName, string className)
        {
            return pipelineTypeName.Equals(className, StringComparison.Ordinal) ||
                   pipelineTypeName.EndsWith(className, StringComparison.Ordinal);
        }

        private static bool IsDiffusersPipelineType(string pipelineTypeName, string className)
        {
            return pipelineTypeName.Equals(className, StringComparison.Ordinal) ||
                   pipelineTypeName.EndsWith(className, StringComparison.Ordinal);
        }

        private static Dictionary<string, object> BuildPipelineInfo(
            object inputs,
            object outputs,
            Func<object[], object>? preprocess,
            PostprocessDelegate? postprocess)
        {
            return new Dictionary<string, object>
            {
                ["inputs"] = inputs,
                ["outputs"] = outputs,
                ["preprocess"] = preprocess,
                ["postprocess"] = postprocess
            };
        }

        private static object Arg(object[] args, int index) => (args != null && args.Length > index) ? args[index] : null;

        private static Textbox TextInput(string label) => new Textbox(label: label);
        private static Textbox TextOutput(string label) => new Textbox(label: label, interactive: false);
        private static Audio AudioInput(string label) => new Audio(type: "filepath", label: label);
        private static Image ImageInput(string label) => new Image(type: "filepath", label: label);
        private static Image ImageOutput(string label) => new Image(type: "filepath", label: label, interactive: false);

        private static List<Component> SdBaseInputs()
        {
            return new List<Component>
            {
                TextInput("Prompt"),
                TextInput("Negative prompt"),
                new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5)
            };
        }

        private static List<Component> SdImageToImageInputs()
        {
            return new List<Component>
            {
                TextInput("Prompt"),
                TextInput("Negative prompt"),
                ImageInput("Image"),
                new Slider(label: "Strength", min: 0, max: 1, value: 0.8, step: 0.1),
                new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5)
            };
        }

        private static List<Component> SdInpaintInputs()
        {
            return new List<Component>
            {
                TextInput("Prompt"),
                TextInput("Negative prompt"),
                ImageInput("Image"),
                ImageInput("Mask Image"),
                new Slider(label: "Strength", min: 0, max: 1, value: 0.8, step: 0.1),
                new Slider(label: "Number of inference steps", min: 1, max: 500, value: 50, step: 1),
                new Slider(label: "Guidance scale", min: 1, max: 20, value: 7.5, step: 0.5)
            };
        }

        private static object? GetValue(object obj, string key)
        {
            if (obj == null) return null;

            if (obj is IDictionary<string, object> dict)
            {
                return dict.TryGetValue(key, out var value) ? value : null;
            }

            if (obj is IDictionary<object, object> objDict)
            {
                return objDict.TryGetValue(key, out var value) ? value : null;
            }

            var prop = obj.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return prop?.GetValue(obj);
        }

        private static List<object> ToList(object obj)
        {
            if (obj == null) return new List<object>();
            if (obj is IEnumerable<object> eo) return eo.ToList();
            if (obj is System.Collections.IEnumerable e)
            {
                var list = new List<object>();
                foreach (var item in e) list.Add(item);
                return list;
            }
            return new List<object> { obj };
        }

        private static object? First(object obj)
        {
            return ToList(obj).FirstOrDefault();
        }

        private static Dictionary<string, object> ToScoreDict(object result, string labelKey, string scoreKey)
        {
            var output = new Dictionary<string, object>();
            foreach (var item in ToList(result))
            {
                var label = GetValue(item, labelKey)?.ToString() ?? string.Empty;
                var score = GetValue(item, scoreKey) ?? 0;
                if (!string.IsNullOrWhiteSpace(label))
                {
                    output[label] = score;
                }
            }
            return output;
        }

        private static Dictionary<string, object> ZipLabelsScores(object result)
        {
            var labels = ToList(GetValue(result, "labels"));
            var scores = ToList(GetValue(result, "scores"));
            var count = Math.Min(labels.Count, scores.Count);
            var output = new Dictionary<string, object>();
            for (int i = 0; i < count; i++)
            {
                var k = labels[i]?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(k))
                {
                    output[k] = scores[i] ?? 0;
                }
            }
            return output;
        }

        private static List<object> ToObjectDetections(object result)
        {
            var dets = new List<object>();
            foreach (var item in ToList(result))
            {
                var box = GetValue(item, "box");
                var xmin = Convert.ToInt32(GetValue(box, "xmin") ?? 0);
                var ymin = Convert.ToInt32(GetValue(box, "ymin") ?? 0);
                var xmax = Convert.ToInt32(GetValue(box, "xmax") ?? 0);
                var ymax = Convert.ToInt32(GetValue(box, "ymax") ?? 0);
                var label = GetValue(item, "label")?.ToString() ?? "";
                var score = GetValue(item, "score");
                var text = score != null ? $"{label} ({score})" : label;
                dets.Add(new object[] { new object[] { xmin, ymin, xmax, ymax }, text });
            }
            return dets;
        }

        private static List<string> SplitCsv(object value)
        {
            return (value?.ToString() ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        private static string? GetStringProperty(object obj, string name)
        {
            if (obj == null) return null;
            var p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return p?.GetValue(obj)?.ToString();
        }
    }
}
