class DynamoDBSeeder {
    [string]$Region = "eu-west-2"
    [string]$Prefix = "tm2-"
    [string]$SyndicateConfigPath = "../../.syndicate-config-dev/syndicate.yml"
    [hashtable]$SeedFiles = @{
        Locations = "../seed-data-locations.json"
        Dishes = "../seed-data-dishes.json"
        Waiters = "../seed-data-waiters.json"
    }
    [string]$TempFile = "seed-data-processed.json"

    [string] GetResourcesSuffix() {
        try {
            if (-not (Test-Path $this.SyndicateConfigPath)) {
                Write-Warning "Syndicate config file not found. Using default suffix."
                return "-dev4"
            }

            $suffix = (Get-Content $this.SyndicateConfigPath | Where-Object { $_ -match "resources_suffix:" } |
                ForEach-Object { $_ -replace "resources_suffix:\s*", "" }).Trim()

            if ($suffix) { return $suffix } else { return "-dev4" }
        }
        catch {
            Write-Error "Error reading syndicate config: $_"
            return "-dev4"
        }
    }

    [bool] ValidateSeedFiles() {
        $valid = $true
        foreach ($file in $this.SeedFiles.Values) {
            if (-not (Test-Path $file)) {
                Write-Error "Seed file $file not found!"
                $valid = $false
            }
        }
        return $valid
    }

    [string] GetMatchingTable([string]$ResourceType, [string]$Suffix) {
        try {
            $tables = (aws dynamodb list-tables --region $this.Region --profile syndicate --query 'TableNames' --output json | ConvertFrom-Json)
            $matchingTables = $tables | Where-Object { $_ -like "$($this.Prefix)$ResourceType*$Suffix" }

            if ($matchingTables.Count -eq 0) {
                throw "No matching tables found for $ResourceType"
            }

            $dev4Table = $matchingTables | Where-Object { $_ -like "*dev4" }
            if ($dev4Table) {
                return $dev4Table
            } else {
                return $matchingTables[0]
            }
        }
        catch {
            Write-Error "Table matching error: $_"
            return $null
        }
    }

    [hashtable] ProcessSeedData([string]$Suffix) {
        try {
            $requestItems = @{}

            foreach ($key in $this.SeedFiles.Keys) {
                $tableName = $this.GetMatchingTable($key, $Suffix)
                if (-not $tableName) { continue }

                $seedFile = $this.SeedFiles[$key]
                $seedData = Get-Content $seedFile -Raw | ConvertFrom-Json
                $items = $seedData."$key"

                if ($items) {
                    $requestItems[$tableName] = $items
                    Write-Host "Prepared data for table: $tableName (Items: $($items.Count))"
                } else {
                    Write-Warning "No items found in $seedFile for $key"
                }
            }

            return $requestItems
        }
        catch {
            Write-Error "Failed to process seed data: $_"
            return @{}
        }
    }

    [void] Seed() {
        if (-not $this.ValidateSeedFiles()) { return }

        $resourcesSuffix = $this.GetResourcesSuffix()
        $requestItems = $this.ProcessSeedData($resourcesSuffix)

        if ($requestItems.Count -eq 0) {
            Write-Error "No items to seed. Exiting."
            return
        }

        try {
            $jsonContent = $requestItems | ConvertTo-Json -Depth 10
            [System.IO.File]::WriteAllText($this.TempFile, $jsonContent)
            Write-Host "Temporary seed file created: $($this.TempFile)"

            $awsCommand = "aws dynamodb batch-write-item --request-items file://$($this.TempFile) --region $($this.Region) --profile syndicate"
            Write-Host "Executing: $awsCommand"
            Invoke-Expression $awsCommand
        }
        catch {
            Write-Error "Seeding failed: $_"
        }
        finally {
            Remove-Item $this.TempFile -ErrorAction SilentlyContinue
        }
    }
}

try {
    $seeder = [DynamoDBSeeder]::new()
    $seeder.Seed()
}
catch {
    Write-Error "Seeding process encountered an error: $_"
}
