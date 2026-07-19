namespace Gradio.Net.Events;

public class EventArg
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Doc { get; set; }
    public string ComponentProp { get; set; }

    public EventArg(string name, string type, string doc = "", string componentProp = "true")
    {
        Name = name;
        Type = type;
        Doc = doc;
        ComponentProp = componentProp;
    }
}
