# Define variables
$region = "eu-west-2"
$prefix = "tm2-"
$syndicateFile = "../../.syndicate-config-dev/syndicate.yml"
$tablesSeedFile = "../seed-data-tables.json"
$reservationsSeedFile = "../seed-data-reservations.json"
$feedbacksSeedFile = "../seed-data-feedbacks.json"
$tempFile = "seed-data-processed.json"

# Get the suffix from syndicate.yml
$resourcesSuffix = (Get-Content $syndicateFile | Where-Object { $_ -match "resources_suffix:" }) -replace "resources_suffix:\s*", ""
if (-not $resourcesSuffix) {
    $resourcesSuffix = "-dev3"
    Write-Host "No suffix found in syndicate.yml, defaulting to $resourcesSuffix"
}

# Construct table names
$tablesTable = "$prefix" + "Tables" + "$resourcesSuffix"
$reservationsTable = "$prefix" + "Reservations" + "$resourcesSuffix"
$feedbacksTable = "$prefix" + "LocationFeedbacks" + "$resourcesSuffix"

# Check if seed files exist
if (-not (Test-Path $tablesSeedFile)) {
    Write-Error "Seed file $tablesSeedFile not found!"
    exit 1
}
if (-not (Test-Path $reservationsSeedFile)) {
    Write-Error "Seed file $reservationsSeedFile not found!"
    exit 1
}
if (-not (Test-Path $feedbacksSeedFile)) {
    Write-Error "Seed file $feedbacksSeedFile not found!"
    exit 1
}

# Validate reservation seed data for placeholder waiter IDs
try {
    $reservationsData = Get-Content $reservationsSeedFile -Raw | ConvertFrom-Json
    $reservationsItems = $reservationsData.Reservations

    $placeholderFound = $false
    foreach ($item in $reservationsItems) {
        $waiterId = $item.PutRequest.Item.waiterId.S
        if ($waiterId -eq "PLACEHOLDER_REQUIRES_UPDATE" -or
            $waiterId -match "After waiter signs up" -or
            $waiterId -match "waiterId from user") {

            $reservationId = $item.PutRequest.Item.id.S
            Write-Error "Placeholder waiter ID found in reservation $reservationId"
            $placeholderFound = $true
        }
    }

    if ($placeholderFound) {
        Write-Error "Error: Seed file contains placeholder waiter IDs that need to be updated."
        Write-Error "Please update the waiterId fields with actual waiter IDs from your users table."
        exit 1
    } else {
        Write-Host "Validation passed. All waiter IDs have been properly set."
    }
} catch {
    Write-Error "Failed to validate waiter IDs: $($_.Exception.Message)"
    exit 1
}

# Process JSON: Update table names
try {
    # Read JSON files and extract the inner arrays
    $tablesData = Get-Content $tablesSeedFile -Raw | ConvertFrom-Json
    $reservationsData = Get-Content $reservationsSeedFile -Raw | ConvertFrom-Json
    $feedbacksData = Get-Content $feedbacksSeedFile -Raw | ConvertFrom-Json

     # Extract the 'Locations' array
    $tablesItems = $tablesData.Tables
    $reservationsItems = $reservationsData.Reservations
    $feedbacksItems = $feedbacksData.LocationFeedbacks

    # Create the request structure
    $requestItems = @{
        "$tablesTable" = $tablesItems
        "$reservationsTable" = $reservationsItems
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

Write-Host "Seeded data into $tablesTable and $reservationsTable and $feedbacksTable"