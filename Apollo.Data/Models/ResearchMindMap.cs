using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Apollo.Data.Models;

public class ResearchMindMap
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Research))]
    public Guid ResearchId { get; set; }
    public Research? Research { get; set; }

    // Store the serialized MindMapNode graph in a JSONB column
    [Column(TypeName = "jsonb")]
    public string GraphData { get; set; } = "{}";

    // Helper methods for serialization/deserialization
    public void SetGraphData(MindMapNode rootNode)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        GraphData = JsonSerializer.Serialize(rootNode, options);
    }

    public MindMapNode? GetGraphData()
    {
        if (string.IsNullOrEmpty(GraphData) || GraphData == "{}")
            return null;

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Deserialize<MindMapNode>(GraphData, options);
        }
        catch (NotSupportedException ex)
        {
            // TODO: Use proper logging (ILogger) in production
            Console.WriteLine(
                $"Failed to deserialize GraphData (type discriminator missing): {ex.Message}"
            );
            return null;
        }
        catch (JsonException ex)
        {
            // TODO: Use proper logging (ILogger) in production
            Console.WriteLine($"JSON parsing error in GraphData: {ex.Message}");
            return null;
        }
    }
}
