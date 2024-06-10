using System.Net.WebSockets;
using Moq;
using OaHouseAi.ChatGpt.Services.Interfaces;
using OaHouseAi.ChatGpt.Services;
using OaHouseAi.Configuration.Services.Interfaces;
using OaHouseAi.Configuration.Services;
using OaHouseAi.Discord.Services.Interfaces;
using OaHouseAi.Discord.Services;
using Xunit.Sdk;
using OaHouseAi.Repository.Usages.Interfaces;

namespace OaHouseAi.Tests;

public class OaDiscordServiceTests
{
    [Fact]
    public void RemoveTokenStringTest()
    {
        var removedTokenString = OaDiscordService.RemoveTokenString("Apple (tokens: 22)");
        Assert.Equal("Apple", removedTokenString);
    }

    [Fact]
    public void RemoveMentionStringEncoded()
    {
        var removedMentionString = OaDiscordService.RemoveMentionString(
            "here is a thing \u003C@1082692016426713148\u003E and the thing is here", 
            1082692016426713148.ToString());
        Assert.Equal("here is a thing  and the thing is here", removedMentionString);
    }

    [Fact]
    public void RemoveMentionString()
    {
        var removedMentionString = OaDiscordService.RemoveMentionString(
            "here is a thing <@1082692016426713148> and the thing is here", 
            1082692016426713148.ToString());
        Assert.Equal("here is a thing  and the thing is here", removedMentionString);
    }

    [Fact]
    public async Task SendLongMessage_SingleSend()
    {
        int maxMessageLength = 2000;
        int testMessageLength = 1900;
        var totalTokens = 1000;
        var random = new Random();
        var channelId = Faker.RandomNumber.Next((long)0, long.MaxValue).ToString();
        var referenceMessage = Faker.RandomNumber.Next((long)0, long.MaxValue).ToString();
        var messageContent = string.Join("", Faker.Lorem.Paragraphs(10));
        if (messageContent.Length > testMessageLength) messageContent = messageContent[..testMessageLength];

        var gptServiceMock = new Mock<IChatGpt>();
        var configurationServiceMock = new Mock<IOAHouseChatGptConfiguration>();
        var usageRepositoryMock = new Mock<IUsageRepository>();
        var discordHttpServiceMock = new Mock<IOaDiscordHttp>();
        var clientWebSocketWrapperMock = new Mock<IClientWebSocketWrapper>();
        discordHttpServiceMock.Setup(_ => _.SendMessageAsync(
            channelId.ToString(),
            messageContent,
            referenceMessage.ToString()));
        IOaDiscord discordService = new OaDiscordService(
            gptServiceMock.Object,
            configurationServiceMock.Object,
            usageRepositoryMock.Object,
            discordHttpServiceMock.Object,
            clientWebSocketWrapperMock.Object
        );
        await discordService.SendLongMessage(
            channelId,
            referenceMessage, 
            messageContent, 
            totalTokens,
            maxMessageLength);
        discordHttpServiceMock.Verify(m => m.SendMessageAsync(
            channelId, messageContent + $" (tokens: {totalTokens})", referenceMessage), Times.Exactly(1));
        discordHttpServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendLongMessage_MultipleSend()
    {
        int maxMessageLength = 2000;
        int testMessageLength = 3000;
        var totalTokens = 1000;
        var random = new Random();
        var channelId = Faker.RandomNumber.Next((long)0, long.MaxValue).ToString();
        var referenceMessage = Faker.RandomNumber.Next((long)0, long.MaxValue).ToString();
        var messageContent = string.Join("", Faker.Lorem.Paragraphs(50));
        if (messageContent.Length > testMessageLength) messageContent = messageContent[..testMessageLength];

        var gptServiceMock = new Mock<IChatGpt>();
        var configurationServiceMock = new Mock<IOAHouseChatGptConfiguration>();
        var usageRepositoryMock = new Mock<IUsageRepository>();
        var discordHttpServiceMock = new Mock<IOaDiscordHttp>();
        var clientWebSocketWrapperMock = new Mock<ClientWebSocketWrapper>();
        discordHttpServiceMock.Setup(_ => _.SendMessageAsync(
            channelId.ToString(),
            messageContent,
            referenceMessage.ToString()));
        IOaDiscord discordService = new OaDiscordService(
            gptServiceMock.Object,
            configurationServiceMock.Object,
            usageRepositoryMock.Object,
            discordHttpServiceMock.Object,
            clientWebSocketWrapperMock.Object);
        await discordService.SendLongMessage(
            channelId,
            referenceMessage, 
            messageContent, 
            totalTokens,
            maxMessageLength);
        discordHttpServiceMock.Verify(m => m.SendMessageAsync(
            channelId, messageContent.Substring(0, maxMessageLength), referenceMessage), Times.Exactly(1));
        discordHttpServiceMock.Verify(m => m.SendMessageAsync(
            channelId, messageContent.Substring(maxMessageLength) + $" (tokens: {totalTokens})", referenceMessage), Times.Exactly(1));
        discordHttpServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Identify_VerifySend()
    {
        var gptServiceMock = new Mock<IChatGpt>();
        var configurationServiceMock = new Mock<IOAHouseChatGptConfiguration>();
        var usageRepositoryMock = new Mock<IUsageRepository>();
        var discordHttpServiceMock = new Mock<IOaDiscordHttp>();
        var clientWebSocketWrapperMock = new Mock<IClientWebSocketWrapper>();
        clientWebSocketWrapperMock.Setup(_ => _.SendAsync(
            It.IsAny<ClientWebSocket>(),
            new ArraySegment<byte>(new byte[0]), 
            WebSocketMessageType.Text, 
            true, 
            CancellationToken.None))
            .Returns(Task.CompletedTask);

        IOaDiscord discordService = new OaDiscordService(
            gptServiceMock.Object,
            configurationServiceMock.Object,
            usageRepositoryMock.Object,
            discordHttpServiceMock.Object,
            clientWebSocketWrapperMock.Object);

        await discordService.Identify();
        clientWebSocketWrapperMock.Verify(m => m.SendAsync(
            It.IsAny<ClientWebSocket>(),
            It.IsAny<ArraySegment<byte>>(),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        ), Times.Exactly(1));
        clientWebSocketWrapperMock.VerifyNoOtherCalls();
    }
}