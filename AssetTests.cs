using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace SnipeIt.UiTests;

public class AssetTests : PageTest
{
    [Test]
    public async Task CreateMacbookAndCheckoutToRandomUser()
    {
        string assetTag = $"BEE-{Guid.NewGuid():N}";
        string serial = $"SN-{Guid.NewGuid():N}";

        await Page.GotoAsync("https://demo.snipeitapp.com/login");

        await Page.GetByRole(AriaRole.Textbox,
            new() { Name = "Username", Exact = true })
            .FillAsync("admin");

        await Page.GetByRole(AriaRole.Textbox,
            new() { Name = "Password", Exact = true })
            .FillAsync("password");

        await Page.GetByRole(AriaRole.Button,
            new() { Name = "Login", Exact = true }).ClickAsync();

        await Expect(Page.GetByText("Admin User",
            new() { Exact = true }).First)
            .ToBeVisibleAsync(new() { Timeout = 15000 });

        await Page.GetByText("Create New",
            new() { Exact = true }).ClickAsync();

        await Page.GetByText("Asset",
            new() { Exact = true }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox,
            new() { Name = "Asset Tag", Exact = true })
            .FillAsync(assetTag);

        await Page.Locator("#serial_1").FillAsync(serial);

        await Page.Locator(
            "#model_select_id + span .select2-selection").ClickAsync();

        await Page.GetByRole(AriaRole.Searchbox)
            .FillAsync("Macbook Pro");

        var modelOption = Page.Locator(".select2-results__option")
            .Filter(new()
            {
                HasTextRegex = new Regex("Macbook Pro 13\"")
            }).First;

        await Expect(modelOption)
            .ToBeVisibleAsync(new() { Timeout = 15000 });

        await modelOption.ClickAsync();

        await Page.Locator(
            "#status_select_id + span .select2-selection").ClickAsync();

        await Page.Locator(".select2-results__option")
            .Filter(new()
            {
                HasTextRegex = new Regex("^Ready to Deploy$")
            }).ClickAsync();

        await Page.Locator(
            "#assigned_user_select + span .select2-selection").ClickAsync();

        var userOptions = Page.Locator(".select2-results__option")
            .Filter(new()
            {
                HasTextRegex = new Regex(@"#\d+")
            });

        await Expect(userOptions.First)
            .ToBeVisibleAsync(new() { Timeout = 15000 });

        int count = await userOptions.CountAsync();
        var selectedOption = userOptions.Nth(Random.Shared.Next(count));

        string selectedUser =
            (await selectedOption.InnerTextAsync()).Trim();

        await selectedOption.ClickAsync();

        TestContext.Progress.WriteLine($"Asset tag: {assetTag}");
        TestContext.Progress.WriteLine($"Serial: {serial}");
        TestContext.Progress.WriteLine($"Selected user: {selectedUser}");

        await Page.Locator("#submit_button").ClickAsync();

        var successMessage = Page.Locator(".alert-success")
            .Filter(new() { HasText = assetTag });

        await Expect(successMessage)
            .ToBeVisibleAsync(new() { Timeout = 15000 });

        // Find this run's asset in the main Assets list.
        await Page.GotoAsync("https://demo.snipeitapp.com/hardware");

        var assetSearch = Page.GetByRole(AriaRole.Searchbox,
     new() { Name = "Search", Exact = true });

        await assetSearch.FillAsync(assetTag);
        await assetSearch.PressAsync("Enter");

        var assetRow = Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = assetTag });

        await Expect(assetRow)
            .ToHaveCountAsync(1, new() { Timeout = 15000 });

        await Expect(assetRow)
            .ToContainTextAsync("Macbook Pro 13\"");

        // Navigate to its details from the list.
        await assetRow.GetByRole(AriaRole.Link,
            new() { Name = assetTag, Exact = true }).ClickAsync();

        // Validate the asset details.
        var assetHeading = Page.GetByRole(AriaRole.Heading)
            .Filter(new() { HasText = assetTag });

        await Expect(assetHeading)
            .ToBeVisibleAsync(new() { Timeout = 15000 });

        await Expect(assetHeading)
            .ToContainTextAsync("Macbook Pro 13\"");

        await Expect(Page.GetByText(assetTag, new() { Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByText(serial, new() { Exact = true }))
            .ToBeVisibleAsync();

        // The dropdown includes extra information after the user's name.
        string selectedUserName = selectedUser.Split('(')[0].Trim();

        await Expect(Page.GetByRole(AriaRole.Link,
            new() { Name = selectedUserName, Exact = true }))
            .ToBeVisibleAsync();

        var statusCard = Page.Locator("div")
            .Filter(new()
            {
                Has = Page.GetByRole(AriaRole.Link,
                    new() { Name = selectedUserName, Exact = true })
            })
            .Filter(new() { HasText = "Ready to Deploy" })
            .Last;

        await Expect(statusCard).ToBeVisibleAsync();
        await Expect(statusCard).ToContainTextAsync("Ready to Deploy");
        await Expect(statusCard).ToContainTextAsync("Deployed");

        // Open History from the asset page.
        await Page.Locator("a[href='#history']").ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading,
            new() { Name = "History", Exact = true }))
            .ToBeVisibleAsync();

        // Validate creation and checkout records for this asset.
        var historyRows = Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = assetTag });

        var creationRow = historyRows.Filter(new()
        {
            HasTextRegex = new Regex(@"create\s+new")
        });

        var checkoutRow = historyRows.Filter(new()
        {
            Has = Page.GetByRole(AriaRole.Cell,
                new() { Name = "checkout", Exact = true })
        });
        await Expect(creationRow)
            .ToHaveCountAsync(1, new() { Timeout = 15000 });

        await Expect(creationRow).ToContainTextAsync("Admin User");
        await Expect(creationRow).ToContainTextAsync("Macbook Pro 13\"");

        await Expect(checkoutRow)
            .ToHaveCountAsync(1, new() { Timeout = 15000 });

        await Expect(checkoutRow).ToContainTextAsync("Admin User");
        await Expect(checkoutRow).ToContainTextAsync("Macbook Pro 13\"");

        TestContext.Progress.WriteLine(
            "Asset details, creation history and checkout history validated.");
    }
}