using OpenAI.Responses;
using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

#pragma warning disable OPENAI001

IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// Azure Foundry Client

Console.WriteLine("Starting the AzureFoundry client...");


const string deploymentName = "gpt-4.1-nano";
string azureFoundryEndPointWithVersion = configuration["AzureFoundry:EndPointWithVersion"] ?? throw new InvalidOperationException("AzureFoundry:EndPoint is not configured.");
string azureFoundryEndPointProject = configuration["AzureFoundry:EndPointProject"] ?? throw new InvalidOperationException("AzureFoundry:EndPointProject is not configured.");
string azureFoundryApiKey = configuration["AzureFoundry:ApiKey"] ?? throw new InvalidOperationException("AzureFoundry:ApiKey is not configured.");
// OpenAi
string openAiApiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
// AzureOpenAI
string azureOpenAiEndPoint = configuration["AzureOpenAI:EndPoint"] ?? throw new InvalidOperationException("AzureOpenAI:EndPoint is not configured.");
string azureOpenAiApiKey = configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured.");


/*
 * Steps:
 * 1: Create a 'Microsoft Foundry' Resource + Deploy Model
 * 2: Add Nuget Packages (Microsoft.Agents.AI.OpenAi)
 * 3: Create an OpenAiClient with Endpoint (API Key or Azure Identity)
 * 4: Get a GetResponsesClient and Create and AI Agent from it (model is mandatory)
 * 5: Call RunAsync or RunStreamingAsync
 */

/*
 * Steps:
 * 1: Create a 'Microsoft Foundry' Resource + Deploy Model
 * 2: Add Nuget Packages (Microsoft.Agents.AI.Foundry)
 * 3: Create an AIProjectClient with Endpoint and AzureCli
 * 4: Create an AI Agent from the client (instructions _ model are mandatory)
 * 5: Call RunAsync or RunStreamingAsync
 */

//InitializeAzureFoundryClient(azureFoundryApiKey, azureFoundryEndPointWithVersion, deploymentName);
//await InitializeOpenAiClient(openAiApiKey, deploymentName);
//await InitializeAzureOpenAiClient(azureOpenAiEndPoint, azureOpenAiApiKey, deploymentName);

void InitializeAzureFoundryClient(string azureFoundryApiKey, string azureFoundryEndPointWithVersion1, string deploymentName2)
{
    Console.WriteLine("--AzureFoundry--");
    ResponsesClient client = new(
        credential: new ApiKeyCredential(azureFoundryApiKey),
        options: new ResponsesClientOptions()
        {
            Endpoint = new Uri($"{azureFoundryEndPointWithVersion1}"),
        });
    CreateResponseOptions options = new()
    {
        Model = deploymentName2,   // "gpt-4.1-nano"
        InputItems =
        {
            ResponseItem.CreateUserMessageItem("What is the capital of France?"),
        },
    };

    ResponseResult response = client.CreateResponse(options);

    Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");

    Console.ReadLine();
}

async Task InitializeOpenAiClient(string openAiApiKey, string deploymentName1)
{
    // OpenAI Client
    Console.WriteLine("Starting the OpenAi client...");
    Console.WriteLine("--OpenAi--");

    OpenAIClient openAiClient = new OpenAIClient(openAiApiKey);

    ChatClientAgent openAiAgent = openAiClient.GetChatClient(deploymentName1).AsAIAgent();

    AgentResponse openAiResponse = await openAiAgent.RunAsync("What is the capital of France?");

    Console.WriteLine(openAiResponse);

     await foreach(AgentResponseUpdate update in openAiAgent.RunStreamingAsync("How to make soup?"))
     {
         Console.Write(update);
     }

    Console.ReadLine();
}

async Task InitializeAzureOpenAiClient(string azureOpenAiEndPoint, string azureOpenAiApiKey, string deploymentName)
{
    // AzureOpenAI

    Console.WriteLine("Starting the AzureOpenAi client...");
    Console.WriteLine("--AzureOpenAi--");

    AzureOpenAIClient azureOpenAiClient = new AzureOpenAIClient(new Uri(azureOpenAiEndPoint), new ApiKeyCredential(azureOpenAiApiKey));  

    ChatClientAgent azureOpenAiAgent = azureOpenAiClient.GetChatClient(deploymentName).AsAIAgent();

    AgentResponse azureOpenAiAgentResponse = await azureOpenAiAgent.RunAsync("What is the capital of France?");

    Console.WriteLine(azureOpenAiAgentResponse);

    await foreach (AgentResponseUpdate update in azureOpenAiAgent.RunStreamingAsync("How to make soup?"))
    {
        Console.Write(update);
    }

    Console.ReadLine();
}

