using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task HomePageInitTest()
    {
        await Page.GotoAsync("https://localhost:7201");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Fetch Data" }).ClickAsync();

        await Page.GetByText("2", new() { Exact = true }).ClickAsync();

        await Page.GetByText("3", new() { Exact = true }).ClickAsync();

        await Page.GetByText("4", new() { Exact = true }).ClickAsync();

        await Page.Locator("span").Filter(new() { HasText = "10 / 쪽" }).ClickAsync();

        await Page.GetByText("100 / 쪽").ClickAsync();
    }
}