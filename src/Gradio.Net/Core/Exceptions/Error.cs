namespace Gradio.Net.Core.Exceptions;

public class Error : Exception
{
    public double? Duration { get; set; }

    public bool Visible { get; set; }

    public string Title { get; set; }

    public bool PrintException { get; set; }

    public Error() : base()
    {
        InitializeDefaults();
    }

    public Error(string message) : base(message)
    {
        InitializeDefaults();
    }

    public Error(string message, Exception innerException) : base(message, innerException)
    {
        InitializeDefaults();
    }

    public Error(string message, double? duration, bool visible, string title, bool printException) : base(message)
    {
        Duration = duration;
        Visible = visible;
        Title = title;
        PrintException = printException;
    }

    private void InitializeDefaults()
    {
        Duration = 10;
        Visible = true;
        Title = "Error";
        PrintException = true;
    }

    public override string ToString()
    {
        return Message;
    }
}
