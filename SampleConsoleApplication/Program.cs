using OpenAI.Responses;
using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

#pragma warning disable OPENAI001

IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// Azure Foundry Client

Console.WriteLine("Starting the AzureFoundry client...");
Console.WriteLine("--AzureFoundry--");

const string deploymentName = "gpt-4.1-nano";
string azureFoundryEndPointWithVersion = configuration["AzureFoundry:EndPointWithVersion"] ?? throw new InvalidOperationException("AzureFoundry:EndPoint is not configured.");
string azureFoundryEndPointProject = configuration["AzureFoundry:EndPointProject"] ?? throw new InvalidOperationException("AzureFoundry:EndPointProject is not configured.");
string azureFoundryApiKey = configuration["AzureFoundry:ApiKey"] ?? throw new InvalidOperationException("AzureFoundry:ApiKey is not configured.");
// OpenAi
string openAiApiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
// AzureOpenAI
string azureOpenAiEndPoint = configuration["AzureOpenAI:EndPoint"] ?? throw new InvalidOperationException("AzureOpenAI:EndPoint is not configured.");
string azureOpenAiApiKey = configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured.");

ResponsesClient client = new(
    credential: new ApiKeyCredential(azureFoundryApiKey),
    options: new ResponsesClientOptions()
    {
        Endpoint = new Uri($"{azureFoundryEndPointWithVersion}"),
    });
CreateResponseOptions options = new()
{
    Model = deploymentName,   // "gpt-4.1-nano"
    InputItems =
    {
        ResponseItem.CreateUserMessageItem("What's the weather like today for Stamford, Connecticut?"),
    },
};

ResponseResult response = client.CreateResponse(options);

Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");

Console.ReadLine();

// OpenAI Client
Console.WriteLine("Starting the OpenAi client...");
Console.WriteLine("--OpenAi--");

OpenAIClient openAiClient = new OpenAIClient(openAiApiKey);

ChatClientAgent openAiAgent = openAiClient.GetChatClient(deploymentName).AsAIAgent();

AgentResponse openAiResponse = await openAiAgent.RunAsync("What is the capital of France?");

Console.WriteLine(openAiResponse);

Console.ReadLine();

// AzureOpenAI

Console.WriteLine("Starting the AzureOpenAi client...");
Console.WriteLine("--AzureOpenAi--");

AzureOpenAIClient azureFoundryClient = new AzureOpenAIClient(new Uri(azureOpenAiEndPoint), new ApiKeyCredential(azureOpenAiApiKey));  

ChatClientAgent azureFoundryAgent = azureFoundryClient.GetChatClient(deploymentName).AsAIAgent();

AgentResponse azureFondryAgentResponse = await azureFoundryAgent.RunAsync("What is the capital of France?");

Console.WriteLine(azureFondryAgentResponse);

Console.ReadLine();