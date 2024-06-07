using demo;
using Gradio.Net;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddGradio();

//var app = builder.Build();

//app.UseGradio(await CreateBlocks());

//app.Run();

App.Launch(await CreateBlocks(), config => {
    config.Stylesheets = new string[] {
            "https://fonts.font.im/css2?family=Source+Sans+Pro:wght@400;600&display=swap",
            "https://fonts.font.im/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
        };
    //²Î¿¼ facefusion
    config.BodyCss.Add("background_fill_primary", "*neutral_100");

    config.BodyCss.Add("block_background_fill", "white");

    config.BodyCss.Add("block_border_width", "0");

    config.BodyCss.Add("block_label_background_fill", "*primary_100");

    config.BodyCss.Add("block_label_background_fill_dark", "*primary_600");

    config.BodyCss.Add("block_label_border_width", "none");

    config.BodyCss.Add("block_label_margin", "0.5rem");

    config.BodyCss.Add("block_label_radius", "*radius_md");

    config.BodyCss.Add("block_label_text_color", "*primary-500");

    config.BodyCss.Add("block_label_text_color_dark", "white");

    config.BodyCss.Add("block_label_text_weight", "600");

    config.BodyCss.Add("block_title_background_fill", "*primary_100");

    config.BodyCss.Add("block_title_background_fill_dark", "*primary_600");

    config.BodyCss.Add("block_title_padding", "*block_label_padding");

    config.BodyCss.Add("block_title_radius", "*block_label_radius");

    config.BodyCss.Add("block_title_text_color", "*primary_500");

    config.BodyCss.Add("block_title_text_size", "*text_sm");

    config.BodyCss.Add("block_title_text_weight", "600");

    config.BodyCss.Add("block_padding", "0.5rem");

    config.BodyCss.Add("border_color_primary", "transparent");

    config.BodyCss.Add("border_color_primary_dark", "transparent");

    config.BodyCss.Add("button_large_padding", "2rem 0.5rem");

    config.BodyCss.Add("button_large_text_weight", "normal");

    config.BodyCss.Add("button_primary_background_fill", "*primary_500");

    config.BodyCss.Add("button_primary_text_color", "white");

    config.BodyCss.Add("button_secondary_background_fill", "white");

    config.BodyCss.Add("button_secondary_border_color", "transparent");

    config.BodyCss.Add("button_secondary_border_color_dark", "transparent");

    config.BodyCss.Add("button_secondary_border_color_hover", "transparent");

    config.BodyCss.Add("button_secondary_border_color_hover_dark", "transparent");

    config.BodyCss.Add("button_secondary_text_color", "*neutral_800");

    config.BodyCss.Add("button_small_padding", "0.75rem");

    config.BodyCss.Add("checkbox_background_color", "*neutral_200");

    config.BodyCss.Add("checkbox_background_color_selected", "*primary_600");

    config.BodyCss.Add("checkbox_background_color_selected_dark", "*primary_700");

    config.BodyCss.Add("checkbox_border_color_focus", "*primary-500");

    config.BodyCss.Add("checkbox_border_color_focus_dark", "*primary_600");

    config.BodyCss.Add("checkbox_border_color_selected", "*primary_600");

    config.BodyCss.Add("checkbox_border_color_selected_dark", "*primary_700");

    config.BodyCss.Add("checkbox_label_background_fill", "*neutral_50");

    config.BodyCss.Add("checkbox_label_background_fill_hover", "*neutral_50");

    config.BodyCss.Add("checkbox_label_background_fill_selected", "*primary_500");

    config.BodyCss.Add("checkbox_label_background_fill_selected_dark", "*primary_600");

    config.BodyCss.Add("checkbox_label_text_color_selected", "white");

    config.BodyCss.Add("input_background_fill", "*neutral_50");

    config.BodyCss.Add("shadow_drop", "none");

    config.BodyCss.Add("slider_color", "*primary_500");

    config.BodyCss.Add("slider_color_dark", "*primary_600");
});

async Task<Blocks> CreateBlocks()
{
    using (Blocks blocks = gr.Blocks())
    {
        await FirstDemo.Create();

        await MediaDemo.Create();

        await LayoutDemo.Create();

        await ChatbotDemo.Create();

        await FormDemo.Create();

        await ProgressDemo.Create();

        return blocks;
    }
}