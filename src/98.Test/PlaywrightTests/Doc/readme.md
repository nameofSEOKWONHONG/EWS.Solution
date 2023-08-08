[사용법]
* 참조 사이트 : https://playwright.dev/dotnet/docs/intro
* 아래의 순서대로 설치 및 입력, 실행.

1. 설치
   1. dotnet restore
   2. dotnet build 
   3. cd .\bin\Debug\net7.0
   4. `\playwright.ps1 install`
   
2. 검사
   1. `$env:PWDEBUG=1`
   2. dotnet test --NUnit.NumberOfTestWorkers=5

3. 테스트 제너레이터
   1. .\playwright.ps1 codegen https://localhost:7201
   2. 콘솔에 기록된 사항을 코드 테스트에 반영하여 테스트 수행 (2.2)

