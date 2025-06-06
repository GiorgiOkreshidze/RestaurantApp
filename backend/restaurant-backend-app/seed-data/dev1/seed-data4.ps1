﻿# Define variables
$region = "eu-west-2"
$prefix = "tm2-"
$syndicateFile = "../../.syndicate-config-dev/syndicate.yml"
$reservationsSeedFile = "../seed-data-reservations.json"
$tempFile = "seed-data-processed.json"

# Get the suffix from syndicate.yml
$resourcesSuffix = (Get-Content $syndicateFile | Where-Object { $_ -match "resources_suffix:" }) -replace "resources_suffix:\s*", ""
if (-not $resourcesSuffix) {
    $resourcesSuffix = "-dev5"
    Write-Host "No suffix found in syndicate.yml, defaulting to $resourcesSuffix"
}

# Construct table names
$reservationsTable = "$prefix" + "Reservations" + "$resourcesSuffix"

if (-not (Test-Path $reservationsSeedFile)) {
    Write-Error "Seed file $reservationsSeedFile not found!"
    exit 1
}

# Process JSON: Update table names
try {
    # Read JSON files and extract the inner arrays
    $reservationsData = Get-Content $reservationsSeedFile -Raw | ConvertFrom-Json

    # Extract the 'Locations' array
    $reservationsItems = $reservationsData.Reservations

    # Create the request structure
    $requestItems = @{
        "$reservationsTable" = $reservationsItems
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

Write-Host "Seeded data into $locationsTable and $dishesTable"