using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace ChatService.Configuration;

public class CustomJsonFormatter(JsonValueFormatter valueFormatter = null) : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var buffer = new StringWriter();
        buffer.Write("{");

        buffer.Write("\"Timestamp\":");
        buffer.Write($"\"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\"");

        buffer.Write(",\"Level\":");
        buffer.Write($"\"{logEvent.Level}\"");

        buffer.Write(",\"Message\":");
        buffer.Write($"\"{logEvent.RenderMessage()}\"");
        
        if (logEvent.Exception != null)
        {
            buffer.Write(",\"Exception\":");
            buffer.Write($"\"{logEvent.Exception}\"");
        }

        buffer.Write("}");
        output.WriteLine(buffer.ToString());
    }
}
