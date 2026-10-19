\# Snipe-IT Playwright Assessment



Automated UI test built with C#, .NET 10, Playwright 1.63.0 and NUnit.



\## Scenario



The test performs the following workflow:



1\. Logs in to the Snipe-IT demo.

2\. Creates a Macbook Pro 13" asset with a unique asset tag and serial number.

3\. Sets the status to Ready to Deploy.

4\. Checks out the asset to a randomly selected user.

5\. Searches for the created asset in the assets list.

6\. Opens the asset details page from the search results.

7\. Validates the asset tag, serial number, model, status and assigned user.

8\. Opens History and verifies the creation and checkout records, including the asset, model and acting administrator.



\## Prerequisites



\- .NET 10 SDK

\- Internet access

\- Chromium browser installed through Playwright

\- PowerShell 7 (`pwsh`) for the standard browser installation command, or the Windows alternative below



\## Download the Project



```powershell

git clone https://github.com/Bee-qa/SnipeIt-Playwright-Assessment.git

cd SnipeIt-Playwright-Assessment

```



Alternatively, download and extract the repository ZIP, then open a terminal in the directory containing `SnipeIt.UiTests.csproj`.



\## Setup



Restore dependencies and build:



```powershell

dotnet restore

dotnet build

```



Install Chromium:



```powershell

pwsh .\\bin\\Debug\\net10.0\\playwright.ps1 install chromium

```



\### Windows alternative



If PowerShell 7 is unavailable or script execution is restricted, use the bundled Playwright CLI:



```powershell

\& ".\\bin\\Debug\\net10.0\\.playwright\\node\\win32\_x64\\node.exe" ".\\bin\\Debug\\net10.0\\.playwright\\package\\cli.js" install chromium

```



\## Run the Test



\### Headless



```powershell

dotnet test --filter CreateMacbookAndCheckoutToRandomUser -- Playwright.BrowserName=chromium Playwright.LaunchOptions.Headless=true

```



\### Visible browser with detailed output



```powershell

dotnet test --filter CreateMacbookAndCheckoutToRandomUser --logger "console;verbosity=detailed" -- Playwright.BrowserName=chromium Playwright.LaunchOptions.Headless=false

```



\## Project Files



| File | Purpose |

| --- | --- |

| `AssetTests.cs` | End-to-end workflow and assertions |

| `SnipeIt.UiTests.csproj` | Target framework and package references |

| `.gitignore` | Excludes build output and local development files |

| `README.md` | Setup, execution and design notes |



\## Design Decisions



\- Playwright's NUnit `PageTest` provides an isolated browser context for each test.

\- GUID-based asset tags and serial numbers avoid collisions between runs.

\- Role-based locators identify controls where practical.

\- CSS selectors target Select2 controls and the History tab.

\- Playwright assertions wait for expected UI conditions.

\- Asset identifiers and the selected user are logged to help investigate failures.

\- The test creates and verifies the asset entirely through the UI.



\## Demo Credentials



Application: https://demo.snipeitapp.com/login



\- Username: `admin`

\- Password: `password`



These are public demo credentials.



\## Assumptions and Limitations



\- Random user selection is from the users currently loaded in the dropdown.

\- Ready to Deploy is the asset's status label. The additional Deployed badge indicates that it is checked out.

\- History validation checks creation and checkout actions, the asset, model and acting administrator. Assigned-user validation occurs on the asset details page.

\- Created assets remain in the demo for review; automated cleanup is not included.

\- The application is a shared public demo. Data resets, availability and changes by other users can affect execution.



\## Validation



The complete workflow passed locally using Chromium on Windows with .NET 10.

