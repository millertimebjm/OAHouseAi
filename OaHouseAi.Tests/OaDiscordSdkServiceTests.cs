using OAHouseChatGpt.Services.OADiscord;

namespace OaHouseAi.Tests;

public class OaDiscordSdkServiceTests
{
    [Fact]
    public void RemoveTokenStringTest()
    {
        var removedTokenString = OaDiscordSdkService.RemoveTokenString("Apple (tokens: 22)");
        Assert.Equal("Apple", removedTokenString);
    }

    [Fact]
    public void RemoveMentionStringEncoded()
    {
        var removedMentionString = OaDiscordSdkService.RemoveMentionString(
            "here is a thing \u003C@1082692016426713148\u003E and the thing is here", 
            1082692016426713148.ToString());
        Assert.Equal("here is a thing  and the thing is here", removedMentionString);
    }

    [Fact]
    public void RemoveMentionString()
    {
        var removedMentionString = OaDiscordSdkService.RemoveMentionString(
            "here is a thing <@1082692016426713148> and the thing is here", 
            1082692016426713148.ToString());
        Assert.Equal("here is a thing  and the thing is here", removedMentionString);
    }
}