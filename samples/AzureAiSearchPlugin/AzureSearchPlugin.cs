using Microsoft.SemanticKernel;
using System.ComponentModel;

public enum SearchType
{
    KnowledgeBase,
    Product
}

/// <summary>
/// Plugin for searching documents using synthetic data based on search type
/// </summary>
[Description("Plugin for searching documents using synthetic data.")]
public class AzureSearchPlugin
{
    private readonly SearchType _searchType;
    private readonly Random _random;

    private readonly List<Dictionary<string, object>> _knowledgeBaseData = new()
    {
        new Dictionary<string, object>
        {
            ["id"] = "kb001",
            ["title"] = "REST API Authentication Guide",
            ["content"] = "Complete guide to authenticating with our REST API using API keys, OAuth 2.0, and JWT tokens. Includes code examples for multiple programming languages.",
            ["category"] = "API Documentation",
            ["tags"] = "api, authentication, rest, oauth, jwt, integration",
            ["dateCreated"] = "2024-01-15",
            ["author"] = "API Documentation Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb002",
            ["title"] = "Billing and Payment Methods FAQ",
            ["content"] = "Frequently asked questions about billing cycles, payment methods, invoicing, and subscription management. Learn about credit card payments, wire transfers, and enterprise billing options.",
            ["category"] = "Billing",
            ["tags"] = "billing, payment, faq, subscription, invoice, credit card",
            ["dateCreated"] = "2024-02-10",
            ["author"] = "Billing Support Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb003",
            ["title"] = "Azure Integration Tutorial: Getting Started",
            ["content"] = "Step-by-step tutorial for integrating your applications with Azure services. Covers Azure Active Directory, Storage, and Cognitive Services integration patterns.",
            ["category"] = "Tutorial",
            ["tags"] = "azure, integration, tutorial, active directory, storage, cognitive services",
            ["dateCreated"] = "2024-03-05",
            ["author"] = "Azure Integration Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb004",
            ["title"] = "Advanced Azure SDK Integration Patterns",
            ["content"] = "Learn advanced patterns for integrating with Azure using the latest SDKs. Includes retry policies, connection pooling, and performance optimization techniques.",
            ["category"] = "Tutorial",
            ["tags"] = "azure, sdk, integration, patterns, performance, optimization",
            ["dateCreated"] = "2024-04-12",
            ["author"] = "Azure SDK Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb005",
            ["title"] = "API Rate Limiting and Best Practices",
            ["content"] = "Understanding API rate limits, implementing proper retry logic, and following best practices for API integration to ensure reliable service.",
            ["category"] = "API Documentation",
            ["tags"] = "api, rate limiting, best practices, retry, integration, reliability",
            ["dateCreated"] = "2024-05-08",
            ["author"] = "API Documentation Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb006",
            ["title"] = "Payment Processing and Subscription Management",
            ["content"] = "Detailed guide on how payment processing works, managing subscriptions, handling failed payments, and understanding billing cycles.",
            ["category"] = "Billing",
            ["tags"] = "payment processing, subscription, billing cycle, failed payment, management",
            ["dateCreated"] = "2024-05-20",
            ["author"] = "Billing Support Team"
        },
        new Dictionary<string, object>
        {
            ["id"] = "kb007",
            ["title"] = "Azure Service Bus Integration Guide",
            ["content"] = "Complete tutorial on integrating with Azure Service Bus for reliable messaging patterns in distributed applications.",
            ["category"] = "Tutorial",
            ["tags"] = "azure, service bus, messaging, integration, distributed, tutorial",
            ["dateCreated"] = "2024-06-01",
            ["author"] = "Azure Integration Team"
        }
    };

