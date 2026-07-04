using System;

namespace IngilizceProjeMAUI.Models
{
    public class ChatMessage
    {
        public bool IsUserMessage { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
