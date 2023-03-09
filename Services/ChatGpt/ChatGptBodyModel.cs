namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptBodyModel
{
    public string Model { get; set; }
    public IEnumerable<ChatGptMessageModel> Messages { get; set; }
}