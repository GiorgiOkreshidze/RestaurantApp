# Define variables
$region = "eu-west-2"
$prefix = "tm2-"
$syndicateFile = "../../.syndicate-config-dev/syndicate.yml"
$feedbacksSeedFile = "../seed-data-feedbacks.json"
$tempFile = "seed-data-processed.json"

# Get the suffix from syndicate.yml
$resourcesSuffix = (Get-Content $syndicateFile | Where-Object { $_ -match "resources_suffix:" }) -replace "resources_suffix:\s*", ""
if (-not $resourcesSuffix) {
    $resourcesSuffix = "-dev5"
    Write-Host "No suffix found in syndicate.yml, defaulting to $resourcesSuffix"
}

# Construct table names
$feedbacksTable = "$prefix" + "LocationFeedbacks" + "$resourcesSuffix"

# Check if seed files exist
if (-not (Test-Path $feedbacksSeedFile)) {
    Write-Error "Seed file $feedbacksSeedFile not found!"
    exit 1
}


# Process JSON: Update table names
try {
    # Read JSON files and extract the inner arrays
    $feedbacksData = Get-Content $feedbacksSeedFile -Raw | ConvertFrom-Json

    # Extract the 'Locations' array
    $feedbacksItems = $feedbacksData.LocationFeedbacks

    # Create the request structure
    $requestItems = @{
        "$feedbacksTable" = $feedbacksItems
    }

    # Write to temp file without BOM
    [System.IO.File]::WriteAllText($tempFile, ($requestItems | ConvertTo-Json -Depth 10))
    Write-Host "Temp file created: $tempFile"
} catch {
    Write-Error "Failed to process seed files : $($_.Exception.Message)"
    exit 1
}

# Verify temp file exists
if (-not (Test-Path $tempFile)) {
    Write-Error "Processed file $tempFile was not created!"
    exit 1
}

# Run the AWS CLI command
$awsCommand = "aws dynamodb batch-write-item --request-items file://$tempFile --region $region --profile syndicate"
Write-Host "Executing: $awsCommand"
Invoke-Expression $awsCommand

# Clean up
Remove-Item $tempFile -ErrorAction SilentlyContinue

Write-Host "Seeded data into $feedbacksTable"