param ($testResultDir = ".\TestResults")

$env:LOCAL_TEST_DIR = $testResultDir.Replace("\","/")

docker-compose -f .\docker-compose.build.yml build
docker-compose -f .\docker-compose.test.yml up
docker-compose -f .\docker-compose.test.yml down

if ((Get-ChildItem -Path $testResultDir |Measure-Object).Count -eq 8) {
    Write-Host "Test results files present. Please review" -ForegroundColor Green
    exit 0
}

Write-Host "Not all test result files are present" -ForegroundColor Red
exit 1    

   