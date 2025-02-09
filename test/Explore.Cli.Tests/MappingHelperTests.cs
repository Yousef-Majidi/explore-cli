using System.Text.Json;
using Explore.Cli.Models;
using Spectre.Console;

namespace Explore.Cli.Tests;

public class MappingHelperTests
{
    [Fact]
    public void CollectionEntriesNotLimitedToSoap_ShouldReturnFalseForNull()
    {
        bool expected = false;
        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_FalseWhenOnlySoap()
    {
        bool expected = false;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "SOAP"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_FalseWhenOnlyWSDL()
    {
        bool expected = false;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "WSDL"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CollectionEntriesNotLimitedToSoap_TrueWhenOther()
    {
        bool expected = true;

        List<CollectionEntry> setupCollection = new List<CollectionEntry>()
        {
            new CollectionEntry()
            {
                Type = "OTHER"
            }
        };

        var actual = MappingHelper.CollectionEntriesNotLimitedToSoap(setupCollection);

        Assert.Equal(expected, actual);
    } 

    [Theory]
    [InlineData("XSRF-TOKEN=be05885a-41fc-4820-83fb-5db17015ed4a", "be05885a-41fc-4820-83fb-5db17015ed4a")]
    [InlineData("xsrf-token=dd3424c9-17ec-4b20-a89c-ca89d98bbd3b", "dd3424c9-17ec-4b20-a89c-ca89d98bbd3b")]
    [InlineData("Xsrf-Token=dd3424c9-17ec-4b20-a89c-ca89d98bbd3b", "dd3424c9-17ec-4b20-a89c-ca89d98bbd3b")]
    [InlineData("bf936dc3-6c70-43a0-a4c5-ddb42569a9c8", null)]
    public void ExtractXSRFTokenFromCookie_Tests(string cookie, string expected)
    {
        var actual = MappingHelper.ExtractXSRFTokenFromCookie(cookie);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MapEntryBodyToContentExamples_ShouldMapJson()
    {
        var expected = "{\n    \"owner\": \"frank-kilcommins\",\n    \"name\": \"Common Domains\",\n    \"description\": \"common components for all APIs\",\n    \"apis\": [],\n    \"domains\": [\n        \"Problem\",\n        \"ErrorResponses\"\n    ]\n}";
        var actual = MappingHelper.MapEntryBodyToContentExamples(expected);

        Assert.Equal(expected, actual.Example?.Value);
    }

    [Fact]
    public void MapEntryBodyToContentExamples_ShouldMapXML()
    {
        var expected = "<soap:Envelope\n    xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\"\n    xmlns:tem=\"http://tempuri.org/\">\n    <soap:Header/>\n    <soap:Body>\n        <tem:Multiply>\n            <tem:intA>10</tem:intA>\n            <tem:intB>10</tem:intB>\n        </tem:Multiply>\n    </soap:Body>\n</soap:Envelope>";
        var actual = MappingHelper.MapEntryBodyToContentExamples(expected);

        Assert.Equal(expected, actual.Example?.Value);
    }

    [Fact]
    public void MapInspectorParamsToExploreParams_ShouldMapHeadersParams()
    {
        Models.Parameter expectedHeader = new Models.Parameter()
        {
            In = "header",
            Name = "Authorization",
            Examples = new Models.Examples()
            {
                Example = new Models.Example()
                {
                    Value = "0648454d-8307-40c9-b0ac-73fa4a48354e"
                }
            }
        };

        var entryAsJsonString = @"      {
        ""_id"": {
          ""timestamp"": 1684423109,
          ""counter"": 13169316,
          ""time"": 1684423109000,
          ""date"": ""2023-05-18T15:18:29Z"",
          ""machineIdentifier"": 8669634,
          ""processIdentifier"": 30503,
          ""timeSecond"": 1684423109
        },
        ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HistoryEntry"",
        ""userId"": ""59bff4e3-89be-4078-bf9c-76cff2b9f2dc"",
        ""endpoint"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net/api/payees?country_of_registration=IE&name=ltd"",
        ""uri"": {
          ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.URI"",
          ""scheme"": ""https"",
          ""host"": ""sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
          ""path"": ""/api/payees"",
          ""query"": ""country_of_registration=IE&name=ltd""
        },
        ""method"": ""GET"",
        ""body"": ""{\n    \""owner\"": \""frank-kilcommins\"",\n    \""name\"": \""Common Domains\"",\n    \""description\"": \""common components for all APIs\"",\n    \""apis\"": [],\n    \""domains\"": [\n        \""Problem\"",\n        \""ErrorResponses\""\n    ]\n}"",
        ""authentication"": """",
        ""headers"": [
          {
            ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HeaderWithValue"",
            ""name"": ""Authorization"",
            ""value"": ""0648454d-8307-40c9-b0ac-73fa4a48354e""
          }
        ],
        ""type"": ""OTHER"",
        ""entryId"": ""f5f0874f-d656-4b3f-9707-b879fadd7e8c"",
        ""timestamp"": ""2023-05-18T15:18:37Z"",
        ""ciHost"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
        ""name"": ""FinTech Workshop Request 0""
      }";

        CollectionEntry? entry = JsonSerializer.Deserialize<CollectionEntry>(entryAsJsonString);
        var result = MappingHelper.MapInspectorParamsToExploreParams(entry == null ? new CollectionEntry(){} : entry);

        Assert.Equal(1, result.Count(x => x.Name?.ToLowerInvariant() == "authorization"));
        Assert.Equal(1, result.Count(x => x.In == expectedHeader.In && x.Name == expectedHeader.Name && x.Examples?.Example?.Value == expectedHeader.Examples.Example.Value));      

    }

    [Fact]
    public void MapInspectorParamsToExploreParams_ShouldMapQueryParams()
    {
        Models.Parameter expectedQuery = new Models.Parameter()
        {
            In = "query",
            Name = "name",
            Examples = new Models.Examples()
            {
                Example = new Models.Example()
                {
                    Value = "ltd"
                }
            }
        };

        var entryAsJsonString = @"      {
        ""_id"": {
          ""timestamp"": 1684423109,
          ""counter"": 13169316,
          ""time"": 1684423109000,
          ""date"": ""2023-05-18T15:18:29Z"",
          ""machineIdentifier"": 8669634,
          ""processIdentifier"": 30503,
          ""timeSecond"": 1684423109
        },
        ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HistoryEntry"",
        ""userId"": ""59bff4e3-89be-4078-bf9c-76cff2b9f2dc"",
        ""endpoint"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net/api/payees?country_of_registration=IE&name=ltd"",
        ""uri"": {
          ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.URI"",
          ""scheme"": ""https"",
          ""host"": ""sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
          ""path"": ""/api/payees"",
          ""query"": ""country_of_registration=IE&name=ltd""
        },
        ""method"": ""GET"",
        ""body"": ""{\n    \""owner\"": \""frank-kilcommins\"",\n    \""name\"": \""Common Domains\"",\n    \""description\"": \""common components for all APIs\"",\n    \""apis\"": [],\n    \""domains\"": [\n        \""Problem\"",\n        \""ErrorResponses\""\n    ]\n}"",
        ""authentication"": """",
        ""headers"": [
          {
            ""modelClass"": ""com.smartbear.readyapi.inspector.services.repository.models.HeaderWithValue"",
            ""name"": ""Authorization"",
            ""value"": ""0648454d-8307-40c9-b0ac-73fa4a48354e""
          }
        ],
        ""type"": ""OTHER"",
        ""entryId"": ""f5f0874f-d656-4b3f-9707-b879fadd7e8c"",
        ""timestamp"": ""2023-05-18T15:18:37Z"",
        ""ciHost"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net"",
        ""name"": ""FinTech Workshop Request 0""
      }";

        CollectionEntry? entry = JsonSerializer.Deserialize<CollectionEntry>(entryAsJsonString);
        var result = MappingHelper.MapInspectorParamsToExploreParams(entry == null ? new CollectionEntry(){} : entry);

        Assert.Equal(1, result.Count(x => x.Name?.ToLowerInvariant() == "name"));
        Assert.Equal(1, result.Count(x => x.In == expectedQuery.In && x.Name == expectedQuery.Name && x.Examples?.Example?.Value == expectedQuery.Examples.Example.Value));      

    }    

    [Fact]
    public void MapInspectorAuthenticationToCredentials_BasicAuth()
    {
        var input = "Basic Authentication/username:password";
        var expected = new Credentials() { Type = "BasicAuthCredentials", Username = "username", Password = "password" };
        var actual = MappingHelper.MapInspectorAuthenticationToCredentials(input);

        Assert.Equal(expected.Type, actual?.Type);
        Assert.Equal(expected.Username, actual?.Username);
        Assert.Equal(expected.Password, actual?.Password);
    }

    [Fact]
    public void MapInspectorAuthenticationToCredentials_TokenAuth()
    {
        var input = "OAuth 2.0/0648454d-8307-40c9-b0ac-73fa4a48354e";
        var expected = new Credentials() { Type = "TokenCredentials", Token = "0648454d-8307-40c9-b0ac-73fa4a48354e"};
        var actual = MappingHelper.MapInspectorAuthenticationToCredentials(input);

        Assert.Equal(expected.Type, actual?.Type);
        Assert.Equal(expected.Token, actual?.Token);
    }    

    [Fact]
    public void MapConnectionServersToServerList()
    {
        var entryAsJsonString = @"{
                ""id"": ""b11a0d72-bb2c-454f-8f19-3d501a7ac65f"",
                ""name"": ""REST"",
                ""schema"": ""OpenAPI"",
                ""schemaVersion"": ""3.0.1"",
                ""connectionDefinition"": {
                ""info"": {
                    ""title"": ""app1"",
                    ""version"": ""version2""
                },
                ""tags"": [],
                ""paths"": {
                    ""/api/payees"": {
                    ""get"": {
                        ""responses"": [],
                        ""parameters"": [
                        {
                            ""in"": ""query"",
                            ""name"": ""country_of_registration"",
                            ""examples"": {
                            ""example"": {
                                ""value"": ""IE""
                            }
                            }
                        },
                        {
                            ""in"": ""query"",
                            ""name"": ""name"",
                            ""examples"": {
                            ""example"": {
                                ""value"": ""ltd""
                            }
                            }
                        }
                        ]
                    }
                    }
                },
                ""openapi"": ""3.0.0"",
                ""servers"": [
                    {
                    ""url"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net""
                    }
                ]
                },
                ""settings"": {
                ""type"": ""RestConnectionSettings"",
                ""encodeUrl"": true,
                ""connectTimeout"": 30,
                ""followRedirects"": true
                },
                ""credentials"": null,
                ""_links"": {
                ""self"": {
                    ""href"": ""https://api.explore.swaggerhub.com/spaces-api/v1/spaces/dd4ad781-a47a-4dd5-84c1-5c799aa8e1b8/apis/89907a82-5047-499f-8236-ea796935248d/connections/b11a0d72-bb2c-454f-8f19-3d501a7ac65f""
                }
                }
            }";

        var expected = new Connection() {Id = "b11a0d72-bb2c-454f-8f19-3d501a7ac65f", 
                ConnectionDefinition = new ConnectionDefinition() {
                    Servers = new List<Server>() {
                        new Server() { Url = "https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net" }
                    } }
            };

        var actual = JsonSerializer.Deserialize<Connection>(entryAsJsonString);

        Assert.Equal(expected.Id, actual?.Id);
        Assert.Equal(expected.ConnectionDefinition.Servers.FirstOrDefault()?.Url, actual?.ConnectionDefinition?.Servers?.FirstOrDefault()?.Url);
    }

    [Fact]
    public void MapPagedConnectionsJsonToObjects()
    {
        var entryAsJsonString = @"{
            ""_embedded"": {
                ""connections"": [
                {
                    ""id"": ""b11a0d72-bb2c-454f-8f19-3d501a7ac65f"",
                    ""name"": ""REST"",
                    ""schema"": ""OpenAPI"",
                    ""schemaVersion"": ""3.0.1"",
                    ""connectionDefinition"": {
                    ""info"": {
                        ""title"": ""app1"",
                        ""version"": ""version2""
                    },
                    ""tags"": [],
                    ""paths"": {
                        ""/api/payees"": {
                        ""get"": {
                            ""responses"": [],
                            ""parameters"": [
                            {
                                ""in"": ""query"",
                                ""name"": ""country_of_registration"",
                                ""examples"": {
                                ""example"": {
                                    ""value"": ""IE""
                                }
                                }
                            },
                            {
                                ""in"": ""query"",
                                ""name"": ""name"",
                                ""examples"": {
                                ""example"": {
                                    ""value"": ""ltd""
                                }
                                }
                            }
                            ]
                        }
                        }
                    },
                    ""openapi"": ""3.0.0"",
                    ""servers"": [
                        {
                        ""url"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net""
                        }
                    ]
                    },
                    ""settings"": {
                    ""type"": ""RestConnectionSettings"",
                    ""encodeUrl"": true,
                    ""connectTimeout"": 30,
                    ""followRedirects"": true
                    },
                    ""credentials"": null,
                    ""_links"": {
                    ""self"": {
                        ""href"": ""https://api.explore.swaggerhub.com/spaces-api/v1/spaces/dd4ad781-a47a-4dd5-84c1-5c799aa8e1b8/apis/89907a82-5047-499f-8236-ea796935248d/connections/b11a0d72-bb2c-454f-8f19-3d501a7ac65f""
                    }
                    }
                },
                {
                    ""id"": ""8184596a-bbee-4c2e-a050-a7ef74d4e3bc"",
                    ""name"": ""REST"",
                    ""schema"": ""OpenAPI"",
                    ""schemaVersion"": ""3.0.1"",
                    ""connectionDefinition"": {
                    ""info"": {
                        ""title"": ""app1"",
                        ""version"": ""version2""
                    },
                    ""tags"": [],
                    ""paths"": {
                        ""/api/payees"": {
                        ""get"": {
                            ""responses"": [],
                            ""parameters"": [
                            {
                                ""in"": ""query"",
                                ""name"": ""country_of_registration"",
                                ""examples"": {
                                ""example"": {
                                    ""value"": ""IT""
                                }
                                }
                            },
                            {
                                ""in"": ""query"",
                                ""name"": ""jurisdiction_identifier_type"",
                                ""examples"": {
                                ""example"": {
                                    ""value"": ""fiscal-code""
                                }
                                }
                            }
                            ]
                        }
                        }
                    },
                    ""openapi"": ""3.0.0"",
                    ""servers"": [
                        {
                        ""url"": ""https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net""
                        }
                    ]
                    },
                    ""settings"": {
                    ""type"": ""RestConnectionSettings"",
                    ""encodeUrl"": true,
                    ""connectTimeout"": 30,
                    ""followRedirects"": true
                    },
                    ""credentials"": null,
                    ""_links"": {
                    ""self"": {
                        ""href"": ""https://api.explore.swaggerhub.com/spaces-api/v1/spaces/dd4ad781-a47a-4dd5-84c1-5c799aa8e1b8/apis/89907a82-5047-499f-8236-ea796935248d/connections/8184596a-bbee-4c2e-a050-a7ef74d4e3bc""
                    }
                    }
                }
                ]
            },
            ""_links"": {
                ""self"": {
                ""href"": ""https://api.explore.swaggerhub.com/spaces-api/v1/spaces/dd4ad781-a47a-4dd5-84c1-5c799aa8e1b8/apis/89907a82-5047-499f-8236-ea796935248d/connections?page=0&size=2000""
                }
            },
            ""page"": {
                ""size"": 2000,
                ""totalElements"": 2,
                ""totalPages"": 1,
                ""number"": 0
            }
            }";


        var actual = JsonSerializer.Deserialize<PagedConnections>(entryAsJsonString);            

        Assert.Equal(2, actual?.Embedded?.Connections?.Count());
        Assert.Equal("https://sbdevrel-fua-smartbearcoin-prd.azurewebsites.net", actual?.Embedded?.Connections?.FirstOrDefault(c => c.Id == "b11a0d72-bb2c-454f-8f19-3d501a7ac65f")?.ConnectionDefinition?.Servers?.FirstOrDefault()?.Url);
    }

    [Fact]
    public static void MassageConnectionExportForImport_Should_Pass()
    {
        var sut = new Connection();

        var act = MappingHelper.MassageConnectionExportForImport(sut);

        Assert.Equal("ConnectionRequest", sut.Type);
    }
}