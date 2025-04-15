namespace Demo.AspNetCore.Launcher;

class GradioBlockLoader
{
    public static async Task<Blocks> Load(IEnumerable<IGradioBlockBuilder> builders)
    {
        using (Blocks blocks = gr.Blocks())
        {
            foreach (var builder in builders)
            {
                await builder.Build();
            }
            return blocks;
        }
    }
}
