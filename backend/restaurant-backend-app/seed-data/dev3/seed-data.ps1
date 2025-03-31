# Define variables
$region = "eu-west-2"
$prefix = "tm2-"
$syndicateFile = "../../.syndicate-config-dev/syndicate.yml"
$locationsSeedFile = "../seed-data-locations.json"
$dishesSeedFile = "../seed-data-dishes.json"
$employeesSeedFile = "../seed-data-employee-info.json"
$tempFile = "seed-data-processed.json"

# Get the suffix from syndicate.yml
$resourcesSuffix = (Get-Content $syndicateFile | Where-Object { $_ -match "resources_suffix:" }) -replace "resources_suffix:\s*", ""
if (-not $resourcesSuffix) {
    $resourcesSuffix = "-dev3"
    Write-Host "No suffix found in syndicate.yml, defaulting to $resourcesSuffix"
}

# Construct table names
$locationsTable = "$prefix" + "Locations" + "$resourcesSuffix"
$dishesTable = "$prefix" + "Dishes" + "$resourcesSuffix"
$employeesTable = "$prefix" + "EmployeeInfo" + "$resourcesSuffix"

# Check if seed files exist
if (-not (Test-Path $locationsSeedFile)) {
    Write-Error "Seed file $locationsSeedFile not found!"
    exit 1
}
if (-not (Test-Path $dishesSeedFile)) {
    Write-Error "Seed file $dishesSeedFile not found!"
    exit 1
}
if (-not (Test-Path $employeesSeedFile)) {
    Write-Error "Seed file $employeesSeedFile not found!"
    exit 1
}

# Process JSON: Update table names
try {
    # Read JSON files and extract the inner arrays
    $locationsData = Get-Content $locationsSeedFile -Raw | ConvertFrom-Json
    $dishesData = Get-Content $dishesSeedFile -Raw | ConvertFrom-Json
    $employeesData = Get-Content $employeesSeedFile -Raw | ConvertFrom-Json

    $locationsItems = $locationsData.Locations  # Extract the 'Locations' array
    $dishesItems = $dishesData.Dishes
    $employeesItems = $employeesData.EmployeeInfo

    # Create the request structure
    $requestItems = @{
        "$locationsTable" = $locationsItems
        "$dishesTable" = $dishesItems
        "$employeesTable" = $employeesItems
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

Write-Host "Seeded data into $locationsTable and $dishesTable"s