    private readonly List<Dictionary<string, object>> _productData = new()
    {
        new Dictionary<string, object>
        {
            ["id"] = "prod001",
            ["name"] = "Dell XPS 13 Laptop",
            ["description"] = "Ultra-portable laptop perfect for college students with 13.3\" display, Intel Core i7, 16GB RAM, 512GB SSD",
            ["price"] = "$1,299.99",
            ["category"] = "Laptops",
            ["brand"] = "Dell",
            ["rating"] = "4.5/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod002",
            ["name"] = "MacBook Air M2",
            ["description"] = "Lightweight laptop ideal for students with Apple M2 chip, 13.6\" Liquid Retina display, 8GB unified memory, 256GB SSD",
            ["price"] = "$1,199.00",
            ["category"] = "Laptops",
            ["brand"] = "Apple",
            ["rating"] = "4.7/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod003",
            ["name"] = "HP Pavilion 15 Student Laptop",
            ["description"] = "Budget-friendly laptop for college with 15.6\" display, AMD Ryzen 5, 8GB RAM, 256GB SSD, perfect for everyday tasks",
            ["price"] = "$649.99",
            ["category"] = "Laptops",
            ["brand"] = "HP",
            ["rating"] = "4.2/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod004",
            ["name"] = "Razer BlackWidow V3 Gaming Keyboard",
            ["description"] = "Mechanical gaming keyboard with Razer Green switches, RGB lighting, and programmable keys for competitive gaming",
            ["price"] = "$139.99",
            ["category"] = "Gaming Accessories",
            ["brand"] = "Razer",
            ["rating"] = "4.6/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod005",
            ["name"] = "Logitech G Pro X Gaming Mouse",
            ["description"] = "Ultra-lightweight wireless gaming mouse with HERO 25K sensor, designed for esports professionals",
            ["price"] = "$149.99",
            ["category"] = "Gaming Accessories",
            ["brand"] = "Logitech",
            ["rating"] = "4.8/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod006",
            ["name"] = "Corsair K65 RGB Mini Gaming Keyboard",
            ["description"] = "Compact 65% mechanical gaming keyboard with Cherry MX switches and dynamic RGB lighting",
            ["price"] = "$109.99",
            ["category"] = "Gaming Accessories",
            ["brand"] = "Corsair",
            ["rating"] = "4.4/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod007",
            ["name"] = "Sony WH-1000XM4 Wireless Headphones",
            ["description"] = "Premium noise-canceling wireless headphones perfect for workouts and travel with 30-hour battery life",
            ["price"] = "$279.99",
            ["category"] = "Wireless Headphones",
            ["brand"] = "Sony",
            ["rating"] = "4.9/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod008",
            ["name"] = "Bose QuietComfort Earbuds",
            ["description"] = "True wireless earbuds with world-class noise cancellation, perfect for workouts and active lifestyles",
            ["price"] = "$199.99",
            ["category"] = "Wireless Headphones",
            ["brand"] = "Bose",
            ["rating"] = "4.6/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod009",
            ["name"] = "Jabra Elite 75t Wireless Earbuds",
            ["description"] = "Compact wireless earbuds with secure fit, great for workouts and daily use with 7.5-hour battery life",
            ["price"] = "$149.99",
            ["category"] = "Wireless Headphones",
            ["brand"] = "Jabra",
            ["rating"] = "4.3/5",
            ["availability"] = "In Stock"
        },
        new Dictionary<string, object>
        {
            ["id"] = "prod010",
            ["name"] = "SteelSeries Apex Pro Gaming Keyboard",
            ["description"] = "Premium gaming keyboard with adjustable mechanical switches and OLED smart display for customization",
            ["price"] = "$199.99",
            ["category"] = "Gaming Accessories",
            ["brand"] = "SteelSeries",
            ["rating"] = "4.7/5",
            ["availability"] = "In Stock"
        }
    };

    public AzureSearchPlugin(SearchType searchType)
    {
        _searchType = searchType;
        _random = new Random();
    }

    [KernelFunction, Description("Searches for documents based on a query and search type.")]
    public async Task<string> Search(
        [Description("The search query to find relevant documents")] string query,
        [Description("Maximum number of results to return")] int maxResults = 10
    )
    {
        await Task.Delay(100); // Simulate async operation

        var dataSource = _searchType == SearchType.KnowledgeBase ? _knowledgeBaseData : _productData;
        var shuffledData = dataSource.OrderBy(x => _random.Next()).ToList();
        
        // Limit results to maxResults
        var selectedResults = shuffledData.Take(Math.Min(maxResults, dataSource.Count)).ToList();
        var results = new List<string>();

        foreach (var item in selectedResults)
        {
            var score = Math.Round(_random.NextDouble() * 0.5 + 0.5, 2); // Random score between 0.5 and 1.0
            var resultText = $"Relevance Score: {score:F2}\n";
            
            foreach (var field in item)
            {
                if (field.Value != null)
                {
                    resultText += $"{field.Key}: {field.Value}\n";
                }
            }
            
            results.Add(resultText);
        }

        return results.Count > 0 
            ? $"Found {results.Count} {_searchType} documents for query '{query}':\n\n" + string.Join("\n", results)
            : $"No {_searchType.ToString().ToLower()} documents found matching your query.";
    }
}
