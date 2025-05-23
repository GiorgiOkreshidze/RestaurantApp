﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
using Function.Models.Feedbacks;
using Function.Models.Responses;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Feedbacks;

public class GetLocationFeedbacksAction
{
    private readonly IFeedbackService _feedbackService;
    private const int DefaultPageSize = 20;
    private const int DefaultPage = 0;
    private const string DefaultSortProperty = "date";
    private const string DefaultSortDirection = "asc";

    public GetLocationFeedbacksAction()
    {
        _feedbackService = new FeedbackService();
    }

    public async Task<APIGatewayProxyResponse> GetLocationFeedbacks(APIGatewayProxyRequest request)
    {
        try
        {
            var parameters = ExtractParameters(request);
            var sortOptions = ParseSortParameters(request);
            var (feedbacks, nextToken) = await GetPaginatedFeedbacks(parameters, sortOptions);
            var totalItemCount = await TotalItemCount();

            var responseBody = BuildResponseBody(feedbacks, nextToken, parameters, sortOptions, (int)Math.Ceiling((double)totalItemCount / parameters.PageSize));

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(responseBody),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (Exception e)
        {
            return ActionUtils.FormatResponse(500, new
            {
                message = e.Message
            });
        }
    }

    private LocationFeedbacksParameters ExtractParameters(APIGatewayProxyRequest request)
    {
        var locationId = request.PathParameters["id"];

        if (request.QueryStringParameters is null)
        {
            return new LocationFeedbacksParameters
            {
                LocationId = locationId,
                Type = null,
                Page = DefaultPage,
                PageSize = DefaultPageSize,
                NextPageToken = null
            };
        }

        var typeStr = request.QueryStringParameters.TryGetValue("type", out var tstr) ? tstr : "";
        var pageStr = request.QueryStringParameters.TryGetValue("page", out var pstr) ? pstr : "";
        var sizeStr = request.QueryStringParameters.TryGetValue("size", out var size) ? size : "";
        var nextPageToken = request.QueryStringParameters.TryGetValue("nextPageToken", out var token) ? token : null;

        int pageSize = int.TryParse(sizeStr, out var s) ? s : DefaultPageSize;
        int page = int.TryParse(pageStr, out var p) ? p : DefaultPage;

        LocationFeedbackType? type = null;
        if (!string.IsNullOrEmpty(typeStr))
        {
            type = typeStr.ToLocationFeedbackType();
        }

        return new LocationFeedbacksParameters
        {
            LocationId = locationId,
            Type = type,
            Page = page,
            PageSize = pageSize,
            NextPageToken = nextPageToken
        };
    }

    private (string SortProperty, string SortDirection) ParseSortParameters(APIGatewayProxyRequest request)
    {
        string sortProperty = DefaultSortProperty;
        string sortDirection = DefaultSortDirection;

        if (request.MultiValueQueryStringParameters != null &&
            request.MultiValueQueryStringParameters.TryGetValue("sort", out var sortParams) &&
            sortParams != null && sortParams.Any())
        {
            var sort = sortParams[0].Split(',');

            if (!string.IsNullOrWhiteSpace(sort[0]))
            {
                sortProperty = sort[0];
            }

            if (sort.Length > 1 && !string.IsNullOrWhiteSpace(sort[1]))
            {
                sortDirection = sort[1];
            }
        }

        return (sortProperty, sortDirection);
    }

    private async Task<(List<LocationFeedback> Feedbacks, string? NextToken)> GetPaginatedFeedbacks(
        LocationFeedbacksParameters parameters,
        (string SortProperty, string SortDirection) sortOptions)
    {
        var currentNextToken = parameters.NextPageToken;
        List<LocationFeedback> currentPageFeedbacks = new();

        if (parameters.Page <= 1)
        {
            LocationFeedbackQueryParameters queryParameters = new() 
            { 
                LocationId = parameters.LocationId, 
                Type = parameters.Type, 
                SortProperty = sortOptions.SortProperty, 
                SortDirection = sortOptions.SortDirection, 
                PageSize = parameters.PageSize, 
                NextPageToken = currentNextToken
            };

            var (feedbacks, nextToken) = await _feedbackService.GetLocationFeedbacksAsync(queryParameters);

            currentPageFeedbacks = feedbacks;
            currentNextToken = nextToken;
        }
        else
        {
            currentNextToken = null;

            for (int i = 0; i < parameters.Page; i++)
            {
                LocationFeedbackQueryParameters queryParameters = new()
                {
                    LocationId = parameters.LocationId,
                    Type = parameters.Type,
                    SortProperty = sortOptions.SortProperty,
                    SortDirection = sortOptions.SortDirection,
                    PageSize = parameters.PageSize,
                    NextPageToken = currentNextToken
                };

                var (pageFeedbacks, nextToken) = await _feedbackService.GetLocationFeedbacksAsync(queryParameters);

                if (pageFeedbacks.Count == 0 || nextToken == null)
                {
                    return (new List<LocationFeedback>(), null);
                }

                currentPageFeedbacks = pageFeedbacks;
                currentNextToken = nextToken;
            }
        }

        return (currentPageFeedbacks, currentNextToken);
    }

    private object BuildResponseBody(
        List<LocationFeedback> content,
        string? nextPageToken,
        LocationFeedbacksParameters parameters,
        (string SortProperty, string SortDirection) sortOptions,
        int totalPageCount)
    {
        return new
        {
            size = parameters.PageSize,
            content = content.Select(f => new
            {
                id = f.Id,
                rate = f.Rate,
                comment = f.Comment,
                userName = f.UserName,
                userAvatarUrl = f.UserAvatarUrl,
                date = f.Date,
                type = f.Type.ToString().Replace("_", "-"),
                locationId = f.LocationId
            }).ToList(),
            number = parameters.Page,
            sort = new
            {
                direction = sortOptions.SortDirection.ToUpper(),
                nullHandling = "string",
                ascending = sortOptions.SortDirection.ToLower() == "asc",
                property = sortOptions.SortProperty,
                ignoreCase = true
            },
            first = parameters.Page == 0,
            totalPages = totalPageCount,
            numberOfElements = content.Count,
            pageable = new
            {
                offset = parameters.Page * parameters.PageSize,
                sort =
                    new
                    {
                        direction = sortOptions.SortDirection.ToUpper(),
                        nullHandling = "string",
                        ascending = sortOptions.SortDirection.ToLower() == "asc",
                        property = sortOptions.SortProperty,
                        ignoreCase = true
                    },
                paged = true,
                pageSize = parameters.PageSize,
                pageNumber = parameters.Page,
                unpaged = false
            }
        };
    }

    private async Task<int> TotalItemCount()
    {
        return await _feedbackService.TotalItemCountAsync();
    }
}