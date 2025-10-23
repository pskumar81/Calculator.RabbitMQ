using System.Text.Json.Serialization;

namespace Calculator.Client.Models;

/// <summary>
/// Represents a calculation request message for RabbitMQ communication.
/// Replaces the gRPC CalculateRequest proto message.
/// </summary>
public class CalculationRequest
{
    /// <summary>
    /// Unique identifier for correlating request with response in RabbitMQ
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// The operation to perform: "Add", "Subtract", "Multiply", "Divide"
    /// </summary>
    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// The first number for the calculation
    /// </summary>
    [JsonPropertyName("number1")]
    public double Number1 { get; set; }

    /// <summary>
    /// The second number for the calculation
    /// </summary>
    [JsonPropertyName("number2")]
    public double Number2 { get; set; }

    /// <summary>
    /// The queue name where the response should be sent
    /// </summary>
    [JsonPropertyName("replyTo")]
    public string ReplyTo { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the request was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}